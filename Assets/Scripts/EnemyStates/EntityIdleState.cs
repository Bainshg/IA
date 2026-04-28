using UnityEngine;

public class EntityIdleState : EntityState
{
    private EnemyAI _ai;
    private float _timer;
    private Renderer _rend;
    private int _choice; 
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
        float[] weights = { 30f, 40f, 15f }; // Quieto, Vigilar, Alerta
        _choice = RouletteWheel.Select(weights);

        switch (_choice)
        {
            case 0: // QUIETO: se queda 3 segundos totales en idle
                if (_rend) _rend.material.color = Color.cyan;
                _timer += 2.0f; 
                break;
            case 1: // VIGILAR: Rota para ver su panorama, está 2 segundos totales
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
        //rotacion
        if (_choice == 1)
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