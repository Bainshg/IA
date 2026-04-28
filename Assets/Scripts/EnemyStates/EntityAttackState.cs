using UnityEngine;

public class EntityAttackState : EntityState
{
    private EnemyAI _ai;

    public EntityAttackState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm) => _ai = ai;

    public override void Awake()
    {
        // Triggereamos la pantalla de Game Over
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("¡Falta el UIManager en la escena!");
        }
    }

    public override void Execute() { } 
}