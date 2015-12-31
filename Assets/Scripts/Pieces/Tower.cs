using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public GameObject MeleeWeaponPrefab, RangeWeaponPrefab;
        public float Speed, Health, DeathSpeed;
        public Spot Objective, MoveTo, Spot;
        public Edge Edge;
        public Color ColorDefault, ColorDying, ColorDead;
        public Transform ColliderObj;
        public bool MeleeWeaponActive, RangeWeaponActive;
        public bool HasMeleeWeapon { get { return _meleeWeapon != null; } }
        public bool HasRangeWeapon { get { return _rangeWeapon != null; } }

        EnemyManager _enemyManager;
        MapManager _mapManager;
        TowerManager _towerManager;
        SpriteRenderer _renderer;
        float _initialHealth, _invInitialHealth, _deltaTime;
        MeleeWeapon _meleeWeapon;
        RangeWeapon _rangeWeapon;
        Collider2D _collider;

        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
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

            if (MeleeWeaponPrefab != null)
            {
                MeleeWeaponActive = true;
                GameObject obj = Instantiate(MeleeWeaponPrefab, transform.position, Quaternion.identity) as GameObject;
                obj.transform.parent = transform;
                obj.name = MeleeWeaponPrefab.name;
                _meleeWeapon = obj.GetComponent<MeleeWeapon>();
            }

            if (RangeWeaponPrefab != null)
            { 
                RangeWeaponActive = true;
                GameObject obj = Instantiate(RangeWeaponPrefab, transform.position, Quaternion.identity) as GameObject;
                obj.transform.parent = transform;
                obj.name = RangeWeaponPrefab.name;

                GameObject bulletCan = GameObject.FindGameObjectWithTag("Bullets");
                if (bulletCan == null)
                {
                    GameObject x = new GameObject();
                    x.name = "Bullets";
                    x.tag = "Bullets";
                    bulletCan = x;
                }

                _rangeWeapon = obj.GetComponent<RangeWeapon>();
                _rangeWeapon.BulletCan = bulletCan.transform;
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
                _rangeWeapon.EnemyTarget = _enemyManager.GetNearestEnemy(this);
            }

            _renderer.color = Color.Lerp(ColorDefault, ColorDying, (_initialHealth - Health) * _invInitialHealth);

            _deltaTime += Time.deltaTime;

            if (!IsAlive())
            {
                _renderer.color = Color.Lerp(_renderer.color, ColorDead, _deltaTime / DeathSpeed);
                if (_deltaTime >= DeathSpeed)
                {
                    _renderer.color = ColorDefault;
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
                Edge = _mapManager.EdgeAt(Spot, MoveTo);
                Spot = null;
            }
            else if (Edge != null)
            {
                MoveTo = _mapManager.NextStep(transform.position, Edge, Objective);
            }

            transform.rotation = Tools2D.LookAt(transform.position, MoveTo.transform.position);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void Reborn()
        {
            if (HasMeleeWeapon)
            {
                _meleeWeapon.enabled = true;
                _meleeWeapon.gameObject.SetActive(true);
            }
            if (HasRangeWeapon)
            {
                _rangeWeapon.enabled = true;
                _rangeWeapon.gameObject.SetActive(true);
            }
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
                Kill();
                _deltaTime = 0;
            }
        }

        public void Kill()
        {
            Health = 0;
            if (HasMeleeWeapon)
            {
                _meleeWeapon.enabled = false;
                _meleeWeapon.gameObject.SetActive(false);
            }
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

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (HasMeleeWeapon)
            {
                _meleeWeapon.OnCollisionStart(other);
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (HasMeleeWeapon)
            {
                _meleeWeapon.OnCollisionEnd(other);
            }
        }
    }
}