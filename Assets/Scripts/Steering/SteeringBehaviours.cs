using UnityEngine;

public static class SteeringBehaviours
{
    public static Vector3 Seek(Transform agent, Vector3 targetPos, Vector3 currentVelocity, float maxSpeed)
    {
        Vector3 flatTarget = new Vector3(targetPos.x, agent.position.y, targetPos.z);
        Vector3 desired = (flatTarget - agent.position).normalized * maxSpeed;
        return desired - currentVelocity;
    }

    public static Vector3 Flee(Transform agent, Vector3 targetPos, Vector3 currentVelocity, float maxSpeed)
    {
        Vector3 flatTarget = new Vector3(targetPos.x, agent.position.y, targetPos.z);
        Vector3 desired = (agent.position - flatTarget).normalized * maxSpeed;
        return desired - currentVelocity;
    }

    // Pursuit: Persigue prediciendo la posici�n futura del jugador 
    public static Vector3 Pursuit(Transform agent, Transform target, Vector3 targetVelocity, Vector3 currentVelocity, float maxSpeed)
    {
        float distance = Vector3.Distance(target.position, agent.position);
        float predictionTime = distance / maxSpeed;
        Vector3 futurePosition = target.position + targetVelocity * predictionTime;

        return Seek(agent, futurePosition, currentVelocity, maxSpeed);
    }

    // Evade: Huye de la posici�n futura del jugador
    public static Vector3 Evade(Transform agent, Transform target, Vector3 targetVelocity, Vector3 currentVelocity, float maxSpeed)
    {
        float distance = Vector3.Distance(target.position, agent.position);
        float predictionTime = distance / maxSpeed;
        Vector3 futurePosition = target.position + targetVelocity * predictionTime;

        return Flee(agent, futurePosition, currentVelocity, maxSpeed);
    }
}