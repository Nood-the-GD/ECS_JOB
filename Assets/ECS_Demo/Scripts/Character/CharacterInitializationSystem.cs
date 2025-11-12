using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CharacterInitializationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (mass, initialization) in SystemAPI.Query<RefRW<PhysicsMass>, EnabledRefRW<CharacterInitialization>>())
        {
            mass.ValueRW.InverseInertia = float3.zero;
            initialization.ValueRW = false;
        }
    }
}