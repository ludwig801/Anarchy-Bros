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
        public Animator Anim;
        public bool IsAttacking;

        EnemyManager _enemyManager;
        GameManager _gameManager;
        MapManager _mapManager;
        MeleeWeapon _meleeWeapon;
        List<SpriteRenderer> _renderers;
        SpriteRenderer _colliderRenderer;
        float _initialHealth, _invInitialHealth, _deltaTime;
        Collider2D _collider;

        void Start()
        {
            _enemyManager = EnemyManager.Instance;
            _gameManager = GameManager.Instance;
            _mapManager = MapManager.Instance;

            if (MoveTo == null)
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

            Spot =_mapManager.SpotAt(transform.position);
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
            IsAttacking = false;
        }

        void Update()
        {
            if (!_gameManager.IsCurrentState(GameStates.Play))
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
                    _enemyManager.OnEnemyKill();
                    _deltaTime = 0;
                }
                return;
            }

            if (Objective == null || !Objective.IsAlive())
            {
                IsAttacking = false;
                if (Anim.isInitialized)
                {
                    Anim.SetBool("IsAttacking", false);
                }

                if (!_enemyManager.GetNewObjective(out Objective))
                {
                    Die();
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
            if (IsAttacking) return;

            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                transform.position = MoveTo.transform.position;
                Spot = MoveTo;
                Edge = null;
            }

            if (Spot != null)
            {
                if (Objective.Spot != null)
                {
                    MoveTo = _mapManager.NextStep(Spot, Objective.Spot);
                }
                else if (Objective.Edge != null)
                {
                    MoveTo = _mapManager.NextStep(Spot, Objective.MoveTo);
                }

               _mapManager.EdgeAt(Spot, MoveTo, out Edge);
                Spot = null;
            }
            else if (Edge != null)
            {
                if (Objective.Spot != null)
                {
                    MoveTo = _mapManager.NextStep(transform.position, Edge, Objective.Spot);
                }
                else if (Objective.Edge != null)
                {
                    MoveTo = _mapManager.NextStep(transform.position, Edge, Objective.MoveTo);
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
                Die();
            }
        }

        public void Reborn()
        {
            if (_collider != null)
            {
                _collider.enabled = true;
            }
            SpriteObj.gameObject.SetActive(true);
            Anim.enabled = true;
        }

        public void Die()
        {
            Anim.enabled = false;
            Health = 0;
            _deltaTime = 0;
            _collider.enabled = false;
            IsAttacking = false;
            if (HasMeleeWeapon)
            {
                _meleeWeapon.RemoveAllTargets();
            }
            SpriteObj.gameObject.SetActive(false);
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

            IKillable x = other.transform.parent.GetComponent<IKillable>();
            if(x.IsAlive())
            {
                IsAttacking = true;
                if (Anim.isInitialized)
                {
                    Anim.SetBool("IsAttacking", true);
                }

                if (HasMeleeWeapon)
                {
                    _meleeWeapon.OnCollisionStart(other);
                }
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
                    IsAttacking = false;
                    if (Anim.isInitialized)
                    {
                        Anim.SetBool("IsAttacking", false);
                    }
                }
            }
            else
            {
                IsAttacking = false;
                if (Anim.isInitialized)
                {
                    Anim.SetBool("IsAttacking", false);
                }
            }
        }
    }
}
