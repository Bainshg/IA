using UnityEngine;

// en desuso (confirmar con el equipo): flocking alternativo con OverlapSphere. el que
// se usa es FlockManager + FlockAgent. no lo borro hasta confirmar que nadie lo necesita.
[RequireComponent(typeof(PatrolBehaviour))]
[RequireComponent(typeof(SteeringAgent))] // Obliga a tener tu agente de steering
public class Boid : MonoBehaviour
{
    [Header("Radios de Detecci�n")]
    [SerializeField] private float separationRadius = 2f;
    [SerializeField] private float cohesionRadius = 4f;

    [Header("Pesos de las Fuerzas")]
    [SerializeField, Range(0.0f, 3.0f)] private float separationWeight = 1.5f;
    [SerializeField, Range(0.0f, 3.0f)] private float cohesionWeight = 1.0f;
    [SerializeField, Range(0.0f, 3.0f)] private float alignmentWeight = 1.0f;
    [SerializeField, Range(0.0f, 3.0f)] private float patrolWeight = 2.0f;

    [SerializeField] private LayerMask boidMask;

    private Transform myTransform;
    private PatrolBehaviour _patrol;
    private SteeringAgent _myAgent; // Tu agente real de movimiento
    private Vector3 myPosition => myTransform.position;

    private void Awake()
    {
        myTransform = transform;
        _patrol = GetComponent<PatrolBehaviour>();
        _myAgent = GetComponent<SteeringAgent>();
    }

    void Start()
    {
        // Le metemos un impulso inicial directo usando tu script
        Vector3 randomImpulse = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        _myAgent.ApplySteering(randomImpulse * _myAgent.MaxSpeed);
    }

    void Update()
    {
        Flocking();
    }

    private void Flocking()
    {
        Vector3 totalForce = Vector3.zero;

        // Sumamos las 3 fuerzas l�gicas que te dio tu profesor
        totalForce += Separation() * separationWeight;
        totalForce += Cohesion() * cohesionWeight;
        totalForce += Alignment() * alignmentWeight;

        // Sumamos el avance por los Waypoints usando tu biblioteca est�tica 'SteeringBehaviours'
        if (_patrol != null && _patrol.GetWaypoints() != null && _patrol.GetWaypoints().Length > 0)
        {
            Vector3 targetWaypoint = _patrol.GetCurrentWaypoint();

            // LLAMADA A TU SCRIPT: SteeringBehaviours.Seek
            Vector3 patrolForce = SteeringBehaviours.Seek(myTransform, targetWaypoint, _myAgent.Velocity, _myAgent.MaxSpeed);
            totalForce += patrolForce * patrolWeight;

            // Si el grupo ronda la zona, avanza al siguiente nodo
            if (_patrol.HasReachedPoint(2.5f))
            {
                _patrol.UpdateToNextPoint();
            }
        }

        // Le mandamos la fuerza final acumulada a tu propio ApplySteering
        _myAgent.ApplySteering(totalForce);
    }

    private Vector3 Separation()
    {
        var boidsInRange = Physics.OverlapSphere(myPosition, separationRadius, boidMask);
        Vector3 totalForce = Vector3.zero;
        int cont = 0;
        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentBoid = boidsInRange[i];
            if (currentBoid == this.GetComponent<Collider>()) continue;

            var direction = myPosition - currentBoid.transform.position;
            var force = direction.normalized / (direction.magnitude / separationRadius);

            totalForce += force;
            cont++;
        }
        if (cont == 0) return Vector3.zero;

        totalForce /= cont;

        // Adaptado a tu script: calcula la fuerza deseada multiplicada por la velocidad mxima
        Vector3 desired = totalForce * _myAgent.MaxSpeed;
        return desired - _myAgent.Velocity;
    }

    private Vector3 Cohesion()
    {
        var avgPosition = Vector3.zero;
        int cont = 0;
        var boidsInRange = Physics.OverlapSphere(myPosition, cohesionRadius, boidMask);

        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentBoid = boidsInRange[i];
            if (currentBoid == this.GetComponent<Collider>()) continue;

            avgPosition += currentBoid.transform.position;
            cont++;
        }
        if (cont == 0) return Vector3.zero;

        avgPosition /= cont;

        // Usa tu Seek est�tico para ir al centro del enjambre
        return SteeringBehaviours.Seek(myTransform, avgPosition, _myAgent.Velocity, _myAgent.MaxSpeed);
    }

    private Vector3 Alignment()
    {
        var boidsInRange = Physics.OverlapSphere(myPosition, cohesionRadius, boidMask);
        Vector3 avgVelocity = Vector3.zero;
        int cont = 0;
        for (int i = 0; i < boidsInRange.Length; i++)
        {
            var currentBoid = boidsInRange[i].GetComponent<Boid>();
            if (currentBoid == this || currentBoid == null) continue;

            avgVelocity += currentBoid.transform.forward;
            cont++;
        }
        if (cont == 0) return Vector3.zero;

        Vector3 desired = avgVelocity.normalized * _myAgent.MaxSpeed;
        return desired - _myAgent.Velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, cohesionRadius);
    }
}