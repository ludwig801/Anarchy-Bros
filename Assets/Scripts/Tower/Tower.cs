using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public float Speed, Health, DeathSpeed;
        public Spot Objective, MoveTo, Spot;
        public Edge Edge;
        public Color ColorDefault, ColorDying, ColorDead;
        public MeleeWeapon MeleeWeapon;
        public RangeWeapon RangeWeapon;

        public bool IsAlive { get { return (Health > 0f) && gameObject.activeSelf; } }

        SpriteRenderer _renderer;
        float _initialHealth, _invInitialHealth, _deltaTime;

        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            Spot = MapManager.Instance.SpotAt(transform.position);
            Spot.Tower = this;
            Edge = null;
            transform.position = MoveTo.transform.position;
            _initialHealth = Health;
            _invInitialHealth = 1f / _initialHealth;
            _deltaTime = 0;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            if (RangeWeapon != null)
            {
                RangeWeapon.EnemyTarget = EnemyManager.Instance.GetNearestEnemy(this);
            }

            _renderer.color = Color.Lerp(ColorDefault, ColorDying, (_initialHealth - Health) * _invInitialHealth);

            _deltaTime += Time.deltaTime;

            if (!IsAlive)
            {
                _renderer.color = ColorDead;
                if (_deltaTime >= DeathSpeed)
                {
                    Debug.Log("Tower Died");
                    _renderer.color = ColorDefault;
                    Health = _initialHealth;
                    gameObject.SetActive(false);
                    TowerManager.Instance.OnTowerKill();
                }
                return;
            }

            MoveTowardsObjective();
        }

        void MoveTowardsObjective()
        {
            if (Tools2D.IsPositionEqual(transform.position, Objective.transform.position))
            {
                return;
            }

            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                Spot = MoveTo;
                Edge = null;
            }

            if (Spot != null)
            {
                MoveTo = MapManager.Instance.NextStep(Spot, Objective);
                Edge = MapManager.Instance.EdgeAt(Spot, MoveTo);
                Spot = null;
            }
            else if (Edge != null)
            {
                MoveTo = MapManager.Instance.NextStep(transform.position, Edge, Objective);
            }

            transform.rotation = Tools2D.LookAt(transform.position, MoveTo.transform.position);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void Reborn()
        {
            if (MeleeWeapon != null)
            {
                MeleeWeapon.Enabled = true;
            }
            if (RangeWeapon != null)
            {
                RangeWeapon.Enabled = true;
            }
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (!IsAlive)
            {
                Kill();
                _deltaTime = 0;
            }
        }

        public void Kill()
        {
            Health = 0;
            if (MeleeWeapon != null)
            {
                MeleeWeapon.Enabled = false;
            }
            if (RangeWeapon != null)
            {
                RangeWeapon.Enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TowerManager.Instance.OnTowerClicked(eventData, this);
        }
    }
}