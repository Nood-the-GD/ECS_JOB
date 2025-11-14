using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct EnemySpawnData : IComponentData
{
    public Entity EnemyPrefab;
    public float SpawnInterval;
    public float SpawnDistance;
}

public struct EnemySpawnState : IComponentData
{
    public float SpawnTimer;
    public Random Random;
}

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnInterval = 1f;
    public float SpawnDistance = 10f;
    public uint RandomSeed = 0;

    private class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EnemySpawnData
            {
                EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = authoring.SpawnInterval,
                SpawnDistance = authoring.SpawnDistance
            });
            AddComponent(entity, new EnemySpawnState
            {
                SpawnTimer = 0,
                Random = new Random(seed: authoring.RandomSeed)
            });
        }
    }
}