using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float damageValue = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask enemyLayer;

    public void PerformAttack()
    {
        // Detectamos enemigos en una esfera frente al jugador
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damageValue);
                Debug.Log($"Golpeaste a: {enemy.name}");
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