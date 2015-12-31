using AnarchyBros.Enums;
using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class MeleeWeapon : MonoBehaviour
    {
        public float Damage, AttackDelay, FadeSpeed;
        public Tags.Tag CollisionTag;
        public List<IKillable> Targets;
        public List<Collider2D> TargetsColliders;
        float _deltaTime;

        void Start()
        {
            _deltaTime = 0;
            Targets = new List<IKillable>();
            TargetsColliders = new List<Collider2D>();
        }

        void Update()
        {
            _deltaTime += Time.deltaTime;

            RemoveDeadTargets();

            if (Targets.Count <= 0) return;

            if (_deltaTime >= AttackDelay)
            {
                IKillable target = Targets[0];

                if (target != null && target.IsAlive())
                {
                    target.TakeDamage(Damage);
                    _deltaTime = 0;
                }
            }
        }

        public void OnCollisionStart(Collider2D other)
        {
            if (other.tag != Tags.GetStringTag(CollisionTag)) return;
            if (HasTarget(other)) return;

            IKillable x = other.transform.parent.GetComponent<IKillable>();
            Targets.Add(x);
            TargetsColliders.Add(other);
        }

        public void OnCollisionEnd(Collider2D other)
        {
            RemoveTarget(other);
        }

        bool HasTarget(Collider2D other)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].GetCollider() == other)
                {
                    return true;
                }
            }

            return false;
        }

        void RemoveDeadTargets()
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                if (!Targets[i].IsAlive())
                {
                    Targets.RemoveAt(i);
                    TargetsColliders.RemoveAt(i);
                    i--;
                }
            }
        }

        void RemoveTarget(Collider2D data)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].GetCollider() == data)
                {
                    Targets.RemoveAt(i);
                    TargetsColliders.RemoveAt(i);
                    return;
                }
            }
        }
    }
}