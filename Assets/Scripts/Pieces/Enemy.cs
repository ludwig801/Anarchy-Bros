using AnarchyBros.Enums;
using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class Enemy : MonoBehaviour, IKillable
    {
        public Transform SpriteObj, ColliderObj;
        public GameObject MeleeWeapon;
        public Tower Objective;
        public Spot MoveTo, Spot;
        public Edge Edge;
        public float Speed, Health, DeathSpeed;
        public Color ColorDefault, ColorDying, ColorDead;
        public Vector2 Direction;
        public Tags.Tag CollisionTag;
        public bool HasMeleeWeapon { get { return _meleeWeapon != null; } }

        MeleeWeapon _meleeWeapon;
        List<SpriteRenderer> _renderers;
        SpriteRenderer _colliderRenderer;
        float _initialHealth, _invInitialHealth, _deltaTime;
        Collider2D _collider;
        bool _isAttacking;

        void Start()
        {
            if(MoveTo == null)
            {
                Destroy(gameObject);
                return;
            }

            if (MeleeWeapon != null)
            {
                _meleeWeapon = (Instantiate(MeleeWeapon, transform.position, Quaternion.identity) as GameObject).GetComponent<MeleeWeapon>();
                _meleeWeapon.name = MeleeWeapon.name;
                _meleeWeapon.transform.parent = transform;
            }

            Spot = MapManager.Instance.SpotAt(transform.position);
            Edge = null;
            transform.position = Tools2D.Convert(transform.position, MoveTo.transform.position);

            _renderers = new List<SpriteRenderer>();
            for (int i = 0; i < SpriteObj.childCount; i++)
            {
                _renderers.Add(SpriteObj.GetChild(i).GetComponent<SpriteRenderer>());
            }
            _colliderRenderer = ColliderObj.GetComponent<SpriteRenderer>();
            _collider = ColliderObj.GetComponent<Collider2D>();

            _initialHealth = Health;
            _invInitialHealth = 1f / _initialHealth;
            _isAttacking = false;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            _deltaTime += Time.deltaTime;

            SetRenderersColor(Color.Lerp(ColorDefault, ColorDying, (_initialHealth - Health) * _invInitialHealth));

            if (!IsAlive())
            {
                SetRenderersColor(Color.Lerp(ColorDying, ColorDead, _deltaTime / DeathSpeed));

                if (_deltaTime >= DeathSpeed)
                {
                    Spot = null;
                    Edge = null;
                    Health = _initialHealth;
                    SetRenderersColor(ColorDefault);
                    gameObject.SetActive(false);
                    EnemyManager.Instance.OnEnemyKill();
                    _deltaTime = 0;
                }
                return;
            }

            if (Objective == null || !Objective.IsAlive())
            {
                if (!EnemyManager.Instance.GetNewObjective(out Objective))
                {
                    Kill();
                    return;
                }
            }

            MoveTowardsObjective();
        }

        void SetRenderersColor(Color newColor)
        {
            //for (int i = 0; i < _renderers.Count; i++)
            //{
            //    _renderers[i].color = newColor;
            //} 
            _colliderRenderer.color = newColor;
        }

        void MoveTowardsObjective()
        {
            if (_isAttacking) return;

            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                Spot = MoveTo;
                Edge = null;
            }

            if (Spot != null)
            {
                if (Objective.Spot != null)
                {
                    MoveTo = MapManager.Instance.NextStep(Spot, Objective.Spot);
                }
                else if (Objective.Edge != null)
                {
                    MoveTo = MapManager.Instance.NextStep(Spot, Objective.MoveTo);
                }

                Edge = MapManager.Instance.EdgeAt(Spot, MoveTo);
                Spot = null;
            }
            else if (Edge != null)
            {
                if (Objective.Spot != null)
                {
                    MoveTo = MapManager.Instance.NextStep(transform.position, Edge, Objective.Spot);
                }
                else if (Objective.Edge != null)
                {
                    MoveTo = MapManager.Instance.NextStep(transform.position, Edge, Objective.MoveTo);
                }
            }

            Direction = (MoveTo.transform.position - transform.position).normalized;
            transform.rotation = Tools2D.LookAt(Direction);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (Health <= 0f)
            {
                Kill();
            }
        }

        public void Reborn()
        {
            if (_collider != null)
            {
                _collider.enabled = true;
            }
        }

        public void Kill()
        {
            Health = 0;
            _deltaTime = 0;
            _collider.enabled = false;
            _isAttacking = false;
        }

        public bool IsAlive()
        {
            return ((Health > 0f) && gameObject.activeSelf);
        }

        public Collider2D GetCollider()
        {
            return _collider;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag != Tags.GetStringTag(CollisionTag)) return;

            _isAttacking = true;

            if (HasMeleeWeapon)
            {
                _meleeWeapon.OnCollisionStart(other);
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag != Tags.GetStringTag(CollisionTag)) return;

            if (HasMeleeWeapon)
            {
                _meleeWeapon.OnCollisionEnd(other);
                if (_meleeWeapon.Targets.Count <= 0)
                {
                    _isAttacking = false;
                }
            }
            else
            {
                _isAttacking = false;
            }
        }
    }
}
