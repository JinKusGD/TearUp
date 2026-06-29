using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpInfo
{
    public float MaxHp;
    public float Hp;

    public PlayerHpInfo(float maxHp, float hp)
    {
        MaxHp = maxHp;
        Hp = hp;
    }
}

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider; 

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerHpInfo>(OnHpChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHpInfo>(OnHpChanged);
    }

    private void OnHpChanged(PlayerHpInfo playerHpInfo)
    {
        _hpSlider.value = (float)(playerHpInfo.Hp / playerHpInfo.MaxHp);
    }
}
