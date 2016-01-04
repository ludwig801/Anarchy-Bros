using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public GameObject RangeWeaponPrefab;
        public float Speed, Health, DeathSpeed;
        public Spot Objective, MoveTo, Spot;
        public Edge Edge;
        public Color ColorDefault, ColorDying, ColorDead;
        public Transform SpriteObj, ColliderObj, GunPoint, BulletCan;
        public bool HasRangeWeapon { get { return _rangeWeapon != null; } }

        EnemyManager _enemyManager;
        MapManager _mapManager;
        TowerManager _towerManager;
        SpriteRenderer _colliderRenderer;
        float _initialHealth, _invInitialHealth, _deltaTime;
        MeleeWeapon _meleeWeapon;
        RangeWeapon _rangeWeapon;
        Collider2D _collider;

        void Start()
        {
            _colliderRenderer = ColliderObj.GetComponent<SpriteRenderer>();
            _enemyManager = EnemyManager.Instance;
            _mapManager = MapManager.Instance;
            _towerManager = TowerManager.Instance;

            _collider = ColliderObj.GetComponent<Collider2D>();

            Spot = _mapManager.SpotAt(transform.position);
            if (Spot == null)
            {
                Destroy(gameObject);
                return;
            }
            Spot.Tower = this;
            Edge = null;
            transform.position = MoveTo.transform.position;
            _initialHealth = Health;
            _invInitialHealth = 1f / _initialHealth;
            _deltaTime = 0;

            if (RangeWeaponPrefab != null)
            {
                GameObject obj = Instantiate(RangeWeaponPrefab, transform.position, Quaternion.identity) as GameObject;
                obj.transform.parent = transform;
                obj.name = RangeWeaponPrefab.name;

                _rangeWeapon = obj.GetComponent<RangeWeapon>();
                _rangeWeapon.BulletCan = BulletCan;
                _rangeWeapon.GunPoint = GunPoint;
            }
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            if (HasRangeWeapon)
            {
                _rangeWeapon.Target = _enemyManager.GetNearestEnemy(this);
                transform.rotation = Quaternion.Lerp(transform.rotation, Tools2D.LookAt(GunPoint.position, _rangeWeapon.AimAt), Time.deltaTime * 5f);
            }

            _colliderRenderer.color = Color.Lerp(ColorDefault, ColorDying, (_initialHealth - Health) * _invInitialHealth);

            _deltaTime += Time.deltaTime;

            if (!IsAlive())
            {
                _colliderRenderer.color = Color.Lerp(_colliderRenderer.color, ColorDead, _deltaTime / DeathSpeed);
                if (_deltaTime >= DeathSpeed)
                {
                    _colliderRenderer.color = ColorDefault;
                    Health = _initialHealth;
                    gameObject.SetActive(false);
                    _towerManager.OnTowerKill();
                    _deltaTime = 0f;
                }
                return;
            }

            MoveTowardsObjective();
        }

        void MoveTowardsObjective()
        {
            if (Tools2D.IsPositionEqual(transform.position, Objective.transform.position))
            {
                Spot = Objective;
                Edge = null;
                return;
            }

            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                Spot = MoveTo;
                Edge = null;
            }

            if (Spot != null)
            {
                MoveTo = _mapManager.NextStep(Spot, Objective);
                _mapManager.EdgeAt(Spot, MoveTo, out Edge);
                Spot = null;
            }
            else if (Edge != null)
            {
                MoveTo = _mapManager.NextStep(transform.position, Edge, Objective);
            }

            transform.rotation = Tools2D.LookAt(transform.position, MoveTo.transform.position);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_collider.OverlapPoint(eventData.pointerCurrentRaycast.worldPosition))
            {
                _towerManager.OnTowerClicked(eventData, this);
            }
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (!IsAlive())
            {
                Die();
                _deltaTime = 0;
            }
        }

        public void Reborn()
        {
            if (HasRangeWeapon)
            {
                _rangeWeapon.enabled = true;
                _rangeWeapon.gameObject.SetActive(true);
            }
            SpriteObj.gameObject.SetActive(true);
        }

        public void Die()
        {
            Health = 0;
            SpriteObj.gameObject.SetActive(false);

            if (HasRangeWeapon)
            {
                _rangeWeapon.enabled = false;
                _rangeWeapon.gameObject.SetActive(false);
            }
        }

        public bool IsAlive()
        {
            return ((Health > 0f) && gameObject.activeSelf);
        }

        public Collider2D GetCollider()
        {
            return _collider;
        }
    }
}