using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Mixer References")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Keys")]
    [SerializeField] private string masterVolumeKey = "MasterVolume";
    [SerializeField] private string musicVolumeKey = "MusicVolume";
    [SerializeField] private string sfxVolumeKey = "SFXVolume";
    
    void Start()
    {
        InitializeVolumes();
    }

    private void InitializeVolumes()
    {
        SetVolume(masterVolumeKey, PlayerPrefs.GetFloat(masterVolumeKey, 1f));
        SetVolume(musicVolumeKey, PlayerPrefs.GetFloat(musicVolumeKey, 1f));
        SetVolume(sfxVolumeKey, PlayerPrefs.GetFloat(sfxVolumeKey, 1f));
    }

    // Public methods to be called by UI elements
    public void SetMasterVolume(float volume) => SetVolume(masterVolumeKey, volume);
    public void SetMusicVolume(float volume) => SetVolume(musicVolumeKey, volume);
    public void SetSFXVolume(float volume) => SetVolume(sfxVolumeKey, volume);

    private void SetVolume(string key, float linearVolume)
    {
        // Convert linear 0-1 to logarithmic dB
        float dB = Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(key, dB);
        Debug.Log($"Setting audio key {key} to {dB} dB", this);
        PlayerPrefs.SetFloat(key, linearVolume);
    }

    public void ResetToDefaults()
    {
        SetMasterVolume(1f);
        SetMusicVolume(1f);
        SetSFXVolume(1f);
    }
}