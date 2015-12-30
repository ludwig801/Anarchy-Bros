public interface IKillable
{
    void TakeDamage(float amount);

    void Kill();

    bool IsAlive();
}
