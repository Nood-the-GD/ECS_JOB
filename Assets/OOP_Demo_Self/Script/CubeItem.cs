using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeItem : MonoBehaviour
{
    #region Constants
    private const float SEPARATION_WEIGHT = 2f;
    private const float ALIGNMENT_WEIGHT = 1f;
    private const float COHESION_WEIGHT = 1f;
    private const float TARGET_WEIGHT = 1.5f;
    #endregion

    #region Private Fields
    [SerializeField] private float _separationDistance = 0.5f;
    [SerializeField] private float _alignmentDistance = 1f;
    [SerializeField] private float _cohesionDistance = 1.5f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _maxForce = 5f;
    [SerializeField] private float _arrivalDistance = 0.1f;
    [SerializeField] private float _drag = 0.95f;
    [SerializeField] private float _minSpeed = 0.1f;

    private Vector3 _targetPos;
    private Vector3 _velocity = Vector3.zero;

    public Vector3 Velocity => _velocity;
    private List<Transform> _allCube = new List<Transform>();
    public bool IsArrived => Vector3.Distance(transform.position, _targetPos) <= _arrivalDistance;
    #endregion

    #region Public Methods
    public void AddCube(Transform cube)
    {
        if (cube == this.transform)
            return;
        _allCube.Add(cube);
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        targetPos.z = 0;
        _targetPos = targetPos;
    }
    #endregion

    #region Unity Lifecycle
    void Update()
    {
        if (ShouldStop())
        {
            return;
        }

        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 targetSeek = CalculateTargetSeek();

        // Combine all forces
        Vector3 totalForce = separation * SEPARATION_WEIGHT +
                           alignment * ALIGNMENT_WEIGHT +
                           cohesion * COHESION_WEIGHT +
                           targetSeek * TARGET_WEIGHT;

        // Apply force to velocity
        ApplyForce(totalForce);
    }
    #endregion

    #region Private Methods
    private bool ShouldStop()
    {
        if (Vector3.Distance(transform.position, _targetPos) <= _arrivalDistance)
        {
            return true;
        }
        var nearbyCubes = GetNearbyCubes(_alignmentDistance);
        bool isAnyArrive = false;
        for (int i = 0; i < nearbyCubes.Count; i++)
        {
            if (nearbyCubes[i].GetComponent<CubeItem>().IsArrived)
            {
                isAnyArrive = true;
                break;
            }
        }
        foreach (var cube in nearbyCubes)
        {
            var distanceToNearbyCube = Vector3.Distance(transform.position, cube.position);
            if (distanceToNearbyCube > _separationDistance && isAnyArrive)
            {
                return true;
            }
        }

        return false;
    }
    private Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero;
        List<Transform> nearbyCubes = GetNearbyCubes(_separationDistance);

        foreach (var cube in nearbyCubes)
        {
            Vector3 direction = transform.position - cube.position;
            float distance = direction.magnitude;

            if (distance > 0)
            {
                direction = direction.normalized / distance; // Weight by distance
                separation += direction;
            }
        }

        return separation.normalized;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 alignment = Vector3.zero;
        List<Transform> nearbyCubes = GetNearbyCubes(_alignmentDistance);

        if (nearbyCubes.Count == 0) return alignment;

        foreach (var cube in nearbyCubes)
        {
            CubeItem cubeItem = cube.GetComponent<CubeItem>();
            if (cubeItem != null)
            {
                alignment += cubeItem.Velocity;
            }
        }

        alignment /= nearbyCubes.Count;
        return alignment.normalized;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 cohesion = Vector3.zero;
        List<Transform> nearbyCubes = GetNearbyCubes(_cohesionDistance);

        if (nearbyCubes.Count == 0) return cohesion;

        foreach (var cube in nearbyCubes)
        {
            cohesion += cube.position;
        }

        cohesion /= nearbyCubes.Count;
        cohesion = cohesion - transform.position;
        return cohesion.normalized;
    }

    private Vector3 CalculateTargetSeek()
    {
        if (_targetPos == Vector3.zero) return Vector3.zero;

        Vector3 desired = _targetPos - transform.position;
        return desired.normalized;
    }

    private List<Transform> GetNearbyCubes(float radius)
    {
        List<Transform> nearbyCubes = new List<Transform>();

        foreach (var cube in _allCube)
        {
            if (cube != null)
            {
                float distance = Vector3.Distance(cube.position, transform.position);
                if (distance <= radius)
                {
                    nearbyCubes.Add(cube);
                }
            }
        }

        return nearbyCubes;
    }

    private void ApplyForce(Vector3 force)
    {
        // Limit the force
        force = Vector3.ClampMagnitude(force, _maxForce);

        // Apply the force to velocity
        _velocity += force;

        // Limit the velocity
        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);

        // Update position
        transform.position += _velocity * Time.deltaTime;
    }
    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        // Separation distance (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _separationDistance);

        // Alignment distance (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _alignmentDistance);

        // Cohesion distance (green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _cohesionDistance);

        // Velocity direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _velocity.normalized * 0.5f);
    }
    #endregion
}
