using UnityEngine;

public enum ZoneType
{
    Danger,
    Heal
}

public class DangerZone : MonoBehaviour
{
    [SerializeField] private ZoneType _zoneType;
    [SerializeField] private float _coolTime = 1.0f;
    [SerializeField] private float _cooldown = 0.0f;
    [SerializeField] private float _value = 5.0f;

    private ITakeDamageable _damageable;

    private void Update()
    {
        if (_cooldown >= 0)
        {
            _cooldown -= Time.deltaTime;
            return;
        }

        if (_damageable != null) 
        {
            ZoneEffect();
        }
    }

    private void ZoneEffect()
    {
        switch (_zoneType)
        {
            case ZoneType.Danger:
                _damageable.TakeDamage(_value);
                break;
            case ZoneType.Heal:
                _damageable.Heal(_value);
                break;
        }

        _cooldown = _coolTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<ITakeDamageable>(out var damageable)) { return; }

        _damageable = damageable;
        _cooldown = _coolTime;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<ITakeDamageable>(out var damageable)) { return; }

        if (_damageable != damageable)
        {
            return;
        }

        _damageable = null;
    }
}
