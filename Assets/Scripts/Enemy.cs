using UnityEngine;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Transform Objective;
        public float Speed, Damage, Health;

        //SpriteRenderer _renderer;

        void Start()
        {
            //_renderer = GetComponent<SpriteRenderer>();
            //_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0f);
        }

        void Update()
        {
            if (Objective == null)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, Objective.position, Time.deltaTime * Speed);

            if (Mathf.Approximately(Vector3.Distance(transform.position, Objective.position), 0f))
            {
                Destroy(gameObject);
            }

            //_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Mathf.Lerp(_renderer.color.a, 1f, Time.deltaTime * Speed));
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
