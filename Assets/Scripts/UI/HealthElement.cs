using UnityEngine;
using UnityEngine.UI;

public class HealthElement : MonoBehaviour
{
    public Piece Target;
    public Text TargetName;
    public Slider HealthBar;
    public Image HealthBarFill;
    public Color ColorHealthy, ColorDead;

    void Update()
    {
        if (Target != null && Target.Alive)
        {
            TargetName.text = Target.tag;
            HealthBar.maxValue = Target.MaxHealth;
            HealthBar.value = Target.Health;
            HealthBar.value = Target.Health;
            if (HealthBar.value <= 0)
            {
                HealthBarFill.color = Color.clear;
            }
            else
            {
                HealthBarFill.color = Color.Lerp(ColorDead, ColorHealthy, HealthBar.value / HealthBar.maxValue);
            }
        }
    }
}
