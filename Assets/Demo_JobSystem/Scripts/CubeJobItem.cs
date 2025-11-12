using Unity.Collections;
using UnityEngine;

public struct NearbyCubesData
{
    public int[] NearbyCubesIndices;

    public NearbyCubesData(int length)
    {
        NearbyCubesIndices = new int[length];
    }
}

public struct CubeJobItemData
{
    public int Index;
    public Vector3 Position;
    public Vector3 Velocity;
    public bool IsArrived;
    public Vector3 Separation;
    public Vector3 Alignment;
    public Vector3 Cohesion;
    public Vector3 TargetSeek;
    public Vector3 NewPosition;
    public bool IsShouldStop;

    // Thay vì lưu NearbyCubes trong struct, chúng ta sẽ lưu indices
    public int NearbyCubesCount;
    public int NearbyCubesStartIndex; // Index bắt đầu trong NearbyCubesArray
    public int NearbyCubesEndIndex; // Index bắt đầu trong NearbyCubesArray

    public CubeJobItemData(CubeJobItem cubeJobItem)
    {
        Index = cubeJobItem.Index;
        Position = cubeJobItem.transform.position;
        Velocity = cubeJobItem.Velocity;
        IsArrived = false;
        Separation = cubeJobItem.Separation;
        Alignment = cubeJobItem.Alignment;
        Cohesion = cubeJobItem.Cohesion;
        TargetSeek = cubeJobItem.TargetSeek;
        NewPosition = cubeJobItem.NewPosition;
        IsShouldStop = cubeJobItem.IsShouldStop;
        NearbyCubesCount = 0;
        NearbyCubesStartIndex = 0;
        NearbyCubesEndIndex = 0;
    }
}

public class CubeJobItem : MonoBehaviour
{
    #region Private Fields
    public float SeparationDistance = 0.5f;
    public float AlignmentDistance = 1f;
    public float CohesionDistance = 1.5f;
    public float MaxSpeed = 10f;
    public float MaxForce = 5f;
    #endregion

    public Vector3 Velocity = Vector3.zero;
    public Vector3 Separation = Vector3.zero;
    public Vector3 Alignment = Vector3.zero;
    public Vector3 Cohesion = Vector3.zero;
    public Vector3 TargetSeek = Vector3.zero;
    public Vector3 NewPosition = Vector3.zero;
    public bool IsShouldStop = false;
    public int Index { get; internal set; }


    #region Private Methods
    public void ApplyJobData(CubeJobItemData cubeJobItemData)
    {
        Index = cubeJobItemData.Index;
        transform.position = cubeJobItemData.NewPosition;
        Velocity = cubeJobItemData.Velocity;
        Separation = cubeJobItemData.Separation;
        Alignment = cubeJobItemData.Alignment;
        Cohesion = cubeJobItemData.Cohesion;
        TargetSeek = cubeJobItemData.TargetSeek;
        NewPosition = cubeJobItemData.NewPosition;
        IsShouldStop = cubeJobItemData.IsShouldStop;
    }
    #endregion
}
