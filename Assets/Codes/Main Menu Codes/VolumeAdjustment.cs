using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeAdjustment : MonoBehaviour
{
    [Header("Mixer & Sliders")]
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Button SFX")]
    [SerializeField] private AudioSource sfxSource;   // Output = SFX mixer group
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;

    void Start()
    {
        // Default volume is 50%; saved player settings take precedence.
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();

        ButtonSFXUtility.RegisterButtons(sfxSource, clickClip, hoverClip);
    }

    public void SetMasterVolume() => Apply("MasterVolume", masterSlider.value);
    public void SetMusicVolume() => Apply("MusicVolume", musicSlider.value);
    public void SetSFXVolume() => Apply("SFXVolume", sfxSlider.value);

    private void Apply(string parameter, float value)
    {
        // Sliders are linear (0.0001 to 1), mixers use decibels (logarithmic)
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        myMixer.SetFloat(parameter, dB);

        PlayerPrefs.SetFloat(parameter, value);
    }

}

public static class ButtonSFXUtility
{
    private static AudioSource registeredSfxSource;
    private static AudioClip registeredClickClip;
    private static AudioClip registeredHoverClip;
    private static readonly HashSet<Button> registeredButtons = new HashSet<Button>();

    static ButtonSFXUtility()
    {
        SceneManager.sceneLoaded += (_, __) => registeredButtons.Clear();
    }

    public static void RegisterButtons(AudioSource sfxSource, AudioClip clickClip, AudioClip hoverClip)
    {
        if (sfxSource == null)
        {
            Debug.LogError("ButtonSFXUtility requires an AudioSource.");
            return;
        }

        registeredSfxSource = sfxSource;
        registeredClickClip = clickClip;
        registeredHoverClip = hoverClip;

        Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (Button button in buttons)
        {
            RegisterButton(button);
        }
    }

    public static void RegisterButton(Button target)
    {
        if (target == null) return;
        if (registeredSfxSource == null || (registeredClickClip == null && registeredHoverClip == null)) return;
        if (!registeredButtons.Add(target)) return;

        if (registeredClickClip != null)
        {
            target.onClick.AddListener(() =>
            {
                if (registeredSfxSource != null)
                {
                    registeredSfxSource.PlayOneShot(registeredClickClip, 2f);
                }
            });
        }

        if (registeredHoverClip != null)
        {
            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.gameObject.AddComponent<EventTrigger>();
            }

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener(_ =>
            {
                if (target.interactable && registeredSfxSource != null)
                {
                    registeredSfxSource.PlayOneShot(registeredHoverClip);
                }
            });
            trigger.triggers.Add(entry);
        }
    }

    public static void PlayOneShot(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("ButtonSFXUtility cannot play a null audio clip.");
            return;
        }

        if (registeredSfxSource == null)
        {
            Debug.LogError("ButtonSFXUtility requires a registered AudioSource to play SFX.");
            return;
        }

        registeredSfxSource.PlayOneShot(clip);
    }

    public static void PlayOneShot(AudioClip clip, float pitch)
    {
        if (clip == null)
        {
            Debug.LogError("ButtonSFXUtility cannot play a null audio clip.");
            return;
        }

        if (registeredSfxSource == null)
        {
            Debug.LogError("ButtonSFXUtility requires a registered AudioSource to play SFX.");
            return;
        }

        if (pitch <= 0f || float.IsNaN(pitch) || float.IsInfinity(pitch))
        {
            Debug.LogError("ButtonSFXUtility requires a finite, positive pitch.");
            return;
        }

        GameObject playbackObject = new GameObject($"OneShotSFX_{clip.name}");
        AudioSource playbackSource = playbackObject.AddComponent<AudioSource>();
        playbackSource.outputAudioMixerGroup = registeredSfxSource.outputAudioMixerGroup;
        playbackSource.spatialBlend = registeredSfxSource.spatialBlend;
        playbackSource.volume = registeredSfxSource.volume;
        playbackSource.pitch = pitch;
        playbackSource.PlayOneShot(clip);

        UnityEngine.Object.Destroy(playbackObject, clip.length / pitch);
    }

    public static void PlayBriefly(AudioClip clip, float duration)
    {
        if (clip == null)
        {
            Debug.LogError("ButtonSFXUtility cannot play a null audio clip.");
            return;
        }

        if (registeredSfxSource == null)
        {
            Debug.LogError("ButtonSFXUtility requires a registered AudioSource to play SFX.");
            return;
        }

        if (duration <= 0f || float.IsNaN(duration) || float.IsInfinity(duration))
        {
            Debug.LogError("ButtonSFXUtility requires a finite, positive playback duration.");
            return;
        }

        float playbackDuration = Mathf.Min(duration, clip.length);
        GameObject playbackObject = new GameObject($"BriefSFX_{clip.name}");
        AudioSource playbackSource = playbackObject.AddComponent<AudioSource>();
        playbackSource.outputAudioMixerGroup = registeredSfxSource.outputAudioMixerGroup;
        playbackSource.spatialBlend = registeredSfxSource.spatialBlend;
        playbackSource.volume = registeredSfxSource.volume;
        playbackSource.clip = clip;
        playbackSource.Play();
        playbackSource.SetScheduledEndTime(AudioSettings.dspTime + playbackDuration);

        UnityEngine.Object.Destroy(playbackObject, playbackDuration + 0.05f);
    }
}