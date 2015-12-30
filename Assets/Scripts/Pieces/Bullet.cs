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

        void Start()
        {
            _gameManager = GameManager.Instance;
        }

        void Update()
        {
            if (!_gameManager.IsCurrentState(GameStates.Play))
            {
                return;
            }

            transform.position = Tools2D.MoveInDirection(transform.position, Direction, Time.deltaTime * Speed);

            if (MapManager.Instance.OutOfMap(transform.position, transform.localScale))
            {
                Kill();
            }
        }

        void OnTriggerEnter2D(Collider2D data)
        {
            if (data.tag != Tags.GetStringTag(CollisionTag))
            {
                return;
            }

            IKillable x = data.transform.parent.GetComponent<IKillable>();
            if (x != null && x.IsAlive())
            {
                x.TakeDamage(Damage);
                Kill();
            }
        }

        void Kill()
        {
            gameObject.SetActive(false);
        }
    }
}