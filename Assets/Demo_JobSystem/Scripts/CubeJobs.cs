using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct CalculateSeparationJob : IJobParallelFor
{
    public NativeArray<CubeJobItemData> allCube;
    [ReadOnly]
    public float separationDistance;

    public CalculateSeparationJob(NativeArray<CubeJobItemData> allCube, float separationDistance)
    {
        this.allCube = allCube;
        this.separationDistance = separationDistance;
    }

    [BurstCompile]
    public void Execute(int index)
    {
        var cubeJobItemData = allCube[index];
        NativeList<int> nearbyCubesIndices = new NativeList<int>(Allocator.Temp);

        for (int i = 0; i < nearbyCubesIndices.Length; i++)
        {
            int nearbyIndex = nearbyCubesIndices[i];
            var nearbyCube = allCube[nearbyIndex];

            Vector3 direction = cubeJobItemData.Position - nearbyCube.Position;
            float distance = direction.magnitude;

            if (distance > 0)
            {
                direction = direction.normalized / distance; // Weight by distance
                cubeJobItemData.Separation += direction;
            }
        }
        allCube[index] = cubeJobItemData;
    }
}
[BurstCompile]
public struct CalculateAlignmentJob : IJobParallelFor
{
    public NativeArray<CubeJobItemData> allCube;
    [ReadOnly]
    public float radius;

    public CalculateAlignmentJob(NativeArray<CubeJobItemData> allCube, float radius)
    {
        this.allCube = allCube;
        this.radius = radius;
    }

    [BurstCompile]
    public void Execute(int index)
    {
        var cubeJobItemData = allCube[index];
        int startIndex = cubeJobItemData.NearbyCubesStartIndex;
        int endIndex = cubeJobItemData.NearbyCubesEndIndex;

        NativeList<int> nearbyCubesIndices = new NativeList<int>(Allocator.Temp);
        for (int i = 0; i < allCube.Length; i++)
        {
            var cube = allCube[i];
            if (cube.Index == cubeJobItemData.Index) continue;
            float distance = Vector3.Distance(cube.Position, cubeJobItemData.Position);
            if (distance <= radius)
            {
                nearbyCubesIndices.Add(i);
            }
        }

        Vector3 alignment = Vector3.zero;
        if (endIndex - startIndex == 0)
        {
            cubeJobItemData.Alignment = alignment;
            allCube[index] = cubeJobItemData;
            return;
        }

        for (int i = startIndex; i <= endIndex; i++)
        {
            int nearbyIndex = nearbyCubesIndices[startIndex + i];
            var nearbyCube = allCube[nearbyIndex];
            alignment += nearbyCube.Velocity;
        }

        alignment /= nearbyCubesIndices.Length;
        cubeJobItemData.Alignment = alignment.normalized;

        allCube[index] = cubeJobItemData;
    }
}
[BurstCompile]
public struct CalculateCohesionJob : IJobParallelFor
{
    public NativeArray<CubeJobItemData> allCube;
    [ReadOnly]
    public float radius;

    public CalculateCohesionJob(NativeArray<CubeJobItemData> allCube, float radius)
    {
        this.allCube = allCube;
        this.radius = radius;
    }

    [BurstCompile]
    public void Execute(int index)
    {
        var cubeJobItemData = allCube[index];
        int startIndex = cubeJobItemData.NearbyCubesStartIndex;

        NativeList<int> nearbyCubesIndices = new NativeList<int>(Allocator.Temp);
        for (int i = 0; i < allCube.Length; i++)
        {
            var cube = allCube[i];
            if (cube.Index == cubeJobItemData.Index) continue;
            float distance = Vector3.Distance(cube.Position, cubeJobItemData.Position);
            if (distance <= radius)
            {
                nearbyCubesIndices.Add(i);
            }
        }

        Vector3 cohesion = Vector3.zero;
        if (nearbyCubesIndices.Length == 0)
        {
            cubeJobItemData.Cohesion = cohesion;
            allCube[index] = cubeJobItemData;
            return;
        }

        for (int i = 0; i < nearbyCubesIndices.Length; i++)
        {
            int nearbyIndex = nearbyCubesIndices[startIndex + i];
            var nearbyCube = allCube[nearbyIndex];
            cohesion += nearbyCube.Position;
        }

        cohesion /= nearbyCubesIndices.Length;
        cubeJobItemData.Cohesion = cohesion - cubeJobItemData.Position;

        allCube[index] = cubeJobItemData;
    }
}
[BurstCompile]
public struct CalculateTargetSeekJob : IJobParallelFor
{
    [ReadOnly]
    public Vector3 targetPos;
    public NativeArray<CubeJobItemData> allCube;

    public CalculateTargetSeekJob(Vector3 targetPos, NativeArray<CubeJobItemData> allCube)
    {
        this.targetPos = targetPos;
        this.allCube = allCube;
    }

    [BurstCompile]
    public void Execute(int index)
    {
        var cubeJobItemData = allCube[index];

        if (targetPos == Vector3.zero)
        {
            cubeJobItemData.TargetSeek = Vector3.zero;
            allCube[index] = cubeJobItemData;
            return;
        }
        Vector3 desired = targetPos - cubeJobItemData.Position;
        cubeJobItemData.TargetSeek = desired.normalized;

        allCube[index] = cubeJobItemData;
    }
}
// [BurstCompile]
// public struct GetNearbyCubesJob : IJobParallelFor
// {
//     public NativeArray<CubeJobItemData> allCube;
//     [WriteOnly]
//     public NativeArray<NearbyCubesData> nearbyCubesIndices;
//     [ReadOnly]
//     public float radius;

//     public GetNearbyCubesJob(NativeArray<CubeJobItemData> allCube, float radius)
//     {
//         this.allCube = allCube;
//         this.radius = radius;
//     }

//     [BurstCompile]
//     public void Execute(int index)
//     {
//         var cubeJobItemData = allCube[index];
//         int nearbyCount = 0;
//         int startIndex = 0; // Mỗi cube có tối đa 10 nearby cubes
//         int endIndex = 0;
//         int[] nearbyCubesIndicesArray = new int[10];

//         for (int i = 0; i < allCube.Length && nearbyCount < 10; i++)
//         {
//             var cube = allCube[i];
//             if (cube.Index == cubeJobItemData.Index) continue;
//             float distance = Vector3.Distance(cube.Position, cubeJobItemData.Position);
//             if (distance <= radius)
//             {
//                 nearbyCubesIndicesArray[nearbyCount] = i;
//                 nearbyCount++;
//                 endIndex = i;
//                 // nearbyCubesIndices[index].Add(i);
//             }
//         }

//         cubeJobItemData.NearbyCubesCount = nearbyCount;
//         cubeJobItemData.NearbyCubesStartIndex = startIndex;
//         cubeJobItemData.NearbyCubesEndIndex = endIndex;
//         CubeJobManager.Instance.UpdateNearbyCubesIndices(index, nearbyCubesIndicesArray);
//         allCube[index] = cubeJobItemData;
//     }
// }
[BurstCompile]
public struct CalculateNewPosJob : IJobParallelFor
{
    public NativeArray<CubeJobItemData> allCube;
    [ReadOnly]
    public NativeArray<Vector3> forces;
    [ReadOnly]
    public float maxForce;
    [ReadOnly]
    public float maxSpeed;
    [ReadOnly]
    public float deltaTime;

    public CalculateNewPosJob(NativeArray<Vector3> forces, NativeArray<CubeJobItemData> allCube, float maxForce, float maxSpeed, float deltaTime)
    {
        this.forces = forces;
        this.allCube = allCube;
        this.maxForce = maxForce;
        this.maxSpeed = maxSpeed;
        this.deltaTime = deltaTime;
    }

    [BurstCompile]
    public void Execute(int index)
    {
        var cubeJobItemData = allCube[index];
        var force = forces[index];

        // Limit the force
        force = Vector3.ClampMagnitude(force, maxForce);

        // Apply the force to velocity
        cubeJobItemData.Velocity += force;

        // Limit the velocity
        cubeJobItemData.Velocity = Vector3.ClampMagnitude(cubeJobItemData.Velocity, maxSpeed);

        // Update position
        cubeJobItemData.NewPosition = cubeJobItemData.Position + cubeJobItemData.Velocity * deltaTime;

        allCube[index] = cubeJobItemData;
    }
}

[BurstCompile]
public struct ShouldStopJob : IJobParallelFor
{
    public NativeArray<CubeJobItemData> allCube;
    [ReadOnly]
    public Vector3 targetPos;
    [ReadOnly]
    public float arrivalDistance;
    [ReadOnly]
    public float scanDistance;
    [ReadOnly]
    public float radius;

    public ShouldStopJob(NativeArray<CubeJobItemData> allCube, Vector3 targetPos, float arrivalDistance, float scanDistance, float radius)
    {
        this.targetPos = targetPos;
        this.arrivalDistance = arrivalDistance;
        this.scanDistance = scanDistance;
        this.allCube = allCube;
        this.radius = radius;
    }

    [BurstCompile]
    public void Execute(int index)
    {
        var cubeJobItemData = allCube[index];
        int startIndex = cubeJobItemData.NearbyCubesStartIndex;

        cubeJobItemData.IsArrived = Vector3.Distance(cubeJobItemData.Position, targetPos) <= arrivalDistance;
        if (cubeJobItemData.IsArrived)
        {
            cubeJobItemData.IsShouldStop = true;
            allCube[index] = cubeJobItemData;
            return;
        }

        NativeList<int> nearbyCubesIndices = new NativeList<int>(Allocator.Temp);
        for (int i = 0; i < allCube.Length; i++)
        {
            var cube = allCube[i];
            if (cube.Index == cubeJobItemData.Index) continue;
            float distance = Vector3.Distance(cube.Position, cubeJobItemData.Position);
            if (distance <= radius)
            {
                nearbyCubesIndices.Add(i);
            }
        }

        bool isAnyArrive = false;
        for (int i = 0; i < nearbyCubesIndices.Length; i++)
        {
            int nearbyIndex = nearbyCubesIndices[startIndex + i];
            var nearbyCube = allCube[nearbyIndex];
            if (nearbyCube.IsArrived && nearbyCube.Index != index)
            {
                isAnyArrive = true;
                break;
            }
        }

        for (int i = 0; i < nearbyCubesIndices.Length; i++)
        {
            int nearbyIndex = nearbyCubesIndices[startIndex + i];
            var nearbyCube = allCube[nearbyIndex];
            var distanceToNearbyCube = Vector3.Distance(cubeJobItemData.Position, nearbyCube.Position);
            if (distanceToNearbyCube <= scanDistance && isAnyArrive)
            {
                cubeJobItemData.IsShouldStop = true;
                allCube[index] = cubeJobItemData;
                return;
            }
        }
        cubeJobItemData.IsShouldStop = false;

        allCube[index] = cubeJobItemData;
    }
}
// [BurstCompile]
// public class UpdateCubeJobDataJob : IJobParallelFor
// {
//     public NativeArray<CubeJobItemData> allCube;

//     public UpdateCubeJobDataJob(NativeArray<CubeJobItemData> allCube, )
//     {
//         this.allCube = allCube;
//     }

//     [BurstCompile]
//     public void Execute(int index)
//     {

//     }
// }