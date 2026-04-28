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
        _timer = 1.0f;
        float[] weights = { 40f, 25f, 10f }; // 40 Quieto, 25 Vigilar, 10 Alerta
        int choice = RouletteWheel.Select(weights);

        switch (choice)
        {
            case 0: // QUIETO: se queda 3 segundos totales en idle
                if (_rend) _rend.material.color = Color.white;
                _timer += 2.0f; 
                break;
            case 1: // VIGILAR: Rota un poco para mirar otro lugar, 2 segundos tots
                _ai.transform.Rotate(0, 45, 0);
                if (_rend) _rend.material.color = Color.gray;
                _timer += 1.0f; 
                break;
            case 2: // ALERTA: Cambia a amarillo por un momento, solo 1 segundo en Idle
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