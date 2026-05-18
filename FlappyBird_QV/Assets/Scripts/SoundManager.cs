using UnityEngine;
using TMPro;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip wingClip;
    public AudioClip pointClip;
    public AudioClip hitClip;
    public AudioClip dieClip;
    public AudioClip swooshClip;

    [Header("Sound Button Text")]
    public TMP_Text soundButtonText;

    private bool soundOn = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        soundOn = true;
        UpdateSoundButtonText();
    }

    public void PlayWing()
    {
        PlaySound(wingClip);
    }

    public void PlayPoint()
    {
        PlaySound(pointClip);
    }

    public void PlayHit()
    {
        PlaySound(hitClip);
    }

    public void PlayDie()
    {
        PlaySound(dieClip);
    }

    public void PlaySwoosh()
    {
        PlaySound(swooshClip);
    }

    public void ToggleSound()
    {
        soundOn = !soundOn;
        UpdateSoundButtonText();
    }

    private void PlaySound(AudioClip clip)
    {
        if (!soundOn) return;
        if (audioSource == null) return;
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    private void UpdateSoundButtonText()
    {
        if (soundButtonText != null)
        {
            soundButtonText.text = soundOn ? "Âm thanh: Bật" : "Âm thanh: Tắt";
        }
    }
}