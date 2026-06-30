using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private HealthManager playerHealth;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath += LoseGame;
        }
    }

    public int EnemiesKilled { get; private set; }
    public void RegisterEnemyKill() => EnemiesKilled++;

    public void WinGame()
    {
        Debug.Log("Victoria!");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowWinScreen();
        }
    }

    public void LoseGame()
    {
        Debug.Log("Derrota!");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
    }
}