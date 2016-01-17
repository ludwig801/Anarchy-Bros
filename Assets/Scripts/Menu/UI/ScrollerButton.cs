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
    float _fadeSpeed, _animSpeed;

    void Start()
    {
        RectTransform.anchorMin = Vector2.zero;
        RectTransform.anchorMax = Vector2.one;
        RectTransform.offsetMin = Vector2.zero;
        RectTransform.offsetMax = Vector2.zero;
    }

    void Update()
    {
        float t = Time.unscaledDeltaTime * _animSpeed;

        Button.interactable = Unlocked;
        if (Focused)
        {
            ButtonImage.color = Color.Lerp(ButtonImage.color, Unlocked ? ColorFocused : Color.Lerp(ColorFocused, ColorLocked, 0.75f), t);
        }
        else
        {
            ButtonImage.color = Color.Lerp(ButtonImage.color, Unlocked ? ColorDefault : Color.Lerp(ColorDefault, ColorLocked, 0.75f), t);
        }

        RectTransform.anchorMin = Vector2.Lerp(RectTransform.anchorMin, _anchorMinTo, t);
        RectTransform.anchorMax = Vector2.Lerp(RectTransform.anchorMax, _anchorMaxTo, t);
        RectTransform.offsetMin = Vector2.zero;
        RectTransform.offsetMax = Vector2.zero;
    }

    public void MoveTo(Vector2 anchorMin, Vector2 anchorMax, float animationSpeed)
    {
        _animSpeed = animationSpeed;
        _anchorMinTo = anchorMin;
        _anchorMaxTo = anchorMax;
    }
}
