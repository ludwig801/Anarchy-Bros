using UnityEngine;

namespace AnarchyBros
{
    public class Bullet : MonoBehaviour
    {
        public Vector2 Direction;
        public float Speed, Damage;

        void Update()
        {
            transform.position = Tools2D.MoveInDirection(transform.position, Direction, Time.deltaTime * Speed);

            if (MapManager.Instance.OutOfMap(transform.position, transform.localScale))
            {
                Kill();
            }
        }

        void OnTriggerEnter2D(Collider2D data)
        {
            if (data.tag == "Enemy")
            {
                Enemy e = data.GetComponent<Enemy>();
                if (e.IsAlive)
                {
                    e.TakeDamage(Damage);
                    Kill();
                }
            }
        }

        void Kill()
        {
            gameObject.SetActive(false);
        }
    }
}