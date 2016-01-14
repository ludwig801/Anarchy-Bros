using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class ScrollerButton : MonoBehaviour
{
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
    public Color ColorDefault, ColorFocused, ColorLocked;
    public bool Focused, Unlocked;
    public Image ButtonImage
    {
        get
        {
            if (_btnImage == null)
            {
                _btnImage = Button.GetComponent<Image>();
            }
            return _btnImage;
        }
    }
    public Button Button
    {
        get
        {
            if (_btn == null)
            {
                _btn = GetComponent<Button>();
            }
            return _btn;
        }
    }
    public Vector2 AnchorMin
    {
        get
        {
            return RectTransform.anchorMin;
        }
    }
    public Vector2 AnchorMax
    {
        get
        {
            return RectTransform.anchorMax;
        }
    }

    RectTransform _rectTransform;
    Image _btnImage;
    Button _btn;
    Vector2 _anchorMinTo, _anchorMaxTo;
    float _fadeSpeed, _moveSpeed;

    void Update()
    {
        float t = Time.unscaledDeltaTime * _moveSpeed;

        Button.interactable = Unlocked && Focused;
        if (Focused)
        {
            ButtonImage.color = Color.Lerp(ButtonImage.color, Unlocked ? ColorFocused : Color.Lerp(ColorFocused, ColorLocked, 0.75f), t);
        }
        else
        {
            ButtonImage.color = Color.Lerp(ButtonImage.color, Unlocked ? ColorDefault : Color.Lerp(ColorDefault, ColorLocked, 0.75f), t);
        }

        if (!Tools2D.Approximate(RectTransform.anchorMin, _anchorMinTo, 0.001f))
        {
            RectTransform.anchorMin = Vector2.Lerp(RectTransform.anchorMin, _anchorMinTo, t);
        }
        else
        {
            RectTransform.anchorMin = _anchorMinTo;
        }
        if (!Tools2D.Approximate(RectTransform.anchorMax, _anchorMaxTo, 0.001f))
        {
            RectTransform.anchorMax = Vector2.Lerp(RectTransform.anchorMax, _anchorMaxTo, t);
        }
        else
        {
            RectTransform.anchorMax = _anchorMaxTo;
        }
        RectTransform.offsetMin = Vector2.zero;
        RectTransform.offsetMax = Vector2.zero;
    }

    public void MoveTo(Vector2 anchorMin, Vector2 anchorMax, float animationSpeed)
    {
        _moveSpeed = animationSpeed;
        _anchorMinTo = anchorMin;
        _anchorMaxTo = anchorMax;
    }
}
