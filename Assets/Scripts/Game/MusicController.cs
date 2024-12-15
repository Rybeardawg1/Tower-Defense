using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController instance; // only one music controller exists

    public AudioClip background_music;
    private AudioSource audioSource;

    void Awake()
    {
        // Ensure only one instance of MusicController exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

    void Start()
    {
        // Add an AudioSource component if not already attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }


        audioSource.clip = background_music;
        
        audioSource.playOnAwake = true;
        audioSource.loop = true;

        audioSource.volume = 0.3f;

        // Play
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
