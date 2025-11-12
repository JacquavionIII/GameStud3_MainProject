using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        //SceneManager.LoadScene("Game Scene");
        SceneManager.LoadScene("OutdoorsScene");
    }

    public void ExplainGame()
    {
        //SceneManager.LoadScene("Explain Scene");
        SceneManager.LoadScene("Explain");
    }

    public void HomeMenu()
    {
        //SceneManager.LoadScene("Explain Scene");
        SceneManager.LoadScene("Start");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
