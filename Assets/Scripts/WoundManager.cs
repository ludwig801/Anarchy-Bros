using UnityEngine;
using System.Collections.Generic;

public class WoundManager : MonoBehaviour
{
    public GameObject WoundPrefab;
    public List<WoundBehavior> Objects;

    public void CreateWound(Transform source, Transform target)
    {
        WoundBehavior wound = Find();
        wound.name = "wound";
        wound.transform.SetParent(transform);
        wound.Follow = target;
        wound.transform.position = target.position;
        wound.transform.rotation = Tools2D.LookAt(target.position, source.position);
        wound.Live();

        Objects.Add(wound);
    }

    WoundBehavior Find()
    {
        WoundBehavior wound;

        for (int i = 0; i < Objects.Count; i++)
        {
            wound = Objects[i];
            if (!wound.gameObject.activeSelf)
            {
                return wound;
            }
        }

        wound = Instantiate(WoundPrefab).GetComponent<WoundBehavior>();
        return wound;
    }
}
