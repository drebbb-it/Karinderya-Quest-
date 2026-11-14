using UnityEngine;
using UnityEngine.Audio;

public class VolumeLoader : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioClip _buttonClickClip;
    [SerializeField] private AudioClip _buttonHoverClip;

    void Start()
    {
        Apply("MasterVolume");
        Apply("MusicVolume");
        Apply("SFXVolume");
        SetupButtonSounds();
    }

    private void Apply(string parameter)
    {
        float value = PlayerPrefs.GetFloat(parameter, 1f);
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        myMixer.SetFloat(parameter, dB);
    }

    private void SetupButtonSounds()
    {
        if (_buttonClickClip == null && _buttonHoverClip == null)
        {
            return;
        }
        if (_sfxMixerGroup == null)
        {
            Debug.LogError("VolumeLoader: assign the SFX mixer group to enable button sounds.", this);
            return;
        }

        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = _sfxMixerGroup;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
        ButtonSFXUtility.RegisterButtons(sfxSource, _buttonClickClip, _buttonHoverClip);
    }
}