using UnityEngine;

public class MainMenuBGM : MonoBehaviour
{
    public static MainMenuBGM Instance;
    private AudioSource bgmSource;

    void Awake()
    {
        // Ensure only one BGM plays across scenes
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();
    }

    public void SetVolume(float volume)
    {
        if (bgmSource != null)
            bgmSource.volume = volume;
    }
}
