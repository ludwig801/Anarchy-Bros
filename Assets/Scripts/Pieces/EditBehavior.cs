using UnityEngine;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class EditBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Color ColorDefault, ColorMouseOver;
        public bool ScaleOnMouseOver, ScaleOnClick;
        public float AnimationsSpeed, MouseOverScale, ClickScale;

        bool _mouseOver, _clicking;
        SpriteRenderer _renderer;
        Vector3 _startScale, _highlightScale, _clickScale;

        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _mouseOver = false;
            _clicking = false;
            _startScale = transform.localScale;           
        }

        void Update()
        {
            _renderer.color = Color.Lerp(_renderer.color, _mouseOver ? ColorMouseOver : ColorDefault, Time.deltaTime * AnimationsSpeed);

            _highlightScale = MouseOverScale * _startScale;
            _clickScale = ClickScale * _startScale;

            if (ScaleOnClick && _clicking)
            {
                transform.localScale = Tools2D.MoveTowards(transform.localScale, _clickScale, Time.deltaTime * AnimationsSpeed);
            }
            else if (ScaleOnMouseOver)
            {
                transform.localScale = Tools2D.MoveTowards(transform.localScale, _mouseOver ? _highlightScale : _startScale, Time.deltaTime * AnimationsSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOver = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _clicking = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _clicking = false;
        }
    }
}