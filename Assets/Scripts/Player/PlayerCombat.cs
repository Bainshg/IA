using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float damageValue = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask enemyLayer;

    public void PerformAttack()
    {
        // Corregimos el centro del Overlap para que sea un poco más adelante
        Vector3 attackCenter = transform.position + transform.forward * 1.5f;
        Collider[] hitEnemies = Physics.OverlapSphere(attackCenter, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Buscamos la interfaz, no importa el nombre del script de vida
            if (enemy.TryGetComponent(out IDamageable victim))
            {
                victim.TakeDamage(damageValue);
                Debug.Log($"<color=green>Impacto en {enemy.name}!</color>");
            }
        }
    }

    // Para ver el rango en el Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}