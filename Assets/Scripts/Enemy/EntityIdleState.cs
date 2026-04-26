using UnityEngine;

public class EntityIdleState : EntityState
{
    private EnemyAI _ai;
    private float _timer;

    public EntityIdleState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm) => _ai = ai;

    public override void Awake()
    {
        _timer = 2f;
        float[] weights = { 70f, 20f, 10f };
        Debug.Log($"Ruleta: {RouletteWheel.Select(weights)}");
    }

    public override void Execute()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _ai.ResetIterations();
            _sm.ChangeState(EntityStates.Patrol);
        }
    }
}