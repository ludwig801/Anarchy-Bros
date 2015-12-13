using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public float Speed, Health, DeathSpeed;
        public Spot Objective, MoveTo, CurrentSpot;
        public Edge CurrentEdge;
        public Color ColorHurt, ColorDying, ColorDead;
        public MeleeWeapon MeleeWeapon;

        public bool IsAlive { get { return Health > 0f && gameObject.activeSelf; } }

        PieceBehavior _pieceBehavior;
        SpriteRenderer _renderer;
        bool _animateDefault, _dying, _hurt;
        float _initialHealth;

        void Start()
        {
            _pieceBehavior = GetComponent<PieceBehavior>();
            _renderer = GetComponent<SpriteRenderer>();
            CurrentSpot = GraphManager.Instance.GetHitSpot(transform.position);
            CurrentSpot.Tower = this;
            CurrentEdge = null;
            transform.position = MoveTo.transform.position;
            _initialHealth = Health;
            _animateDefault = _pieceBehavior.Animate;
            _dying = false;
            _hurt = false;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            _pieceBehavior.Animate = false;

            if (_hurt)
            {
                if (Tools.AreColorsEqual(_renderer.color, ColorDead))
                {
                    _hurt = false;
                    _renderer.color = _pieceBehavior.ColorDefault;
                }

                _renderer.color = Color.Lerp(_renderer.color, _pieceBehavior.ColorDefault, Time.deltaTime * DeathSpeed);
                return;
            }

            if (_dying)
            {
                if (Tools.AreColorsEqual(_renderer.color, ColorDead))
                {
                    Debug.Log("Died");
                    _dying = false;
                    _pieceBehavior.Animate = _animateDefault;
                    _renderer.color = _pieceBehavior.ColorDefault;
                    Health = _initialHealth;
                    gameObject.SetActive(false);
                    TowerManager.Instance.OnTowerKill();
                }

                _renderer.color = Color.Lerp(_renderer.color, ColorDead, Time.deltaTime * DeathSpeed);
                return;
            }

            MoveTowardsObjective();
        }

        void MoveTowardsObjective()
        {
            if (Tools2D.IsPositionEqual(transform.position, Objective.transform.position))
            {
                return;
            }

            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                CurrentSpot = MoveTo;
                CurrentEdge = null;
            }

            if (CurrentSpot != null)
            {
                MoveTo = GraphManager.Instance.GetBestSpot(CurrentSpot, Objective);
                CurrentEdge = GraphManager.Instance.GetHitEdge(CurrentSpot, MoveTo);
                CurrentSpot = null;
            }
            else if (CurrentEdge != null)
            {
                MoveTo = GraphManager.Instance.GetBestSpot(transform.position, CurrentEdge.A, CurrentEdge.B, Objective);
            }

            Vector2 delta = Tools2D.Subtract(MoveTo.transform.position, transform.position);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (!IsAlive)
            {
                if (!_dying) // don't kill an already dying man
                {
                    Kill();
                }
            }
            else
            {
                _hurt = true;
                _renderer.color = ColorHurt;
            }
        }

        public void Kill()
        {
            _hurt = false;
            _dying = true;
            _renderer.color = ColorDying;
            MeleeWeapon.gameObject.SetActive(false);
            //gameObject.SetActive(false);
            //Health = _initialHealth;
            //TowerManager.Instance.OnTowerKill();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TowerManager.Instance.OnTowerClicked(eventData, this);
        }
    }
}