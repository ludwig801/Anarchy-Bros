using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform MoveTo;
    public float Speed;

    void Update()
    {
        if (!Mathf.Approximately(Vector3.Distance(transform.position, MoveTo.position), 0f))
        {
            transform.position = Vector3.MoveTowards(transform.position, MoveTo.position, Time.deltaTime * Speed);
        }
    }
}