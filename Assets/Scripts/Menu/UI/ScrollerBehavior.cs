using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScrollerBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject OptionButtonPrefab;
    public RectTransform Content;
    public float OptionScaleCurrent, OptionScaleDefault, AnimationSpeed, DragSensibility;
    public List<ScrollerButton> Buttons;
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
            return Mathf.Max(Buttons.Count - 1, 0);
        }
    }
    public int CurrentOption
    {
        get
        {
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

    MenuUIManager _menuManager;
    Vector2 _dragDelta;
    float _dragTime;
    float _centerBtnRightEdge, _centerBtnLeftEdge;
    int _currentOption;

    void Start()
    {
        _menuManager = MenuUIManager.Instance;

        _centerBtnRightEdge = 0.5f + 0.5f * OptionScaleCurrent;
        _centerBtnLeftEdge = 0.5f - 0.5f * OptionScaleCurrent;
    }

    void Update()
    {
        if (Buttons.Count > 0)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                ScrollerButton btn = Buttons[i];
                int offset = (i - CurrentOption);
                Vector2 anchorX = GetAnchorX(offset);
                btn.Focused = (offset == 0);
                btn.MoveTo(new Vector2(anchorX.x, btn.RectTransform.anchorMin.y), new Vector2(anchorX.y, btn.RectTransform.anchorMax.y), AnimationSpeed);
            }
        }
    }

    public void RemoveOptions()
    {
        Buttons.Clear();
    }

    public void AddOption(string name, Sprite img, bool unlocked)
    {
        ScrollerButton btn = Instantiate(OptionButtonPrefab).GetComponent<ScrollerButton>();
        btn.transform.SetParent(Content);
        btn.name = name;
        btn.Unlocked = unlocked;
        int i = Buttons.Count;
        if (img != null)
        {
            btn.ButtonImage.sprite = img;
        }
        btn.Button.onClick.AddListener(() =>
        {
            CurrentOption = i;
            if (btn.Focused)
            {
                MenuManager.NextMenu();
            }   
        });
        Buttons.Add(btn);
    }

    Vector2 GetAnchorX(int offset)
    {
        float anchorMin;
        float anchorMax;

        if (offset > 0)
        {
            anchorMin = (_centerBtnRightEdge) + (offset - 1) * OptionScaleDefault;
            anchorMax = (_centerBtnRightEdge) + offset * OptionScaleDefault;
        }
        else if (offset < 0)
        {
            anchorMin = (_centerBtnLeftEdge) + offset * OptionScaleDefault;
            anchorMax = (_centerBtnLeftEdge) + (offset + 1) * OptionScaleDefault;
        }
        else
        {
            anchorMin = (_centerBtnLeftEdge);
            anchorMax = (_centerBtnRightEdge);
        }

        return new Vector2(anchorMin, anchorMax);
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
