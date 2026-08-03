using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Pastikan scene "Scenes/Play" sudah dimasukkan ke dalam Build Settings (File -> Build Settings)
        SceneManager.LoadScene("Scenes/Play");
    }

    public void ExitGame()
    {
        Debug.Log("Exit Button Clicked. Keluar dari game...");
        Application.Quit();
    }
}
