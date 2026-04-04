using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void UpdateValueText(float value)
    {
        valueText.text = value.ToString();
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGamePlay");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
