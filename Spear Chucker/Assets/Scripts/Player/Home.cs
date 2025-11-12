using UnityEngine;
using UnityEngine.SceneManagement;
public class Home : MonoBehaviour
{
    public bool isHome = false;

    public void Update()
    {
        // if (isHome == true)
        // {
        //     WinScreen();
        // }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Home"))
        {
            Debug.Log("Ah, Sizwe, you are home?");
            isHome = true;
            WinScreen();
        }
    }

    public void WinScreen()
    {
        SceneManager.LoadScene("GameWin");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
