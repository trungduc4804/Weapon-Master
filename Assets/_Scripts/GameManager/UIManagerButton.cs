using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerBottom : MonoBehaviour
{
    [SerializeField] private GameObject settingUI;
    public void OnPressRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPressQuitButton()
    {
        Application.Quit();
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
