using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void OpenMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}