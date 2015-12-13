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
        public float Speed, Attack, Health;

        float _initialHealth;

        void Start()
        {
            CurrentSpot = GraphManager.Instance.GetHitSpot(transform.position);
            CurrentEdge = null;
            transform.position = Tools2D.Convert(transform.position, MoveTo.transform.position);
            _initialHealth = Health;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            UpdateObjective();

            if (Objective == null)
            {
                Kill();
                return;
            }

            MoveTowardsObjective();
        }

        void UpdateObjective()
        {
            if (Objective == null || !Objective.IsAlive)
            {
                Objective = EnemyManager.Instance.GetNewObjective();
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
                Debug.Log("Enemy Died");
                Kill();
            }
        }

        public void Kill()
        {
            gameObject.SetActive(false);
            Health = _initialHealth;
            EnemyManager.Instance.OnEnemyKill();
        }
    }
}
