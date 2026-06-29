using System.Collections;
using UnityEngine;

public class EnemyController : EntityBase, ITakeDamageable
{
    [SerializeField] private Rigidbody _rigidbody;
   
    private EnemyHpInfo _hpInfo;
    private bool _isDead;
   
    public float Hp { get; private set; } = 100;

    public float MaxHp { get; private set; } = 100;

    private void Awake()
    {
        _hpInfo = new EnemyHpInfo(name, 0, 0);
    }

    private void OnEnable()
    {
        _hpInfo.MaxHp = MaxHp;
        _hpInfo.Hp = Hp;
        _isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) { return; }

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

    public void Heal(float value)
    {
        if (_isDead) { return; }

        if (Hp >= MaxHp)
        {
            return;
        }

        float targetHp = Hp + value;
        Hp = Mathf.Clamp(targetHp, 0, MaxHp);
        AudioManager.Instance.PlaySFX(ClipType.Heal);

        HpHudChange();
    }

    private void Dead()
    {
        _isDead = true;
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
        _hpInfo.Hp = Hp;
        EventBus.Invoke(_hpInfo);
    }
}
