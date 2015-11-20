using UnityEngine;

public class Enemy : MonoBehaviour, IKillable
{
    public Transform Objective;
    public float Speed, Damage, Health;

    void Update()
    {
        if (Objective == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, Objective.position, Time.deltaTime * Speed);

        if (Mathf.Approximately(Vector3.Distance(transform.position, Objective.position), 0f))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D data)
    {
        if (data.tag == "Player")
        {
            Player p = data.gameObject.GetComponent<Player>();
            p.TakeDamage(Damage);
            TakeDamage(Health);
        }
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;

        if (Health <= 0f)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
