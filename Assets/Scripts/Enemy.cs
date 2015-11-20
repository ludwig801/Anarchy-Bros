using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform Objective;
    public float Speed;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Objective.position, Time.deltaTime * Speed);

        if (Mathf.Approximately(Vector3.Distance(transform.position, Objective.position), 0f))
        {
            Destroy(gameObject);
        }
    }
}
