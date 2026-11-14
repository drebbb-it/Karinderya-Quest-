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
    public static void RegisterButtons(AudioSource sfxSource, AudioClip clickClip, AudioClip hoverClip)
    {
        if (sfxSource == null)
        {
            Debug.LogError("ButtonSFXUtility requires an AudioSource.");
            return;
        }

        Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (Button button in buttons)
        {
            Button target = button;

            if (clickClip != null)
            {
                target.onClick.AddListener(() => sfxSource.PlayOneShot(clickClip));
            }

            if (hoverClip != null)
            {
                EventTrigger trigger = target.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = target.gameObject.AddComponent<EventTrigger>();
                }

                var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                entry.callback.AddListener(_ =>
                {
                    if (target.interactable)
                    {
                        sfxSource.PlayOneShot(hoverClip);
                    }
                });
                trigger.triggers.Add(entry);
            }
        }
    }
}