using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct DamageThisFrameSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (currentHP, damageThisFrame, entity) in SystemAPI.Query<RefRW<CharacterCurrentHP>, DynamicBuffer<DamageThisFrame>>().WithPresent<DestroyEntityFlag>().WithEntityAccess())
        {
            if (damageThisFrame.IsEmpty) continue;

            foreach (var damage in damageThisFrame)
            {
                currentHP.ValueRW.Value -= damage.Value;
            }

            damageThisFrame.Clear();
            if (currentHP.ValueRO.Value <= 0)
            {
                SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, true);
            }
        }
    }
}