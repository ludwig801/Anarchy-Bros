using UnityEngine;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Spot Objective;
        public Spot LocalObjective;
        public float Speed, Attack, Health;

        float _initialHealth;

        void Start()
        {
            _initialHealth = Health;
        }

        void Update()
        {
            if (!Objective.Occupied)
            {
                if (!EnemyManager.Instance.GenerateObjective(LocalObjective, out Objective))
                {
                    Kill();
                    return;
                }
            }

            if (Mathf.Approximately(Vector2.Distance(transform.position, LocalObjective.transform.position), 0f))
            {
                if (Tools2D.IsPositionEqual(LocalObjective.transform.position, Objective.transform.position))
                {
                    Kill();
                }
                else
                {
                    LocalObjective = GraphManager.Instance.NextStep(LocalObjective, Objective);
                }
            }

            Vector2 delta = Tools2D.Subtract(LocalObjective.transform.position, transform.position);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Tools2D.MoveTowards(transform.position, LocalObjective.transform.position, Time.deltaTime * Speed);
        }

        void OnTriggerEnter2D(Collider2D data)
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
