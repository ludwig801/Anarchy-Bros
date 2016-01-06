using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public RectTransform PanelHealth;
    public RectTransform HealthElementPrefab;
    //public List<HealthElement> HealthElements;

    void Awake()
    {
        Instance = this;
    }

    public HealthElement GetHealthElement()
    {
        //for (int i = 0; i < HealthElements.Count; i++)
        //{
        //    if (!HealthElements[i].gameObject.activeSelf)
        //    {
        //        HealthElements[i].gameObject.SetActive(true);
        //        return HealthElements[i];
        //    }
        //}

        HealthElement newElem = Instantiate(HealthElementPrefab).GetComponent<HealthElement>();
        newElem.name = "Health";
        newElem.transform.SetParent(PanelHealth, false);
        //HealthElements.Add(newElem);

        return newElem;
    }
}