using UnityEngine;

// Small always-available sound hub - one AudioSource for one-shot SFX
// (so overlapping hits don't cut each other off) and a second, separate
// AudioSource for the looping background music. Other scripts just call
// AudioManager.Instance?.PlayHit() etc. and don't need to know or care
// whether a clip is actually assigned.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip hitClip;
    public AudioClip throwClip;
    public AudioClip enemyDeathClip;
    public AudioClip levelUpClip;
    public AudioClip gameOverClip;
    public AudioClip clickClip;
    public AudioClip pickupClip;
    public AudioClip musicClip;

    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private const string SfxVolumeKey = "sfx_volume";
    private const string MusicVolumeKey = "music_volume";

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        // Keep exactly one AudioManager alive - BuildAudioSystem() is
        // idempotent and may find/re-wire an existing one, but this guards
        // against ending up with two if something else creates one too.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Pick up a volume the player set in a previous session, if any.
        if (PlayerPrefs.HasKey(SfxVolumeKey)) sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey);
        if (PlayerPrefs.HasKey(MusicVolumeKey)) musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    void PlayOneShot(AudioClip clip)
    {
        if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayHit() { PlayOneShot(hitClip); }
    public void PlayThrow() { PlayOneShot(throwClip); }
    public void PlayEnemyDeath() { PlayOneShot(enemyDeathClip); }
    public void PlayLevelUp() { PlayOneShot(levelUpClip); }
    public void PlayGameOver() { PlayOneShot(gameOverClip); }
    public void PlayClick() { PlayOneShot(clickClip); }
    public void PlayPickup() { PlayOneShot(pickupClip); }

    // Called from the settings sliders. Applied immediately and saved so
    // it persists across play sessions/builds.
    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        if (musicSource != null) musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
    }
}
