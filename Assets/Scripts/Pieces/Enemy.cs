using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Tower Objective;
        public Spot MoveTo;
        public Spot Spot;
        public Edge Edge;
        public float Speed, Attack, Health, DeathSpeed;
        public Color ColorDefault, ColorDying, ColorDead;

        public bool IsAlive { get { return (Health > 0f) && gameObject.activeSelf; } }

        SpriteRenderer _renderer;
        float _initialHealth, _invInitialHealth, _deltaTime;

        void Start()
        {
            Spot = MapManager.Instance.SpotAt(transform.position);
            Edge = null;
            transform.position = Tools2D.Convert(transform.position, MoveTo.transform.position);

            _renderer = GetComponent<SpriteRenderer>();
            _initialHealth = Health;
            _invInitialHealth = 1f / _initialHealth;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            _deltaTime += Time.deltaTime;

            _renderer.color = Color.Lerp(ColorDefault, ColorDying, (_initialHealth - Health) * _invInitialHealth);

            if (!IsAlive)
            {
                _renderer.color = ColorDead;

                if (_deltaTime >= DeathSpeed)
                {
                    Spot = null;
                    Edge = null;
                    Health = _initialHealth;
                    _renderer.color = ColorDefault;
                    gameObject.SetActive(false);
                    EnemyManager.Instance.OnEnemyKill();
                }
                return;
            }

            if (Objective == null || !Objective.IsAlive)
            {
                if (!EnemyManager.Instance.GetNewObjective(out Objective))
                {
                    Kill();
                    return;
                }
            }

            MoveTowardsObjective();
        }

        void MoveTowardsObjective()
        {
            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                Spot = MapManager.Instance.SpotAt(MoveTo.transform.position);
                Edge = null;
            }

            if (Spot != null)
            {
                if (Objective.Spot != null)
                {
                    MoveTo = MapManager.Instance.NextStep(Spot, Objective.Spot);
                }
                else if (Objective.Edge != null)
                {
                    MoveTo = MapManager.Instance.NextStep(Spot, Objective.MoveTo);
                }
                Edge = MapManager.Instance.EdgeAt(Spot, MoveTo);
                Spot = null;
            }
            else if (Edge != null)
            {
                if (Objective.Spot != null)
                {
                    MoveTo = MapManager.Instance.NextStep(transform.position, Edge, Objective.Spot);
                }
                else if (Objective.Edge != null)
                {
                    MoveTo = MapManager.Instance.NextStep(transform.position, Edge, Objective.MoveTo);
                }
            }

            transform.rotation = Tools2D.LookAt(transform.position, MoveTo.transform.position);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void OnTriggerEnter2D(Collider2D data)
        {
            if (data.tag == "Tower")
            {
                Tower p = data.gameObject.GetComponent<Tower>();
                if (p.IsAlive)
                {
                    p.TakeDamage(Attack);
                    TakeDamage(Health);
                }
            }
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (Health <= 0f)
            {
                Kill();
                _deltaTime = 0;
            }
        }

        public void Kill()
        {
            Health = 0;
        }
    }
}
