using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ScrollerBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Public vars
    public GameObject OptionPrefab, OptionSmallPrefab;
    public RectTransform Content, ContentSmall;
    [Range(0, 1)]
    public float VerticalScale;
    [Range(0, 1)]
    public float VerticalScaleSmall;
    public float AnimationSpeed, DragSensibility;
    public List<ScrollerButton> Buttons;
    public List<ScrollerButtonSmall> ButtonsSmall;
    // Properties
    public int OptionsMin
    {
        get
        {
            return 0;
        }
    }
    public int OptionsMax
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].gameObject.activeSelf)
                {
                    count++;
                }
            }
            return Mathf.Max(count - 1, 0);
        }
    }
    public int CurrentOption
    {
        get
        {
            _currentOption = Mathf.Clamp(_currentOption, OptionsMin, OptionsMax);
            return _currentOption;
        }

        set
        {
            _currentOption = Mathf.Clamp(value, OptionsMin, OptionsMax);
        }
    }
    public MenuUIManager MenuManager
    {
        get
        {
            if (_menuManager == null)
            {
                _menuManager = MenuUIManager.Instance;
            }
            return _menuManager;
        }
    }
    // Private vars
    MenuUIManager _menuManager;
    Vector2 _dragDelta;
    float _aLeft, _aRight, _aTop, _aBottom, _aSmallOffsetX;
    float _contentProportion, _contentSmallProportion;
    float _dragTime;
    int _currentOption;

    void Start()
    {
        _menuManager = MenuUIManager.Instance;

        _contentProportion = Content.rect.height / Content.rect.width;
        _aLeft = 0.5f * (1 - VerticalScale * _contentProportion);
        _aRight = 0.5f * (1 + VerticalScale * _contentProportion);
        _aTop = 0.5f * (1 + VerticalScale);
        _aBottom = 0.5f * (1 - VerticalScale);

        _contentSmallProportion = ContentSmall.rect.height / ContentSmall.rect.width;
        _aSmallOffsetX = 0.5f * VerticalScaleSmall * _contentSmallProportion;
    }

    void Update()
    {
        if (Buttons.Count > 0)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                ScrollerButton btn = Buttons[i];
                if (btn.gameObject.activeSelf)
                {
                    ScrollerButtonSmall btnSmall = ButtonsSmall[i];
                    Vector2 anchorMinTo;
                    Vector2 anchorMaxTo;

                    int offset = (i - CurrentOption);

                    GetAnchors(offset, out anchorMinTo, out anchorMaxTo);
                    btn.Focused = (offset == 0);
                    btn.MoveTo(anchorMinTo, anchorMaxTo, AnimationSpeed);

                    GetAnchorsSmall(offset, out anchorMinTo, out anchorMaxTo);
                    btnSmall.Focused = btn.Focused;
                    btnSmall.MoveTo(anchorMinTo, anchorMaxTo, AnimationSpeed);
                }
            }
        }
    }

    public void RemoveOptions()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].gameObject.SetActive(false);
            ButtonsSmall[i].gameObject.SetActive(false);
        }
        CurrentOption = 0;
    }

    bool GetButton(out ScrollerButton btn, out ScrollerButtonSmall btnSmall)
    {
        btnSmall = null;
        btn = null;

        for (int i = 0; i < Buttons.Count; i++)
        {
            if (!Buttons[i].gameObject.activeSelf)
            {
                btn = Buttons[i];
                btnSmall = ButtonsSmall[i];
                break;
            }
        }

        return (btn != null && btnSmall != null);
    }

    public void AddOption(string name, Sprite img, bool unlocked)
    {
        ScrollerButton btn;
        ScrollerButtonSmall btnSmall;

        if (!GetButton(out btn, out btnSmall))
        {
            btn = Instantiate(OptionPrefab).GetComponent<ScrollerButton>();
            btn.transform.SetParent(Content);
            btnSmall = Instantiate(OptionSmallPrefab).GetComponent<ScrollerButtonSmall>();
            btnSmall.transform.SetParent(ContentSmall);
            Buttons.Add(btn);
            ButtonsSmall.Add(btnSmall);
            int i = Buttons.Count - 1;

            btnSmall.Button.onClick.AddListener(() =>
            {
                CurrentOption = i;
            });
            btn.Button.onClick.AddListener(() =>
            {
                CurrentOption = i;
                if (btn.Focused)
                {
                    MenuManager.NextMenu();
                }
            });
        }
        btn.gameObject.SetActive(true);
        btn.name = name;
        btn.Unlocked = unlocked;       
        if (img != null)
        {
            btn.ButtonImage.sprite = img;
        }
        btnSmall.gameObject.SetActive(true);
        btnSmall.name = name;
        btnSmall.Unlocked = unlocked;
    }

    void GetAnchors(int offset, out Vector2 anchorMin, out Vector2 anchorMax)
    {
        if (offset == 0)
        {
            anchorMin = new Vector2(_aLeft, _aBottom);
            anchorMax = new Vector2(_aRight, _aTop);
        }
        else if (offset < 0)
        {
            anchorMin = new Vector2(_aLeft - _aRight, _aBottom);
            anchorMax = new Vector2(0, _aTop);
        }
        else
        {
            anchorMin = new Vector2(1, _aBottom);
            anchorMax = new Vector2(1 + _aRight - _aLeft, _aTop);
        }
    }

    void GetAnchorsSmall(int offset, out Vector2 anchorMin, out Vector2 anchorMax)
    {
        float vScale = (offset == 0) ? VerticalScaleSmall : 0.5f * VerticalScaleSmall;
        float vBorder = (1 - vScale) * 0.5f;
        float xOffset = 0;
        if (offset > 0)
        {
            xOffset = _aSmallOffsetX + (vScale * _contentSmallProportion) * (offset - 0.5f);
        }
        else if (offset < 0)
        {
            xOffset = (vScale * _contentSmallProportion) * (offset + 0.5f) - _aSmallOffsetX;
        }
        Vector2 center = new Vector2(0.5f + xOffset, 0.5f);
        
        anchorMin = new Vector2(center.x - 0.5f * vScale * _contentSmallProportion, vBorder);
        anchorMax = new Vector2(center.x + 0.5f * vScale * _contentSmallProportion, 1 - vBorder);
    }

    float GetFadeFactor(int offset)
    {
        return (offset == 0) ? 0 : 0.75f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragDelta = new Vector2(0, 0);
        _dragTime = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragDelta += eventData.delta;
        _dragTime += Time.unscaledDeltaTime;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragDelta += eventData.delta;
        _dragTime += Time.unscaledDeltaTime;

        float percDragX = Tools2D.Abs(_dragDelta).x / Screen.width;
        if (percDragX < DragSensibility && _dragDelta.x != 0)
        {
            Buttons[CurrentOption].Focused = false;
            CurrentOption -= (int)(1 * Mathf.Sign(_dragDelta.x));
            Buttons[CurrentOption].Focused = true;
        }
    }
}
