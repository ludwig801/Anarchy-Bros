using UnityEngine;

public interface IKillable
{
    void TakeDamage(float amount);

    void Kill();

    void Reborn();

    bool IsAlive();

    Collider2D GetCollider();
}
