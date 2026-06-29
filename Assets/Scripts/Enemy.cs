using UnityEngine;

public class Enemy : EntityBase, ITakeDamageable
{
    public float Hp { get; private set; } = 100;

    public float MaxHp { get; private set; } = 100;

    private EnemyHpInfo hpInfo;

    private void Awake()
    {
        hpInfo = new EnemyHpInfo(name, MaxHp, Hp);
    }

    public void Heal(float value)
    {
        float targetHp = Hp + value;
        Hp = Mathf.Clamp(targetHp, 0, MaxHp);
        Debug.Log("사운드 재생");

        HpHudChange();
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
        Debug.Log("사운드 재생");

        HpHudChange();
    }

    private void HpHudChange()
    {
        hpInfo.Hp = Hp;
        EventBus.Invoke(hpInfo);
    }
}
