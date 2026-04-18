using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManagerBottom : MonoBehaviour
{
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject miniMap;

    private void OnDisable()
    {
        // Ensure pause state does not leak across scenes.
        Time.timeScale = 1f;
    }

    public void OnPressRestartButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPressQuitButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
            AudioManager.Instance.PlayMusicState(AudioMusicState.MainMenu);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void OnPressResumeButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        settingUI.SetActive(false);
        inventoryUI.SetActive(true);
        Time.timeScale = 1;
    }
    public void OnPressSettingButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        settingUI.SetActive(true);
        inventoryUI.SetActive(false);
        Time.timeScale = 0;
    }   
    public void OnPressMiniMapButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        miniMap.SetActive(true);

    }
    public void OnPressCloseMiniMapButton()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        miniMap.SetActive(false);
    }
}
