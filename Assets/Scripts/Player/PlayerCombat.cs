using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float damageValue = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask enemyLayer;

    
    // area de ataque
    [SerializeField] private Transform rangeIndicator;
    private bool alreadyStun = false;
    public void PerformAttack()
    {
        Vector3 attackCenter = transform.position;
        Collider[] hitEnemies = Physics.OverlapSphere(attackCenter, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamageable victim))
            {
                victim.TakeDamage(damageValue);
            }

            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null && enemyAI.StateMachine != null)
            {
                if (enemyAI.StateMachine.CurrentState is EntityStunnedState) continue;

                float chanceStun = 0f;

                if (!alreadyStun)
                {
                    chanceStun = 100f;

                    alreadyStun = true;

                    Debug.LogWarning("<color=cyan>[TEST RUN] ¡Primer golpe absoluto de la partida! Forzando 100% de Stun.</color>");
                }
                else
                {
                    enemyAI.extraStunChance += 5f;
                    chanceStun = 5f + enemyAI.extraStunChance;
                }

                float chanceNormal = Mathf.Max(0f, 100f - chanceStun);

                bool debeStunear = RouletteWheel.Select(
                    (true, chanceStun),
                    (false, chanceNormal)
                );

                Debug.Log($"[RULETA COMBATE] {enemy.name} -> Chance Stun: {chanceStun}%. ¿Salió Stun?: {debeStunear}");

                if (debeStunear)
                {
                    enemyAI.StateMachine.ChangeState(EntityStates.Stunned);
                }
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