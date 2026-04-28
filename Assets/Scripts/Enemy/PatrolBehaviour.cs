using UnityEngine;

public class PatrolBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private int _currentIndex = 0;
    private int _direction = 1;

    void Start()
    {
        SetClosestWaypointAsStart();
    }

    private void SetClosestWaypointAsStart()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        float closestDistance = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentIndex = closestIndex;
    }

    public Vector3 GetCurrentWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return transform.position;
        return waypoints[_currentIndex].position;
    }

    public void UpdateToNextPoint()
    {
        if (waypoints.Length <= 1) return;

        _currentIndex += _direction;

        if (_currentIndex >= waypoints.Length)
        {
            _direction = -1;
            _currentIndex = waypoints.Length - 2;
        }
        else if (_currentIndex < 0)
        {
            _direction = 1;
            _currentIndex = 1;
        }

        _currentIndex = Mathf.Clamp(_currentIndex, 0, waypoints.Length - 1);
    }

    public bool HasReachedPoint(float threshold = 1.2f)
    {
        return Vector3.Distance(transform.position, GetCurrentWaypoint()) < threshold;
    }
}