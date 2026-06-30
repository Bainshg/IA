using UnityEngine;

public class EntityIdleState : EntityState
{
    private enum IdleBehaviour { Quieto, Vigilar, Alerta }

    private EnemyAI _ai;
    private float _timer;
    private Renderer _rend;
    private IdleBehaviour _choice;
    private float _rotationSpeed = 90f; // Velocidad de rotación en grados por segundo
    public EntityIdleState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm)
    {
        _ai = ai;
        _rend = _ai.GetComponent<Renderer>();
    }

    public override void Awake()
    {
        _ai.Agent.StopAgent();

        _timer = 1.0f;
        int kills = LevelManager.Instance != null ? LevelManager.Instance.EnemiesKilled : 0;
        _choice = RouletteWheel.Select(
            (IdleBehaviour.Quieto,  30f),
            (IdleBehaviour.Vigilar, 40f + kills),
            (IdleBehaviour.Alerta,  15f + kills * 10)
        );

        switch (_choice)
        {
            case IdleBehaviour.Quieto: // se queda 3 segundos totales en idle
                if (_rend) _rend.material.color = Color.cyan;
                _timer += 2.0f;
                break;
            case IdleBehaviour.Vigilar: // Rota para ver su panorama, está 2 segundos totales
                if (_rend) _rend.material.color = Color.gray;
                _timer += 1.0f;
                break;
            case IdleBehaviour.Alerta: // Cambia a amarillo por un momento, solo 1 segundo en Idle
                if (_rend) _rend.material.color = Color.yellow;
                break;
        }
    }

    public override void Execute()
    {   
        //rotacion
        if (_choice == IdleBehaviour.Vigilar)
        {
            _ai.transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0, Space.World);
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _ai.ResetIterations();
            _sm.ChangeState(EntityStates.Patrol);
        }
    }
}