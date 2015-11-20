using UnityEngine;

public class Player : MonoBehaviour, IKillable
{
    public Transform MoveTo;
    public float Speed, Health;

    public bool IsAlive
    {
        get
        {
            return Health > 0f;
        }
    }

    void Update()
    {
        if (!Mathf.Approximately(Vector3.Distance(transform.position, MoveTo.position), 0f))
        {
            transform.position = Vector3.MoveTowards(transform.position, MoveTo.position, Time.deltaTime * Speed);
        }
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;

        if (!IsAlive)
        {
            Kill();
        }       
    }

    public void Kill()
    {
        Debug.Log("The player died");
    }
}