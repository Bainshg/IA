using System.Collections.Generic;
using UnityEngine;

// el agente perdio contacto con el enjambre (0 vecinos en el radio) y deja la patrulla
// para volver al centro del grupo. estado con funcion real, no un Idle vacio.
public class RegroupState : FlockState
{
    public RegroupState(FlockAgent agent, StateMachine<FlockStates> sm) : base(agent, sm) { }

    public override void Execute()
    {
        SteeringAgent steering = _agent.Steering;
        FlockManager flock = FlockManager.Instance;
        if (flock == null) return;

        // centro de masa del resto del grupo (salteo nulos y a mi mismo)
        List<FlockAgent> all = flock.Agents;
        Vector3 centroid = Vector3.zero;
        int count = 0;
        for (int i = 0; i < all.Count; i++)
        {
            FlockAgent other = all[i];
            if (other == null || other == _agent) continue;
            centroid += other.transform.position;
            count++;
        }

        // si soy el unico que queda, no hay grupo al que volver: me quedo quieto
        if (count == 0)
        {
            steering.StopAgent();
            return;
        }

        centroid /= count;
        // vuelve al grupo por ruta Theta* (rodea paredes)
        Vector3 regroupForce = _agent.SteerAlongPathTo(centroid);
        steering.ApplySteering(regroupForce);
    }
}
