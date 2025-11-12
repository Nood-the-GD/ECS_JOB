using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct PlayerAnimationSystem : ISystem
{
    float _lastDirection;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (direction, transform) in SystemAPI.Query<CharacterMoveDirection, RefRW<LocalToWorld>>())
        {
            if (direction.Value.x == 0)
            {
                if (_lastDirection == 0) continue;
                transform.ValueRW.Value.c0.x = _lastDirection;
                continue;
            }
            if (direction.Value.x < 0)
            {
                transform.ValueRW.Value.c0.x = -1;
            }
            else
            {
                transform.ValueRW.Value.c0.x = 1;
            }
            _lastDirection = transform.ValueRW.Value.c0.x;
        }
    }
}