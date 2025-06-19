using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource platformSource;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource mopSource;
    [SerializeField] private AudioSource spongeSource;
    [SerializeField] private AudioSource spraySource;
    [SerializeField] private AudioSource windSource;

    [Header("Clips")]
    public AudioClip citySound;
    public AudioClip platformLiftSound;
    public AudioClip mopSound;
    public AudioClip spongeSound;
    public AudioClip spraySound;
    public AudioClip walkingSound;
    public AudioClip stageCompleteSound;
    public AudioClip windSound;

    private bool isInGameScene;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        // Check if we are in the GameScene
        isInGameScene = SceneManager.GetActiveScene().name == "GameScene";

        if (isInGameScene && citySound != null) {
            musicSource.clip = citySound;
            musicSource.loop = true;
            musicSource.volume = 0.5f;
            musicSource.Play();
        }
    }

    private void Update() {
        if (!isInGameScene) return;

        UpdateCityVolumeByHeight();
        UpdateWindSoundByHeight();
    }

    // Adjust city ambient volume by platform Y position
    private void UpdateCityVolumeByHeight() {
        if (CleanerPlatform.Instance == null) return;

        float y = CleanerPlatform.Instance.transform.position.y;
        float t = Mathf.InverseLerp(8.8f, 58f, y);
        musicSource.volume = Mathf.Lerp(0.5f, 0f, t);
    }

    // Play/stop wind based on stage and height
    private void UpdateWindSoundByHeight() {
        if (CleanerPlatform.Instance == null || windSound == null) return;

        float y = CleanerPlatform.Instance.transform.position.y;
        int stage = StageManager.Instance?.GetCurrentStage() ?? 0;

        if (stage == 3 && y >= 37.8f) {
            PlayWind(0.5f);
        }
        else if (stage == 4 && y >= 50.5f) {
            PlayWind(0.8f);
        }
        else {
            StopWind();
        }
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f) {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayStageComplete() {
        PlayOneShot(stageCompleteSound);
    }

    public void SetPlatformMoving(bool moving) {
        HandleLoop(platformSource, platformLiftSound, moving, 0.5f);
    }

    public void SetWalking(bool moving) {
        HandleLoop(walkSource, walkingSound, moving, 0.2f);
    }

    public void SetSpraying(bool active) {
        HandleLoop(spraySource, spraySound, active, 1f);
    }

    public void SetSponging(bool active, bool isMop) {
        AudioSource source = isMop ? mopSource : spongeSource;
        AudioClip clip = isMop ? mopSound : spongeSound;
        HandleLoop(source, clip, active, isMop ? 0.7f : 1f);
    }

    private void PlayWind(float volume) {
        if (windSource.isPlaying) return;
        windSource.clip = windSound;
        windSource.volume = volume;
        windSource.loop = true;
        windSource.Play();
    }

    private void StopWind() {
        if (windSource.isPlaying)
            windSource.Stop();
    }

    private void HandleLoop(AudioSource source, AudioClip clip, bool play, float volume) {
        if (source == null || clip == null) return;

        if (play) {
            if (!source.isPlaying) {
                source.clip = clip;
                source.volume = volume;
                source.loop = true;
                source.Play();
            }
        }
        else {
            source.Stop();
        }
    }
}


