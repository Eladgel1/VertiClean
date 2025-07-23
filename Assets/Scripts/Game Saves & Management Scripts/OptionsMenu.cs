using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour {
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    [Header("Display")]
    [SerializeField] private Slider brightnessSlider;

    private void Start() {
        // Load stored values or default to full (1f)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (savedVolume < 0.0001f) savedVolume = 1f;

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);
        if (savedBrightness < 0.0001f) savedBrightness = 1f;

        // Set slider values without triggering UI events
        volumeSlider.SetValueWithoutNotify(savedVolume);
        brightnessSlider.SetValueWithoutNotify(savedBrightness);

        // Apply values immediately
        ApplyVolume(savedVolume);
        ApplyBrightness(savedBrightness);
    }

    // Triggered from UI: Volume slider (no parameter)
    public void OnVolumeChanged() {
        float value = volumeSlider.value;
        ApplyVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    // Triggered from UI: Brightness slider (no parameter)
    public void OnBrightnessChanged() {
        float value = brightnessSlider.value;
        ApplyBrightness(value);
        PlayerPrefs.SetFloat("Brightness", value);
    }

    // Applies volume in dB scale to AudioMixer
    private void ApplyVolume(float value) {
        if (value <= 0.0001f) {
            audioMixer.SetFloat("MasterVolume", -80f); // Mute
        }
        else {
            float volumeDb = Mathf.Log10(value) * 20f;
            audioMixer.SetFloat("MasterVolume", volumeDb);
        }

        Debug.Log("[ApplyVolume] Volume value = " + value);
    }

    // Applies brightness to ambient light
    private void ApplyBrightness(float value) {
        RenderSettings.ambientLight = new Color(value, value, value, 1f);
        Debug.Log("[ApplyBrightness] Brightness value = " + value);
    }
}
