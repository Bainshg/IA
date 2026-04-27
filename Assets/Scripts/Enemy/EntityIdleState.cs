using UnityEngine;

public class EntityIdleState : EntityState
{
    private EnemyAI _ai;
    private float _timer;
    private Renderer _rend;

    public EntityIdleState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm)
    {
        _ai = ai;
        _rend = _ai.GetComponent<Renderer>();
    }

    public override void Awake()
    {
        _timer = 2.5f;
        float[] weights = { 60f, 30f, 10f }; // 60% Quieto, 30% Vigilar, 10% Alerta
        int choice = RouletteWheel.Select(weights);

        switch (choice)
        {
            case 0: // IDLE NORMAL
                if (_rend) _rend.material.color = Color.white;
                break;
            case 1: // VIGILAR (Gira un poco para mirar)
                _ai.transform.Rotate(0, 45, 0);
                if (_rend) _rend.material.color = Color.gray;
                break;
            case 2: // ALERTA (Cambia a amarillo por un momento)
                if (_rend) _rend.material.color = Color.yellow;
                break;
        }
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