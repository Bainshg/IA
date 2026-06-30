using UnityEngine;

public class EntityStunnedState : EntityState
{
    private EnemyAI _ai;
    private float _stunTimer;
    private float _maxStunTime = 3f;
    private Color _originalColor;
    private Renderer _rend;

    public EntityStunnedState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm)
    {
        _ai = ai;
        _rend = _ai.GetComponent<Renderer>();
    }

    public override void Awake()
    {
        _stunTimer = 0f;
        _ai.Agent.StopAgent();

        if (_rend)
        {
            _originalColor = _rend.material.color;
            _rend.material.color = Color.blue;
        }
    }

    public override void Execute()
    {
        _stunTimer += Time.deltaTime;

        _ai.Agent.StopAgent();

        _ai.transform.Rotate(Vector3.up * 500f * Time.deltaTime);

        if (_stunTimer >= _maxStunTime)
        {
            if (_rend) _rend.material.color = _originalColor;

            _ai.Agent.StopAgent(); 
            _sm.ChangeState(EntityStates.Patrol);
        }
    }
}