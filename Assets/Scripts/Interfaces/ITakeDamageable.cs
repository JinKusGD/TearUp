public interface ITakeDamageable
{
    public float MaxHp { get; }
    public float Hp { get; }

    public void TakeDamage(float damage);

    public void Heal(float value);
}
