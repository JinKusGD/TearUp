using UnityEngine;

public enum ClipType
{
    Attack,
    Damage,
    Heal,
    Dead
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioController _audioController;
    
    [SerializeField] private AudioClip _DamageClip;
    [SerializeField] private AudioClip _HealClip;
    [SerializeField] private AudioClip _AttackClip;
    [SerializeField] private AudioClip _DeadClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 AudioManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void PlaySFX(ClipType clipType)
    {
        AudioClip audioClip = GetClipByType(clipType);
        _audioController.PlaySFX(audioClip);
    }

    private AudioClip GetClipByType(ClipType clipType)
    {
        AudioClip clip = null;

        switch (clipType)
        {
            case ClipType.Attack:
                clip = _AttackClip;
                break;
            case ClipType.Damage:
                clip = _DamageClip;
                break;
            case ClipType.Heal:
                clip = _HealClip;
                break;
            case ClipType.Dead:
                clip = _DeadClip;
                break;
        }

        return clip;
    }
}