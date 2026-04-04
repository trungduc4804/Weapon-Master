using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManagerBottom : MonoBehaviour
{
    [SerializeField] private GameObject settingUI;

    private void OnDisable()
    {
        // Ensure pause state does not leak across scenes.
        Time.timeScale = 1f;
    }

    public void OnPressRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPressQuitButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void OnPressResumeButton()
    {
        settingUI.SetActive(false);
        Time.timeScale = 1;
    }
    public void OnPressSettingButton()
    {
        settingUI.SetActive(true);
        Time.timeScale = 0;
    }   
}
