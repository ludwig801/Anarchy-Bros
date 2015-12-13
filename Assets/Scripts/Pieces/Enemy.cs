using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Spot Objective, MoveTo, CurrentSpot;
        public Edge CurrenteEdge;
        public float Speed, Attack, Health;

        float _initialHealth;

        void Start()
        {
            CurrentSpot = GraphManager.Instance.GetHitSpot(transform.position);
            CurrenteEdge = null;
            transform.position = MoveTo.transform.position;
            _initialHealth = Health;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            if (!Objective.Occupied)
            {
                if (!EnemyManager.Instance.GenerateObjective(CurrentSpot, out Objective))
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
                CurrentSpot = MoveTo;
                CurrenteEdge = null;
            }

            if (CurrentSpot == Objective)
            {
                return;
            }

            if (CurrentSpot != null)
            {
                MoveTo = GraphManager.Instance.GetNextSpot(CurrentSpot, Objective);
                CurrenteEdge = GraphManager.Instance.GetHitEdge(CurrentSpot, MoveTo);
                CurrentSpot = null;
            }

            if (CurrenteEdge != null)
            {
                MoveTo = GraphManager.Instance.GetNextSpot(CurrenteEdge.A, CurrenteEdge.B, Objective);
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
            gameObject.SetActive(false);
            Health = _initialHealth;
            EnemyManager.Instance.OnEnemyKill();
        }
    }
}
