using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public partial struct CharacterMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (velocity, direction, speed) in SystemAPI.Query<RefRW<PhysicsVelocity>, CharacterMoveDirection, CharacterMoveSpeed>())
        {
            var moveStep = direction.Value * speed.Value;
            velocity.ValueRW.Linear = new float3(moveStep, 0);
        }
    }
}