using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource footstepsSource;
    public AudioSource grappleSource;
    public AudioSource zipSource;
    public AudioSource jumpSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float footstepVolume = 1f;
    [Range(0f, 1f)] public float grappleVolume = 1f;
    [Range(0f, 1f)] public float zipVolume = 1f;
    [Range(0f, 1f)] public float jumpVolume = 1f;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ValidateAudioSources();
        ApplyVolumes();
    }

    private void ValidateAudioSources()
    {
        if (!footstepsSource) Debug.LogWarning("Footsteps AudioSource is not assigned!");
        if (!grappleSource) Debug.LogWarning("Grapple AudioSource is not assigned!");
        if (!zipSource) Debug.LogWarning("Zip AudioSource is not assigned!");
        if (!jumpSource) Debug.LogWarning("Jump AudioSource is not assigned!");
    }

    private void ApplyVolumes()
    {
        if (footstepsSource) footstepsSource.volume = footstepVolume;
        if (grappleSource) grappleSource.volume = grappleVolume;
        if (zipSource) zipSource.volume = zipVolume;
        if (jumpSource) jumpSource.volume = jumpVolume;
    }

    public void PlayFootsteps()
    {
        if (footstepsSource && !footstepsSource.isPlaying)
            footstepsSource.Play();
    }

    public void StopFootsteps()
    {
        if (footstepsSource && footstepsSource.isPlaying)
            footstepsSource.Stop();
    }

    public void PlayGrapple()
    {
        if (grappleSource && grappleSource.clip != null)
            grappleSource.PlayOneShot(grappleSource.clip);
        else
            Debug.LogWarning("Grapple sound missing or clip not assigned.");
    }

    public void PlayZip()
    {
        if (zipSource && zipSource.clip != null)
            zipSource.PlayOneShot(zipSource.clip);
        else
            Debug.LogWarning("Zip sound missing or clip not assigned.");
    }

    public void PlayJump()
    {
        if (jumpSource && jumpSource.clip != null)
            jumpSource.PlayOneShot(jumpSource.clip);
        else
            Debug.LogWarning("Jump sound missing or clip not assigned.");
    }
}
