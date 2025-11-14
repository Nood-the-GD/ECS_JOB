using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using VInspector.Libs;

public partial struct PlayerAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var elapsedTime = SystemAPI.Time.ElapsedTime;

        var beginEcb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = beginEcb.CreateCommandBuffer(state.WorldUnmanaged);
        var physicWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (var (playerCooldownTimestamp, attackData, transform) in SystemAPI.Query<RefRW<PlayerCooldownExpirationTimestamp>, PlayerAttackData, LocalTransform>())
        {
            if (elapsedTime < playerCooldownTimestamp.ValueRO.Value) continue;

            // Attack logic
            var spawnPosition = transform.Position;
            var minDetectionPosition = spawnPosition - attackData.DetectionSize;
            var maxDetectionPosition = spawnPosition + attackData.DetectionSize;

            var aabbInput = new OverlapAabbInput
            {
                Aabb = new Aabb
                {
                    Min = minDetectionPosition,
                    Max = maxDetectionPosition
                },
                Filter = attackData.CollisionFilter
            };

            var overlapResults = new NativeList<int>(state.WorldUpdateAllocator);
            if (!physicWorldSingleton.OverlapAabb(aabbInput, ref overlapResults))
            {
                continue;
            }

            var closestDistance = float.MaxValue;
            var closestEnemyPosition = float3.zero;
            foreach (var result in overlapResults)
            {
                var curEnemyPosition = physicWorldSingleton.Bodies[result].WorldFromBody.pos;
                var distanceToPlayer = math.distance(spawnPosition.xy, curEnemyPosition.xy);

                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestEnemyPosition = curEnemyPosition;
                }
            }

            var vectorToClosestEnemy = closestEnemyPosition - spawnPosition;
            var angleToClosestEnemy = math.atan2(vectorToClosestEnemy.y, vectorToClosestEnemy.x);
            var spawnOrientation = quaternion.Euler(0f, 0f, angleToClosestEnemy); // Facing the closest enemy

            var newBullet = ecb.Instantiate(attackData.BulletPrefab); // Do not exist in the world yet b/c it will be instantiated in the next frame
            ecb.SetComponent(newBullet, LocalTransform.FromPositionRotationScale(spawnPosition, spawnOrientation, 1.3f));

            playerCooldownTimestamp.ValueRW.Value = elapsedTime + attackData.CooldownTime;
        }
    }
}