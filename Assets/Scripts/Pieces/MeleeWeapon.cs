using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class MeleeWeapon : MonoBehaviour
    {
        public float Damage, AttackDelay, FadeSpeed;
        public Tags.Tag CollisionTag;
        float _deltaTime;

        void Start()
        {
            _deltaTime = 0;
        }

        void Update()
        {
            _deltaTime += Time.deltaTime;
        }

        void Attack(IKillable x)
        {
            x.TakeDamage(Damage);
        }

        public void OnTriggerEnter2D(Collider2D data)
        {
            OnCollisionWith(data);
        }

        public void OnTriggerStay2D(Collider2D data)
        {
            OnCollisionWith(data);
        }

        public void OnCollisionWith(Collider2D data)
        {
            if (data.tag != Tags.GetStringTag(CollisionTag))
            {
                return;
            }

            IKillable x = data.transform.parent.GetComponent<IKillable>();
            if (x != null && x.IsAlive())
            {
                if (_deltaTime >= AttackDelay)
                {
                    Attack(x);
                    _deltaTime = 0;
                }
            }
        }
    }
}