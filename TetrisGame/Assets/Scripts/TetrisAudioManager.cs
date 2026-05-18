using UnityEngine;

public class TetrisAudioManager : MonoBehaviour
{
    public static TetrisAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip rotateClip;
    [SerializeField] private AudioClip lockClip;
    [SerializeField] private AudioClip lineClearClip;
    [SerializeField] private AudioClip hardDropClip;
    [SerializeField] private AudioClip holdClip;
    [SerializeField] private AudioClip gameOverClip;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayMove()
    {
        PlaySfx(moveClip);
    }

    public void PlayRotate()
    {
        PlaySfx(rotateClip);
    }

    public void PlayLock()
    {
        PlaySfx(lockClip);
    }

    public void PlayLineClear()
    {
        PlaySfx(lineClearClip);
    }

    public void PlayHardDrop()
    {
        PlaySfx(hardDropClip);
    }

    public void PlayHold()
    {
        PlaySfx(holdClip);
    }

    public void PlayGameOver()
    {
        PlaySfx(gameOverClip);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}