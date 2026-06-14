using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;      // Gameplay BGM
    public AudioSource menuBgmSource;  // Main Menu & Pause BGM
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip gameplayBgm;
    public AudioClip menuBgm;
    public AudioClip eatClip;
    public AudioClip gameOverClip;
    public AudioClip highScoreClip;
    public AudioClip speedUpClip;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        PlayMenuBGM(); // Default to menu BGM
    }

    public void PlayMenuBGM() {
        StopAllBGM();
        if (menuBgm != null) {
            menuBgmSource.clip = menuBgm;
            menuBgmSource.loop = true;
            menuBgmSource.Play();
        }
    }

    public void PlayGameplayBGM() {
        StopAllBGM();
        if (gameplayBgm != null) {
            bgmSource.clip = gameplayBgm;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    private void StopAllBGM() {
        bgmSource.Stop();
        menuBgmSource.Stop();
    }

    public void PlayEat() => sfxSource.PlayOneShot(eatClip);
    public void PlayGameOver() => sfxSource.PlayOneShot(gameOverClip);
    public void PlayHighScore() => sfxSource.PlayOneShot(highScoreClip);
    public void PlaySpeedUp() => sfxSource.PlayOneShot(speedUpClip);
}