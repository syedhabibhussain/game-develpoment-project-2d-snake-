using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame() {
        SceneManager.LoadScene("SampleScene"); // Your game scene name
    }

    public void QuitGame() {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    // Optional: Show High Score from PlayerPrefs
    public void ShowHighScore() {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("High Score: " + highScore);
        // You can display it in a UI text here
    }
}