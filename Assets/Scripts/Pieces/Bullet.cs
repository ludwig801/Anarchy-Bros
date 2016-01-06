using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class Bullet : MonoBehaviour
    {
        public Vector2 Direction;
        public float Speed, Damage;
        public Tags.Tag CollisionTag;

        GameManager _gameManager;
        MapManager _mapManager;

        float _delta;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _mapManager = MapManager.Instance;

            _delta = 0;
        }

        void Update()
        {
            if (!_gameManager.IsCurrentState(GameStates.Play))
            {
                return;
            }

            transform.position = Tools2D.MoveInDirection(transform.position, Direction, Time.deltaTime * Speed);

            _delta += Time.deltaTime;

            if (_delta > 0.03f)
            {
                _delta = 0;
                if (_mapManager.OutOfMap(transform.position, transform.localScale))
                {
                    Die();
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag != Tags.GetStringTag(CollisionTag)) return;

            IKillable x = other.transform.parent.GetComponent<IKillable>();
            if (x != null && x.IsAlive())
            {
                x.TakeDamage(Damage);
                Die();
            }
        }

        //void OnTriggerStay2D(Collider2D other)
        //{
        //    if (other.tag != Tags.GetStringTag(CollisionTag)) return;

        //    IKillable x = other.transform.parent.GetComponent<IKillable>();
        //    if (x != null && x.IsAlive())
        //    {
        //        x.TakeDamage(Damage);
        //        Die();
        //    }
        //}

        void Die()
        {
            gameObject.SetActive(false);
            transform.position = new Vector3(100, 100, transform.position.z);
        }
    }
}