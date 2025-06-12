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

    [Header("Clips")]
    public AudioClip citySound;
    public AudioClip platformLiftSound;
    public AudioClip mopSound;
    public AudioClip spongeSound;
    public AudioClip spraySound;
    public AudioClip walkingSound;
    public AudioClip stageCompleteSound;

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
        isInGameScene = SceneManager.GetActiveScene().name == "GameScene";

        if (isInGameScene && citySound != null) {
            musicSource.clip = citySound;
            musicSource.loop = true;
            musicSource.volume = 0.5f;
            musicSource.Play();
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
