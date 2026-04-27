using UnityEngine;
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
            // Usamos una función anónima para pasar quién murió
            enemy.OnDeath += () => CheckWinCondition(enemy);
        }
    }

    private void CheckWinCondition(HealthManager deadEnemy)
    {
        if (enemiesInLevel.Contains(deadEnemy))
        {
            enemiesInLevel.Remove(deadEnemy);
        }

        // Si ya no quedan enemigos en la lista... ¡GANASTE!
        if (enemiesInLevel.Count <= 0)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        Debug.Log("¡Victoria!");
        // Llamamos a la pantalla de victoria del UI
        if (UIManager.Instance != null)
            UIManager.Instance.ShowWinScreen();
    }

    public void LoseGame()
    {
        Debug.Log("¡Derrota!");
        // En lugar de reiniciar al toque, mostramos la pantalla de Game Over
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver();
    }
}