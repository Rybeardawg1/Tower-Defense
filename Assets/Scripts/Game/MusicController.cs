using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController instance; // only one music controller exists

    public AudioClip backgroundMusic;
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

        // Assign the music and configure the AudioSource
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;        // Enable looping
        audioSource.playOnAwake = true; // Automatically play on start
        audioSource.volume = 0.3f;      // Set initial volume (adjust as needed)

        // Play the music
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
