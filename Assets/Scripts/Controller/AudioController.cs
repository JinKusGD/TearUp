using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceSFX;

    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }

        _audioSourceSFX.PlayOneShot(audioClip);
    }
}