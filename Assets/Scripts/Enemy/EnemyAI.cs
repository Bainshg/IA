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
        _sm.AddState(EntityStates.Stunned, new EntityStunnedState(this, _sm));

        _sm.SetCurrent(new EntityPatrolState(this, _sm));
        
    }
    void Update()
    {
        // El arbol decide el estado, la FSM lo ejecuta
        GetComponent<EnemyDecisionTree>().OnUpdate();
        _sm.Update();
        
        // UI para state name
        if (stateDebugText != null && _sm.CurrentState != null)
        {
            stateDebugText.text = _sm.CurrentState.GetType().Name; //
        }
    }
    
    public void AddIteration() => _patrolIterations++;
    public void ResetIterations() => _patrolIterations = 0;
    public bool NeedsToIdle => _patrolIterations >= 3;

    // --- memoria del objetivo (para perseguir sin LoS con Theta*) ---
    // al perder LoS no olvida al toque: recuerda donde lo vio y lo persigue por una ventana.
    private float _lastSeenTime = -999f;
    public Vector3 LastKnownPlayerPos { get; private set; }

    public void MarkPlayerSeen(Vector3 pos)
    {
        LastKnownPlayerPos = pos;
        _lastSeenTime = Time.time;
    }

    public void ForgetPlayer() => _lastSeenTime = -999f;

    public bool SawPlayerWithin(float seconds) => Time.time - _lastSeenTime <= seconds;
}