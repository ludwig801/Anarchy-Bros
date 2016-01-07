using UnityEngine;

public class UIController : MonoBehaviour
{
    public RectTransform PanelHealth;
    public RectTransform HealthElementPrefab;

    public HealthElement GetHealthElement()
    {
        HealthElement newElem = Instantiate(HealthElementPrefab).GetComponent<HealthElement>();
        newElem.name = "Health";
        newElem.transform.SetParent(PanelHealth, false);
        return newElem;
    }
}