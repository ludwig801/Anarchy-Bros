using UnityEngine;
using UnityEngine.UI;

public class HealthElement : MonoBehaviour
{
    public PieceBehavior Target;
    public Text TargetName;
    public Slider HealthBar;
    public Image HealthBarFill;
    public Color ColorHealthy, ColorDead;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }

    CanvasGroup _canvasGroup;

    void Update()
    {
        if (Target != null && Target.Alive)
        {
            CanvasGroup.alpha = 1;
            TargetName.text = Target.tag;
            HealthBar.maxValue = Target.MaxHealth;
            HealthBar.value = Target.Health;
            HealthBar.value = Target.Health;
            HealthBarFill.color = Color.Lerp(ColorDead, ColorHealthy, HealthBar.value / HealthBar.maxValue);
        }
        else
        {
            HealthBar.value = HealthBar.maxValue;
            HealthBarFill.color = Color.gray;
            CanvasGroup.alpha = CanvasGroup.alpha - Time.deltaTime * 2f;

            if (CanvasGroup.alpha < 0.01f)
            {
                gameObject.SetActive(false);
            }           
        }
    }
}
