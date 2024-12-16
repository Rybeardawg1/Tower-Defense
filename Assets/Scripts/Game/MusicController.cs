using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController instance; // only one music controller exists

    public AudioClip background_music;
    private AudioSource audioSource;

    void Awake()
    {
        
    }

    void Start()
    {
        // Add an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = background_music;
        audioSource.playOnAwake = true;
        audioSource.loop = true;
        audioSource.volume = 0.3f;

        // Play the background music
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
