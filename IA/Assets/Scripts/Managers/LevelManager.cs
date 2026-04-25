using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private List<HealthManager> enemiesInLevel;
    [SerializeField] private HealthManager playerHealth;

    void Awake() { Instance = this; }

    void Start()
    {
        // Suscribirse a la muerte del player
        if (playerHealth != null) playerHealth.OnDeath += LoseGame;

        // Suscribirse a la muerte de cada enemigo
        foreach (var enemy in enemiesInLevel)
        {
            enemy.OnDeath += () => CheckWinCondition(enemy);
        }
    }

    private void CheckWinCondition(HealthManager deadEnemy)
    {
        enemiesInLevel.Remove(deadEnemy);
        if (enemiesInLevel.Count <= 0)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        Debug.Log("¡Victoria! Todos los enemigos eliminados.");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoseGame()
    {
        Debug.Log("¡Derrota! El player ha muerto.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reinicia
    }
}