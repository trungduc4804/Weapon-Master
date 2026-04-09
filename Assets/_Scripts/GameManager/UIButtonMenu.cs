using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonMenu : MonoBehaviour
{
    private const float MuteThreshold = 0.001f;

    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject volumeUI;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Image volumeIconImage;
    [SerializeField] private Sprite volumeOnSprite;
    [SerializeField] private Sprite volumeMuteSprite;

    private float previousVolume = 100f;
    private bool isMuted;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (volumeSlider == null && volumeUI != null)
        {
            volumeSlider = volumeUI.GetComponent<Slider>();
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(UpdateValueText);
            volumeSlider.onValueChanged.AddListener(UpdateValueText);
        }
    }

    private void Start()
    {
        float initialSliderValue = volumeSlider != null ? volumeSlider.value : previousVolume;
        if (AudioManager.Instance != null)
        {
            initialSliderValue = AudioManager.Instance.MasterVolume * 100f;
        }

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(initialSliderValue);
        }

        UpdateValueText(initialSliderValue);
        if (initialSliderValue > 0f)
        {
            previousVolume = initialSliderValue;
        }
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(UpdateValueText);
        }
    }

    public void UpdateValueText(float value)
    {
        if (valueText != null)
        {
            valueText.text = Mathf.RoundToInt(value).ToString();
        }

        float normalizedVolume = Mathf.Clamp01(value / 100f);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(normalizedVolume);
        }
        else
        {
            AudioListener.volume = normalizedVolume;
        }

        isMuted = value <= MuteThreshold;
        UpdateVolumeIcon(value);
    }
    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
            AudioManager.Instance.PlayMusicState(AudioMusicState.Gameplay);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGamePlay");
    }
    public void ExitGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        Application.Quit();
    }

    public void MuteVolume()
    {
        float currentVolume = volumeSlider != null ? volumeSlider.value : (isMuted ? 0f : previousVolume);

        if (!isMuted && currentVolume > 0f)
        {
            previousVolume = currentVolume;
            SetVolume(0f);
            return;
        }

        SetVolume(Mathf.Max(1f, previousVolume));
    }

    private void SetVolume(float value)
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = Mathf.Clamp(value, volumeSlider.minValue, volumeSlider.maxValue);
            return;
        }

        UpdateValueText(value);
    }

    private void UpdateVolumeIcon(float value)
    {
        if (volumeIconImage == null)
        {
            return;
        }

        bool muted = value <= MuteThreshold;
        Sprite targetSprite = muted ? volumeMuteSprite : volumeOnSprite;
        if (targetSprite != null)
        {
            volumeIconImage.sprite = targetSprite;
        }
    }
}
