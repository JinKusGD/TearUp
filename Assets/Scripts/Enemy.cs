using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _hp = 100;

    public void OnTakeDamage(float damage)
    {
        _hp -= damage;
        Debug.Log("사운드 재생");
        Debug.Log(_hp);
    }
}
