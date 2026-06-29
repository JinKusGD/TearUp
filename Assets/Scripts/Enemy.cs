using System.Collections;
using UnityEngine;

public class Enemy : EntityBase, ITakeDamageable
{
    [SerializeField] private Rigidbody _rigidbody;

    public float Hp { get; private set; } = 100;

    public float MaxHp { get; private set; } = 100;

    private EnemyHpInfo hpInfo;
    private bool isDead;

    private void Awake()
    {
        hpInfo = new EnemyHpInfo(name, MaxHp, Hp);
    }

    private void OnEnable()
    {
        isDead = false;
    }

    public void Heal(float value)
    {
        if (isDead) { return; }

        if (Hp >= MaxHp)
        {
            return;
        }

        float targetHp = Hp + value;
        Hp = Mathf.Clamp(targetHp, 0, MaxHp);
        AudioManager.Instance.PlaySFX(ClipType.Heal);

        HpHudChange();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) { return; }

        if (Hp <= 0)
        {
            return;
        }

        float targetHp = Hp - damage;
        Hp = Mathf.Clamp(targetHp, 0, MaxHp);
        HpHudChange();

        if (Hp <= 0)
        {
            Dead();
            return;
        }

        AudioManager.Instance.PlaySFX(ClipType.Damage);
    }

    private void Dead()
    {
        isDead = true;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;

        AudioManager.Instance.PlaySFX(ClipType.Dead);

        StartCoroutine(DelayDead());
    }

    private IEnumerator DelayDead()
    {
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
    }

    private void HpHudChange()
    {
        hpInfo.Hp = Hp;
        EventBus.Invoke(hpInfo);
    }
}
