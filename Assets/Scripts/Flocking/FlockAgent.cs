using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteeringAgent))]
[RequireComponent(typeof(PatrolBehaviour))]
public class FlockAgent : MonoBehaviour
{
    private SteeringAgent _steeringAgent;
    private PatrolBehaviour _patrol;
    private readonly List<FlockAgent> _neighbors = new List<FlockAgent>();

    // FSM propia del flock (usa el StateMachine<T> generico). el flock deja de usar el
    // cerebro heredado (EnemyAI/EnemyDecisionTree); se decide todo aca.
    private StateMachine<FlockStates> _sm;

    // seguidor de ruta Theta* (lo comparten los estados). ir por ruta evita el lio de
    // Seek-vs-avoidance: Theta* garantiza vista libre entre nodos, asi el Seek nodo a
    // nodo nunca apunta a una pared.
    private List<Vector3> _path;
    private int _pathIndex;
    private float _repathTimer;
    private Vector3 _pathDestination;
    private const float RepathInterval = 0.5f;  // recálculo de ruta (no por frame)
    private const float NodeTolerance = 1.0f;    // distancia para dar por alcanzado un nodo/destino

    public SteeringAgent Steering => _steeringAgent;
    public PatrolBehaviour Patrol => _patrol;
    public IReadOnlyList<FlockAgent> Neighbors => _neighbors;

    void Awake()
    {
        _steeringAgent = GetComponent<SteeringAgent>();
        _patrol = GetComponent<PatrolBehaviour>();

        // el Variant hereda el cerebro del Enemy base (EnemyAI + EnemyDecisionTree). ese
        // cerebro compite por el mismo SteeringAgent y, con 'player' vacio, tiraria NRE
        // cada frame. lo apago para que el unico que mueva sea FlockAgent.
        DisableInheritedBrain();

        _sm = new StateMachine<FlockStates>();
        _sm.AddState(FlockStates.Flocking, new FlockingState(this, _sm));
        _sm.AddState(FlockStates.Regroup, new RegroupState(this, _sm));
        _sm.SetCurrent(new FlockingState(this, _sm));
    }

    void OnEnable()
    {
        // auto-registro por las dudas (reactivaciones, agentes puestos a mano).
        // Register() ignora duplicados, asi no choca con el alta del manager.
        if (FlockManager.Instance != null) FlockManager.Instance.Register(this);
    }

    void OnDestroy()
    {
        // al morir el slime me saco de la lista del manager, asi ningun vecino lee un
        // transform ya destruido (el bug del MissingReference).
        if (FlockManager.Instance != null) FlockManager.Instance.Unregister(this);
    }

    void Update()
    {
        UpdateNeighbors();

        // si quedo aislado del grupo, prioriza reagrupar; con vecinos cerca, flocking normal.
        _sm.ChangeState(_neighbors.Count > 0 ? FlockStates.Flocking : FlockStates.Regroup);
        _sm.Update();
    }

    private void UpdateNeighbors()
    {
        _neighbors.Clear();
        if (FlockManager.Instance == null) return;

        List<FlockAgent> all = FlockManager.Instance.Agents;
        float radius = FlockManager.Instance.neighborRadius;

        for (int i = 0; i < all.Count; i++)
        {
            FlockAgent agent = all[i];
            // saltar nulos (destruidos este frame) y a mi mismo
            if (agent == null || agent == this) continue;

            if (Vector3.Distance(transform.position, agent.transform.position) <= radius)
                _neighbors.Add(agent);
        }
    }

    // fuerza de steering que sigue una ruta Theta* hasta 'destination' rodeando paredes.
    // recalcula por intervalo o si cambia el destino. sin grid o sin ruta, cae a Seek directo.
    public Vector3 SteerAlongPathTo(Vector3 destination)
    {
        _repathTimer -= Time.deltaTime;
        bool destinationChanged = Vector3.Distance(destination, _pathDestination) > NodeTolerance;

        if (_path == null || _repathTimer <= 0f || destinationChanged)
        {
            LayerMask mask = GridManager.Instance != null ? GridManager.Instance.ObstacleMask : 0;
            _path = ThetaStar.FindPath(transform.position, destination, mask);
            _pathDestination = destination;
            _pathIndex = 0;
            _repathTimer = RepathInterval;
        }

        if (_path == null || _path.Count == 0)
            return SteeringBehaviours.Seek(transform, destination, _steeringAgent.Velocity, _steeringAgent.MaxSpeed);

        if (_pathIndex >= _path.Count) _pathIndex = _path.Count - 1;

        Vector3 node = _path[_pathIndex];
        if (Vector3.Distance(transform.position, node) <= NodeTolerance && _pathIndex < _path.Count - 1)
        {
            _pathIndex++;
            node = _path[_pathIndex];
        }
        return SteeringBehaviours.Seek(transform, node, _steeringAgent.Velocity, _steeringAgent.MaxSpeed);
    }

    private void DisableInheritedBrain()
    {
        // se podrian sacar del Variant en el Inspector, pero por codigo es robusto y no
        // depende del estado del prefab.
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;

        EnemyDecisionTree decisionTree = GetComponent<EnemyDecisionTree>();
        if (decisionTree != null) decisionTree.enabled = false;
    }
}
