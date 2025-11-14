using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemySpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

        foreach (var (spawnData, spawnState) in SystemAPI.Query<EnemySpawnData, RefRW<EnemySpawnState>>())
        {
            spawnState.ValueRW.SpawnTimer -= deltaTime;
            if (spawnState.ValueRO.SpawnTimer > 0f) continue;
            spawnState.ValueRW.SpawnTimer = spawnData.SpawnInterval;

            var random = spawnState.ValueRO.Random;
            var spawnAngle = spawnState.ValueRW.Random.NextFloat(0f, math.TAU); // Get angle in radians, angle in a circle is 2 * PI (TAU)
            var spawnPoint = new float3(math.sin(spawnAngle), math.cos(spawnAngle), 0f) * spawnData.SpawnDistance;
            spawnPoint += playerPosition;

            var newEnemy = ecb.Instantiate(spawnData.EnemyPrefab);
            ecb.SetComponent(newEnemy, LocalTransform.FromPosition(spawnPoint));

        }
    }
}