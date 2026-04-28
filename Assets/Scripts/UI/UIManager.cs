using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Paneles")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel; // Arrastr� el panel de "Ganaste" ac�

    void Awake() => Instance = this;

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f; // Pausamos para que no te sigan pegando si ganaste
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}