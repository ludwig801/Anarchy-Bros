using UnityEngine;

public interface IKillable
{
    void TakeDamage(float amount);

    void Die();

    void Reborn();

    bool IsAlive();

    Collider2D GetCollider();
}
