using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
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
    [SerializeField] protected float _arrivalDistance = 0.1f;
    [SerializeField] private float _drag = 0.95f;
    [SerializeField] private float _minSpeed = 0.1f;
    #endregion
    protected Vector3 _targetPos = Vector3.zero;

    public bool CanStart;

    List<CubeItemManaged> _cubeItems = new List<CubeItemManaged>();

    public void AddCube(CubeItemManaged cube)
    {
        _cubeItems.Add(cube);
    }
    public void SetTargetPos(Vector3 targetPos)
    {
        targetPos.z = 0;
        _targetPos = targetPos;
    }

    public virtual void Update()
    {
        if (!CanStart)
        {
            return;
        }
        foreach (var cube in _cubeItems)
        {
            var dirToTarget = _targetPos - cube.transform.position;
            MoveCube(cube.transform, dirToTarget);
            cube.IsArrived = Vector3.Distance(cube.transform.position, _targetPos) <= _arrivalDistance;
        }
    }

    #region Boil
    private void MoveCube(Transform cube, Vector3 targetDirection)
    {
        if (ShouldStop(cube))
        {
            return;
        }
        Vector3 separation = CalculateSeparation(cube);
        Vector3 alignment = CalculateAlignment(cube);
        Vector3 cohesion = CalculateCohesion(cube);
        Vector3 targetSeek = targetDirection;

        // Combine all forces
        Vector3 totalForce = separation * SEPARATION_WEIGHT +
                           alignment * ALIGNMENT_WEIGHT +
                           cohesion * COHESION_WEIGHT +
                           targetSeek * TARGET_WEIGHT;

        // Apply force to velocity
        ApplyForce(cube, totalForce);
    }
    private void ApplyForce(Transform cube, Vector3 force)
    {
        // Limit the force
        force = Vector3.ClampMagnitude(force, _maxForce);
        var _velocity = cube.GetComponent<CubeItemManaged>().Velocity;

        // Apply the force to velocity
        _velocity += force;

        // Limit the velocity
        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);

        // Update position
        cube.position += _velocity * Time.deltaTime;
        cube.GetComponent<CubeItemManaged>().Velocity = _velocity;
    }
    private bool ShouldStop(Transform cube)
    {
        if (Vector3.Distance(cube.position, _targetPos) <= _arrivalDistance)
        {
            return true;
        }
        var nearbyCubes = GetNearbyCubes(cube, _alignmentDistance);
        bool isAnyArrive = false;
        for (int i = 0; i < nearbyCubes.Count; i++)
        {
            if (nearbyCubes[i].GetComponent<CubeItemManaged>().IsArrived)
            {
                isAnyArrive = true;
                break;
            }
        }
        foreach (var nearbyCube in nearbyCubes)
        {
            var distanceToNearbyCube = Vector3.Distance(cube.position, nearbyCube.position);
            if (distanceToNearbyCube > _separationDistance && isAnyArrive)
            {
                return true;
            }
        }

        return false;
    }
    private Vector3 CalculateSeparation(Transform cube)
    {
        Vector3 separation = Vector3.zero;
        List<Transform> nearbyCubes = GetNearbyCubes(cube, _separationDistance);

        foreach (var nearbyCube in nearbyCubes)
        {
            Vector3 direction = cube.position - nearbyCube.position;
            float distance = direction.magnitude;

            if (distance > 0)
            {
                direction = direction.normalized / distance; // Weight by distance
                separation += direction;
            }
        }

        return separation.normalized;
    }

    private Vector3 CalculateAlignment(Transform cube)
    {
        Vector3 alignment = Vector3.zero;
        List<Transform> nearbyCubes = GetNearbyCubes(cube, _alignmentDistance);

        if (nearbyCubes.Count == 0) return alignment;

        foreach (var nearbyCube in nearbyCubes)
        {
            CubeItem cubeItem = nearbyCube.GetComponent<CubeItem>();
            if (cubeItem != null)
            {
                alignment += cubeItem.Velocity;
            }
        }

        alignment /= nearbyCubes.Count;
        return alignment.normalized;
    }

    private Vector3 CalculateCohesion(Transform cube)
    {
        Vector3 cohesion = Vector3.zero;
        List<Transform> nearbyCubes = GetNearbyCubes(cube, _cohesionDistance);

        if (nearbyCubes.Count == 0) return cohesion;

        foreach (var nearbyCube in nearbyCubes)
        {
            cohesion += cube.position;
        }

        cohesion /= nearbyCubes.Count;
        cohesion = cohesion - cube.position;
        return cohesion.normalized;
    }


    private List<Transform> GetNearbyCubes(Transform cube, float radius)
    {
        List<Transform> nearbyCubes = new List<Transform>();

        foreach (var nearbyCube in _cubeItems)
        {
            if (nearbyCube != null)
            {
                float distance = Vector3.Distance(cube.position, nearbyCube.transform.position);
                if (distance <= radius)
                {
                    nearbyCubes.Add(nearbyCube.transform);
                }
            }
        }

        return nearbyCubes;
    }
    #endregion

}
