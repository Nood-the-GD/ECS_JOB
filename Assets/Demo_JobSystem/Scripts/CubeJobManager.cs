using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class CubeJobManager : CubeManager
{
    #region Constants
    private const float SEPARATION_WEIGHT = 2f;
    private const float ALIGNMENT_WEIGHT = 1f;
    private const float COHESION_WEIGHT = 1f;
    private const float TARGET_WEIGHT = 1.5f;
    #endregion

    public static CubeJobManager Instance;

    [SerializeField] private int CubeNumber = 100;
    [SerializeField] private CubeJobItem _cubeJobItemPrefab;
    CubeJobItem[] _cubeJobItems;
    NativeArray<CubeJobItemData> _cubeJobItemDatas;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnCubeJobItems();
    }

    private void SpawnCubeJobItems()
    {
        _cubeJobItems = new CubeJobItem[CubeNumber];
        _cubeJobItemDatas = new NativeArray<CubeJobItemData>(CubeNumber, Allocator.Persistent);

        for (int i = 0; i < CubeNumber; i++)
        {
            var cubeJobItem = Instantiate(_cubeJobItemPrefab, transform.position + new Vector3(i * 0.1f, 0, 0), Quaternion.identity);
            if (cubeJobItem != null)
            {
                RandomColor(cubeJobItem.transform);
            }
            cubeJobItem.Index = i;
            _cubeJobItems[i] = cubeJobItem;
            _cubeJobItemDatas[i] = new CubeJobItemData(cubeJobItem);
        }
    }
    private void RandomColor(Transform cube)
    {
        var c = Random.ColorHSV(0, 1, 0, 1, 0, 1);
        cube.GetComponent<SpriteRenderer>().color = c;
    }

    public override void Update()
    {
        // Step 3: Calculate forces for all cubes with proper dependencies
        var calculateSeparationJob = new CalculateSeparationJob(_cubeJobItemDatas, _cubeJobItems[0].SeparationDistance);
        var calculateSeparationJobHandle = calculateSeparationJob.Schedule(_cubeJobItemDatas.Length, 32);

        var calculateAlignmentJob = new CalculateAlignmentJob(_cubeJobItemDatas, _cubeJobItems[0].AlignmentDistance);
        var calculateAlignmentJobHandle = calculateAlignmentJob.Schedule(_cubeJobItemDatas.Length, 32, calculateSeparationJobHandle);

        var calculateCohesionJob = new CalculateCohesionJob(_cubeJobItemDatas, _cubeJobItems[0].AlignmentDistance);
        var calculateCohesionJobHandle = calculateCohesionJob.Schedule(_cubeJobItemDatas.Length, 32, calculateAlignmentJobHandle);

        var calculateTargetSeekJob = new CalculateTargetSeekJob(_targetPos, _cubeJobItemDatas);
        var calculateTargetSeekJobHandle = calculateTargetSeekJob.Schedule(_cubeJobItemDatas.Length, 32, calculateCohesionJobHandle);

        calculateTargetSeekJobHandle.Complete();

        // Step 2: Check if should stop for all cubes in parallel
        var shouldStopJob = new ShouldStopJob(_cubeJobItemDatas, _targetPos, _arrivalDistance, _cubeJobItems[0].AlignmentDistance, _cubeJobItems[0].AlignmentDistance);
        var shouldStopJobHandle = shouldStopJob.Schedule(_cubeJobItemDatas.Length, 32, calculateTargetSeekJobHandle);
        shouldStopJobHandle.Complete();

        // Step 4: Calculate total forces and new positions
        NativeArray<Vector3> totalForces = new NativeArray<Vector3>(_cubeJobItemDatas.Length, Allocator.TempJob);

        for (int i = 0; i < _cubeJobItemDatas.Length; i++)
        {
            var cubeData = _cubeJobItemDatas[i];
            if (!cubeData.IsShouldStop)
            {
                totalForces[i] = cubeData.Separation * SEPARATION_WEIGHT +
                               cubeData.Alignment * ALIGNMENT_WEIGHT +
                               cubeData.Cohesion * COHESION_WEIGHT +
                               cubeData.TargetSeek * TARGET_WEIGHT;
            }
            else
            {
                totalForces[i] = Vector3.zero;
            }
        }


        // Step 5: Apply forces and calculate new positions in parallel
        var newPosJob = new CalculateNewPosJob(totalForces, _cubeJobItemDatas, _cubeJobItems[0].MaxForce, _cubeJobItems[0].MaxSpeed, Time.deltaTime);
        var newPosJobHandle = newPosJob.Schedule(_cubeJobItemDatas.Length, 32, shouldStopJobHandle);
        newPosJobHandle.Complete();

        // Step 6: Update GameObject positions from NativeArray
        for (int i = 0; i < _cubeJobItems.Length; i++)
        {
            if (_cubeJobItems[i] != null)
            {
                _cubeJobItems[i].ApplyJobData(_cubeJobItemDatas[i]);
                _cubeJobItemDatas[i] = new CubeJobItemData(_cubeJobItems[i]);
            }
        }

        // Cleanup
        totalForces.Dispose();
    }

    void OnDestroy()
    {
        if (_cubeJobItemDatas.IsCreated)
            _cubeJobItemDatas.Dispose();
    }
}