using UnityEngine;
using static EntityState;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    private StateMachine<EntityStates> _sm;
    [SerializeField] private TMP_Text stateDebugText;  
    [SerializeField] private Transform player;
    [SerializeField] private bool isAggressive;
    private int _patrolIterations = 0;

    // Accesos r�pidos SOLID
    public SteeringAgent Agent => GetComponent<SteeringAgent>();
    public ObstacleAvoidance Avoidance => GetComponent<ObstacleAvoidance>();
    public PatrolBehaviour Patrol => GetComponent<PatrolBehaviour>();
    public LineOfSight LoS => GetComponent<LineOfSight>();
    public Transform PlayerTransform => player;
    public bool IsAggressive => isAggressive;
    public StateMachine<EntityStates> StateMachine => _sm;

    void Awake()
    {
        _sm = new StateMachine<EntityStates>();
        _sm.AddState(EntityStates.Idle, new EntityIdleState(this, _sm));
        _sm.AddState(EntityStates.Patrol, new EntityPatrolState(this, _sm));
        _sm.AddState(EntityStates.Chase, new EntityChaseState(this, _sm));
        _sm.AddState(EntityStates.RunAway, new EntityRunAwayState(this, _sm));
        _sm.AddState(EntityStates.Attack, new EntityAttackState(this, _sm));

        _sm.SetCurrent(new EntityPatrolState(this, _sm));
    }

    void Update()
    {
        // El �rbol decide el estado, la FSM lo ejecuta
        GetComponent<EnemyDecisionTree>().OnUpdate();
        _sm.Update();
        
        // UI state name
        if (stateDebugText != null && _sm.CurrentState != null)
        {
            stateDebugText.text = _sm.CurrentState.GetType().Name; // "Estado: " + 
        }
    }

    public void AddIteration() => _patrolIterations++;
    public void ResetIterations() => _patrolIterations = 0;
    public bool NeedsToIdle => _patrolIterations >= 3;
}