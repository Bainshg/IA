using System.Collections.Generic;
using UnityEngine;

public class EntityChaseState : EntityState
{
    private EnemyAI _ai;
    private PlayerMovement _playerMovement;

    // ruta de pathfinding solo cuando se pierde la linea de vista
    private List<Vector3> _path;
    private int _pathIndex;
    private float _repathTimer;

    private const float RepathInterval = 0.4f;   // recálculo de ruta (no por frame)
    private const float WaypointTolerance = 1.0f; // distancia para dar por alcanzado un nodo

    public EntityChaseState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm)
    {
        _ai = ai;
        // PlayerTransform puede ser null 
        _playerMovement = _ai.PlayerTransform != null ? _ai.PlayerTransform.GetComponent<PlayerMovement>() : null;
    }

    public override void Awake()
    {
        // al (re)entrar a Chase tiro la ruta vieja para forzar recalculo
        _path = null;
        _repathTimer = 0f;
    }

    public override void Execute()
    {
        bool canSee = _ai.LoS.CanSeeTarget(_ai.PlayerTransform);

        if (canSee)
        {
            // con linea de vista -> steering (Pursuit)
            _ai.MarkPlayerSeen(_ai.PlayerTransform.position);
            _path = null; // mientras lo veo no pathfindeo

            Vector3 playerVelocity = _playerMovement != null ? _playerMovement.Velocity : Vector3.zero;
            Vector3 steer = SteeringBehaviours.Pursuit(
                _ai.transform,
                _ai.PlayerTransform,
                playerVelocity,
                _ai.Agent.Velocity,
                _ai.Agent.MaxSpeed
            );
            steer += _ai.Avoidance.GetAvoidanceForce();
            _ai.Agent.ApplySteering(steer);
        }
        else
        {
            // sin linea de vista -> Theta* a la ultima posicion conocida
            FollowPathToLastKnown();
        }
    }

    private void FollowPathToLastKnown()
    {
        _repathTimer -= Time.deltaTime;
        if (_path == null || _repathTimer <= 0f)
        {
            LayerMask mask = GridManager.Instance != null ? GridManager.Instance.ObstacleMask : 0;
            _path = ThetaStar.FindPath(_ai.transform.position, _ai.LastKnownPlayerPos, mask);
            _pathIndex = 0;
            _repathTimer = RepathInterval;
        }

        // sin ruta o ruta agotada: llego al ultimo punto conocido y no lo encontro.
        // olvida al jugador para que el arbol lo mande de vuelta a Patrol/Idle.
        if (_path == null || _pathIndex >= _path.Count)
        {
            _ai.ForgetPlayer();
            _ai.Agent.StopAgent();
            return;
        }

        Vector3 target = _path[_pathIndex];
        if (Vector3.Distance(_ai.transform.position, target) <= WaypointTolerance)
        {
            _pathIndex++;
            if (_pathIndex >= _path.Count)
            {
                _ai.ForgetPlayer();
                _ai.Agent.StopAgent();
                return;
            }
            target = _path[_pathIndex];
        }

        // Seek al nodo de la ruta + avoidance para obstaculos chicos
        Vector3 steer = SteeringBehaviours.Seek(_ai.transform, target, _ai.Agent.Velocity, _ai.Agent.MaxSpeed);
        steer += _ai.Avoidance.GetAvoidanceForce();
        _ai.Agent.ApplySteering(steer);
    }
}
