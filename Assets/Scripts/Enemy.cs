using UnityEngine;

public class Enemy : EntityBase, ITakeDamageable
{
    public float Hp { get; private set; } = 100;

    public void TakeDamage(float damage)
    {
        Hp -= damage;
        Debug.Log("사운드 재생");
        Debug.Log(Hp);
    }
}
