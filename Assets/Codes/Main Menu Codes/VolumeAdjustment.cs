using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
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
        // Load saved values (default 1 = full volume)
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();

        SetupButtonSounds();
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

    // ---------- Button sounds ----------

    private void SetupButtonSounds()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            Button b = button; // local copy for the lambdas

            // Click
            b.onClick.AddListener(() => PlaySFX(clickClip));

            // Hover (pointer enters the button)
            EventTrigger trigger = b.GetComponent<EventTrigger>();
            if (trigger == null) trigger = b.gameObject.AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener(_ =>
            {
                if (b.interactable) PlaySFX(hoverClip);
            });
            trigger.triggers.Add(entry);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}