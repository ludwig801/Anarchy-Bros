using UnityEngine;
using UnityEngine.UI;

namespace AnarchyBros
{
    public class PieceHealth : MonoBehaviour
    {
        public Slider HealthBar;

        RectTransform _rectTransform;
        //IKillable _piece;

        void Start()
        {
            //_piece = GetComponent<IKillable>();
            _rectTransform = HealthBar.GetComponent<RectTransform>();

            _rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(transform.position);
        }

        void Update()
        {

        }
    }
}