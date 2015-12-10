using UnityEngine;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Vector2 Objective;
        public Vector2 LocalObjective;
        public float Speed, Damage, Health;

        void Start()
        {
            transform.position = MoveTo2D(transform.position, LocalObjective);
        }

        void Update()
        {
            Vector2 delta = LocalObjective - (Vector2)transform.position;
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = MoveTowards2D(transform.position, LocalObjective);

            if (Mathf.Approximately(Vector2.Distance(transform.position, LocalObjective), 0f))
            {
                if (LocalObjective == Objective)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Vector2 newObjective = GraphManager.Instance.NextStep(LocalObjective, Objective);
                    LocalObjective = MoveTo2D(transform.position, newObjective);
                }
            }
        }

        Vector3 MoveTowards2D(Vector3 from, Vector2 to)
        {
            Vector2 v2d = Vector2.MoveTowards(transform.position, LocalObjective, Time.deltaTime * Speed);
            return new Vector3(v2d.x, v2d.y, from.z);
        }

        Vector3 MoveTo2D(Vector3 from, Vector2 to)
        {
            return new Vector3(to.x, to.y, from.z);
        }

        public void OnTriggerEnter2D(Collider2D data)
        {
            if (data.tag == "Tower")
            {
                Tower p = data.gameObject.GetComponent<Tower>();
                p.TakeDamage(Damage);
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
            Destroy(gameObject);
        }
    }
}
