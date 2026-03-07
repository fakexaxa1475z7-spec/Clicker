using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClip;
    [SerializeField] AudioSource audioSource;

    public void PlaySound(int index)
    {
        if (index >= 0 && index < audioClip.Length)
        {
            audioSource.PlayOneShot(audioClip[index]);
        }
    }
    public void PlayRandom()
    {
        int randomIndex = Random.Range(0, audioClip.Length);
        audioSource.PlayOneShot(audioClip[randomIndex]);
    }
}
