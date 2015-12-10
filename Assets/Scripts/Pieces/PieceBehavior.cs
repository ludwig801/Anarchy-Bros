using UnityEngine;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class PieceBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Color ColorDefault, ColorHighlight;

        bool _highlight;
        SpriteRenderer _renderer;

        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _highlight = false;
        }

        void Update()
        {
            _renderer.color = Color.Lerp(_renderer.color, _highlight ? ColorHighlight : ColorDefault, Time.deltaTime * 20f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _highlight = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _highlight = false;
        }
    }
}