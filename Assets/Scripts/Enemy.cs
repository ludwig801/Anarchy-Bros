using UnityEngine;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Vector2 Objective;
        public Vector2 LocalObjective;
        public float Speed, Damage, Health;

        //SpriteRenderer _renderer;

        void Start()
        {
            //_renderer = GetComponent<SpriteRenderer>();
            //_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0f);
            transform.position = MoveTo2D(transform.position, LocalObjective);
        }

        void Update()
        {
            MoveToObjective();

            //_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Mathf.Lerp(_renderer.color.a, 1f, Time.deltaTime * Speed));
        }

        void MoveToObjective()
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
                    Vector2 newObjective = NodeManager.Instance.NextStep(LocalObjective, Objective);
                    //Debug.Log(transform.position + " --> " + newObjective);
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

        void OnTriggerEnter2D(Collider2D data)
        {
            if (data.tag == "Player")
            {
                Player p = data.gameObject.GetComponent<Player>();
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
