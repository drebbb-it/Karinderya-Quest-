using UnityEngine;
using UnityEngine.Audio;

public class VolumeLoader : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;

    void Start()
    {
        Apply("MasterVolume");
        Apply("MusicVolume");
        Apply("SFXVolume");
    }

    private void Apply(string parameter)
    {
        float value = PlayerPrefs.GetFloat(parameter, 1f);
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        myMixer.SetFloat(parameter, dB);
    }
}