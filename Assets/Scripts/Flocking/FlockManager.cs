using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolBehaviour))]
public class FlockManager : MonoBehaviour
{
    public static FlockManager Instance;

    [Header("Configuraci�n del Enjambre")]
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private int flockSize = 10;

    [Header("Par�metros de las 3 Reglas")]
    [Range(1f, 10f)] public float neighborRadius = 3.0f;
    [Range(0f, 5f)] public float separationWeight = 1.5f;
    [Range(0f, 5f)] public float alignmentWeight = 1.0f;
    [Range(0f, 5f)] public float cohesionWeight = 1.0f;
    [Range(0f, 5f)] public float patrolWeight = 2.0f; // Reemplaza el peso del Boss

    private List<FlockAgent> _agents = new List<FlockAgent>();
    private PatrolBehaviour _myPatrol;

    public List<FlockAgent> Agents => _agents;

    void Awake()
    {
        Instance = this;
        _myPatrol = GetComponent<PatrolBehaviour>();
    }

    void Start()
    {
        // Guardamos los waypoints que configuraste en este m�nager
        Transform[] waypointsDelManager = _myPatrol.GetWaypoints();

        for (int i = 0; i < flockSize; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * 3f;
            randomPos.y = transform.position.y;

            GameObject go = Instantiate(slimePrefab, randomPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
            FlockAgent agent = go.GetComponent<FlockAgent>();

            if (agent != null)
            {
                Register(agent);

                PatrolBehaviour agentPatrol = agent.GetComponent<PatrolBehaviour>();
                if (agentPatrol != null && waypointsDelManager != null)
                {
                    agentPatrol.SetWaypoints(waypointsDelManager);
                }
            }
        }
    }

    // cada agente se anota una sola vez. el guard de duplicados deja convivir el alta
    // del manager con el auto-registro del agente (OnEnable) sin meterlo dos veces.
    public void Register(FlockAgent agent)
    {
        if (agent != null && !_agents.Contains(agent)) _agents.Add(agent);
    }

    // lo llama FlockAgent.OnDestroy(). sin esto, al morir un slime quedaba una ref
    // destruida en la lista y GetNeighbors tiraba MissingReferenceException.
    public void Unregister(FlockAgent agent)
    {
        _agents.Remove(agent);
    }
}
