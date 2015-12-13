using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Tower Objective;
        public Spot MoveTo;
        public Spot CurrentSpot;
        public Edge CurrentEdge;
        public float Speed, Attack, Health, DeathSpeed;
        public Color ColorDying, ColorDead;

        PieceBehavior _pieceBehavior;
        SpriteRenderer _renderer;
        float _initialHealth;
        bool _dying, _animateDefault;

        void Start()
        {
            CurrentSpot = GraphManager.Instance.GetHitSpot(transform.position);
            CurrentEdge = null;
            transform.position = Tools2D.Convert(transform.position, MoveTo.transform.position);

            _pieceBehavior = GetComponent<PieceBehavior>();
            _animateDefault = _pieceBehavior.Animate;
            _renderer = GetComponent<SpriteRenderer>();
            _initialHealth = Health;
            _dying = false;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            _pieceBehavior.Animate = false;

            if (_dying)
            {
                if (Tools.AreColorsEqual(_renderer.color, ColorDead))
                {
                    Debug.Log("Died");
                    Health = _initialHealth;
                    _dying = false;
                    _pieceBehavior.Animate = _animateDefault;
                    _renderer.color = _pieceBehavior.ColorDefault;
                    gameObject.SetActive(false);
                    EnemyManager.Instance.OnEnemyKill();
                }

                _renderer.color = Color.Lerp(_renderer.color, ColorDead, Time.deltaTime * DeathSpeed);

                return;
            }

            UpdateObjective();

            if (Objective == null || !Objective.gameObject.activeSelf)
            {
                Kill();
                return;
            }

            MoveTowardsObjective();
        }

        void UpdateObjective()
        {
            if (Objective == null)
            {
                Objective = EnemyManager.Instance.GetNewObjective();
                if (!Objective.IsAlive)
                {
                    Objective = null;
                }
            }
        }

        void MoveTowardsObjective()
        {
            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                CurrentSpot = GraphManager.Instance.GetHitSpot(MoveTo.transform.position);
                CurrentEdge = null;
            }

            if (CurrentSpot != null)
            {
                if (Objective.CurrentSpot != null)
                {
                    MoveTo = GraphManager.Instance.GetBestSpot(CurrentSpot, Objective.CurrentSpot);
                }
                else if (Objective.CurrentEdge != null)
                {
                    MoveTo = GraphManager.Instance.GetBestSpot(CurrentSpot, Objective.MoveTo);
                }
                CurrentEdge = GraphManager.Instance.GetHitEdge(CurrentSpot, MoveTo);
                CurrentSpot = null;
            }
            else if (CurrentEdge != null)
            {
                if (Objective.CurrentSpot != null)
                {
                    MoveTo = GraphManager.Instance.GetBestSpot(transform.position, CurrentEdge.A, CurrentEdge.B, Objective.CurrentSpot);
                }
                else if (Objective.CurrentEdge != null)
                {
                    MoveTo = GraphManager.Instance.GetBestSpot(transform.position, CurrentEdge.A, CurrentEdge.B, Objective.MoveTo);
                }
            }

            Vector2 delta = Tools2D.Subtract(MoveTo.transform.position, transform.position);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void OnTriggerEnter2D(Collider2D data)
        {
            if (data.tag == "Tower")
            {
                Tower p = data.gameObject.GetComponent<Tower>();
                p.TakeDamage(Attack);
                TakeDamage(Health);
            }
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (Health <= 0f)
            {
                Kill();
            }
        }

        public void Kill()
        {
            _dying = true;
            _pieceBehavior.Animate = false;
            _renderer.color = ColorDying;
            //gameObject.SetActive(false);
            //Health = _initialHealth;
            //EnemyManager.Instance.OnEnemyKill();
        }
    }
}
