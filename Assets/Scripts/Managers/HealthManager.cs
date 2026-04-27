using UnityEngine;
using System;

public class HealthManager : MonoBehaviour, IDamageable
{
    [Header("Configuracion de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();

        // Si es el Player, no lo destruimos (o rompemos todo), llamamos al UI
        if (gameObject.CompareTag("Player"))
        {
            if (UIManager.Instance != null) UIManager.Instance.ShowGameOver();
            else Debug.LogError("¡No hay UIManager en la escena para el Player!");
        }
        else
        {
            // Si es un enemigo, se va a la casa
            Destroy(gameObject);
        }
    }
}