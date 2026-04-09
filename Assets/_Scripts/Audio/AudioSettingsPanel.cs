using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsPanel : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Labels")]
    [SerializeField] private TMP_Text masterValueText;
    [SerializeField] private TMP_Text sfxValueText;
    [SerializeField] private TMP_Text musicValueText;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        BindSlider(masterSlider, AudioManager.Instance.MasterVolume, OnMasterVolumeChanged, masterValueText);
        BindSlider(sfxSlider, AudioManager.Instance.SfxVolume, OnSfxVolumeChanged, sfxValueText);
        BindSlider(musicSlider, AudioManager.Instance.MusicVolume, OnMusicVolumeChanged, musicValueText);
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.SetMasterVolume(value);
        UpdateLabel(masterValueText, value);
    }

    public void OnSfxVolumeChanged(float value)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.SetSfxVolume(value);
        UpdateLabel(sfxValueText, value);
        AudioManager.Instance.PlayButtonHover();
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.SetMusicVolume(value);
        UpdateLabel(musicValueText, value);
    }

    private static void BindSlider(Slider slider, float value, UnityEngine.Events.UnityAction<float> callback, TMP_Text label)
    {
        if (slider == null)
        {
            return;
        }

        slider.onValueChanged.RemoveListener(callback);
        slider.onValueChanged.AddListener(callback);
        slider.SetValueWithoutNotify(value);
        UpdateLabel(label, value);
    }

    private static void UpdateLabel(TMP_Text label, float value)
    {
        if (label == null)
        {
            return;
        }

        label.text = Mathf.RoundToInt(value * 100f).ToString();
    }
}
