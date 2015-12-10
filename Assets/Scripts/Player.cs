using UnityEngine;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class Player : MonoBehaviour, IKillable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Color ColorDefault = Color.cyan, ColorHighlight = Color.white;
        public float Speed, Health;
        public Collider2D Collider;
        public PlayerSpot CurrentSpot;

        Vector3 _moveTo;
        bool _highlight;
        SpriteRenderer _sprite;

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
            _moveTo = transform.position;

            _sprite = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (GameManager.Instance.IsPlay)
            {
                if (CurrentSpot == null)
                {
                    CurrentSpot = NodeManager.Instance.GetHitNode<PlayerSpot>(transform.position);
                    CurrentSpot.Player = this;
                }
                else
                {
                    if (CurrentSpot.Player != this)
                    {
                        CurrentSpot.Player = this;
                    }
                    _moveTo = new Vector3(CurrentSpot.transform.position.x, CurrentSpot.transform.position.y, _moveTo.z);
                }

                if (!Mathf.Approximately(Vector3.Distance(transform.position, _moveTo), 0f))
                {
                    transform.position = Vector3.MoveTowards(transform.position, _moveTo, Time.deltaTime * Speed);
                }

                _sprite.color = Color.Lerp(_sprite.color, _highlight ? ColorHighlight : ColorDefault, Time.deltaTime * 20f);
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            _highlight = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _highlight = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlayerManager.Instance.OnPlayerClicked(this);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);

            if (!value)
            {
                _highlight = false;
            }
        }
    }
}