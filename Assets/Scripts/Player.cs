using UnityEngine;

namespace AnarchyBros
{
    public class Player : MonoBehaviour, IKillable
    {
        public float Speed, Health;
        public Collider2D Collider;
        public PlayerSpot CurrentSpot;

        Transform _moveTo;

        public bool IsAlive
        {
            get
            {
                return Health > 0f;
            }
        }

        void Start()
        {
            Collider = GetComponent<Collider2D>();
        }

        void Update()
        {
            _moveTo = CurrentSpot.transform;

            if (!Mathf.Approximately(Vector3.Distance(transform.position, _moveTo.position), 0f))
            {
                transform.position = Vector3.MoveTowards(transform.position, _moveTo.position, Time.deltaTime * Speed);
            }
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (!IsAlive)
            {
                Kill();
            }
        }

        public void Kill()
        {
            Debug.Log("The player died");
        }
    }
}