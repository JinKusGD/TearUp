using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpInfo
{
    public string Name;
    public float MaxHp;
    public float Hp;

    public EnemyHpInfo(string name, float maxHp, float hp)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = hp;
    }
}

public class EnemyInfoHud : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Slider _hpSlider; 

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyHpInfo>(OnHpChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyHpInfo>(OnHpChanged);
    }

    private void OnHpChanged(EnemyHpInfo enemyHpInfo)
    {
        _name.text = enemyHpInfo.Name;
        _hpSlider.value = (float)(enemyHpInfo.Hp / enemyHpInfo.MaxHp);
    }
}
