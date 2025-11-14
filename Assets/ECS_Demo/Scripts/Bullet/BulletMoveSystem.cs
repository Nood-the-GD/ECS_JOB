using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct BulletMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (bulletData, transform) in SystemAPI.Query<BulletData, RefRW<LocalTransform>>())
        {
            transform.ValueRW.Position += transform.ValueRO.Right() * bulletData.MoveSpeed * deltaTime;
        }
    }
}