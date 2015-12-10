using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HidePanelBehavior : MonoBehaviour, IPointerClickHandler
{
    public GameObject MainPanel;
    public GameObject PanelToHide;

    RectTransform _mainPanel;
    float _originalHeight, _minHeight;

    void Start()
    {
        _minHeight = GetComponent<LayoutElement>().minHeight;
        _mainPanel = MainPanel.GetComponent<RectTransform>();
        _originalHeight = _mainPanel.sizeDelta.y;

        //OnPointerClick(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool value = !PanelToHide.activeSelf;
        PanelToHide.SetActive(value);
        _mainPanel.sizeDelta = new Vector2(_mainPanel.sizeDelta.x, value ? _originalHeight : _minHeight);
        _mainPanel.position = new Vector3(_mainPanel.position.x, value ? _originalHeight * 0.5f : _minHeight * 0.5f, _mainPanel.position.z);
    }
}