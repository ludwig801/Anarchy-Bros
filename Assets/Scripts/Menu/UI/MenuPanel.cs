using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    // Public vars
    public Text Title, Description;
    [ReadOnly]
    public bool Visible;
    [ReadOnly]
    public Vector2 HiddenAnchorMin;
    [ReadOnly]
    public Vector2 HiddenAnchorMax;
    // Properties
    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }
    // Private vars
    RectTransform _rectTransform;
    float _animSpeed;

    void Update()
    {
        if (Visible)
        {
            RectTransform.anchorMax = Vector2.Lerp(RectTransform.anchorMax, Vector2.one, Time.unscaledDeltaTime * _animSpeed);
            RectTransform.anchorMin = Vector2.Lerp(RectTransform.anchorMin, Vector2.zero, Time.unscaledDeltaTime * _animSpeed);
        }
        else
        {
            RectTransform.anchorMax = Vector2.Lerp(RectTransform.anchorMax, HiddenAnchorMax, Time.unscaledDeltaTime * _animSpeed);
            RectTransform.anchorMin = Vector2.Lerp(RectTransform.anchorMin, HiddenAnchorMin, Time.unscaledDeltaTime * _animSpeed);
        }
    }

    public void HideLeft(float animationsSpeed)
    {
        _animSpeed = animationsSpeed;
        HiddenAnchorMin = new Vector2(-1, 0);
        HiddenAnchorMax = new Vector2(0, 1);
        Visible = false;
    }

    public void HideAbove(float animationsSpeed)
    {
        _animSpeed = animationsSpeed;
        HiddenAnchorMin = new Vector2(0, 1);
        HiddenAnchorMax = new Vector2(1, 2);
        Visible = false;
    }

    public void HideRight(float animationsSpeed)
    {
        _animSpeed = animationsSpeed;
        HiddenAnchorMin = new Vector2(1, 0);
        HiddenAnchorMax = new Vector2(2, 1);
        Visible = false;
    }

    public void HideBelow(float animationsSpeed)
    {
        _animSpeed = animationsSpeed;
        HiddenAnchorMin = new Vector2(0, -1);
        HiddenAnchorMax = new Vector2(1, 0);
        Visible = false;
    }

    public void Show(float animationsSpeed)
    {
        _animSpeed = animationsSpeed;
        Visible = true;
    }
}
