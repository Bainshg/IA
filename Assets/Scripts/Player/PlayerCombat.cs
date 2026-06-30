using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float damageValue = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask enemyLayer;
    
    // area de ataque
    [SerializeField] private Transform rangeIndicator;
    public void PerformAttack()
    {
        Vector3 attackCenter = transform.position;
        Collider[] hitEnemies = Physics.OverlapSphere(attackCenter, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable victim))
            {
                victim.TakeDamage(damageValue);
                Debug.Log($"<color=green>Impacto en {enemy.name}!</color>");
            }

            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null && enemyAI.StateMachine != null)
            {
                enemyAI.StateMachine.ChangeState(EntityStates.Stunned);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }

    public void SetAttackRange(float newRange) // setter para la FSM
    {
        attackRange = newRange;

        // cambiamos la escala del disco que muestra el area de ataque
        if (rangeIndicator != null)
        {  
            float newScale = (newRange * 2f) + 0.5f;
            rangeIndicator.localScale = new Vector3(newScale, rangeIndicator.localScale.y, newScale);
        }
    }
}