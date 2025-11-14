using Unity.Entities;
using UnityEngine;

public struct DestroyEntityFlag : IComponentData, IEnableableComponent { }

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct DestroyEntitySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>(); // build in Unity buffer system
        var endEcb = ecb.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (_, entity) in SystemAPI.Query<DestroyEntityFlag>().WithEntityAccess())
        {
            if (SystemAPI.HasComponent<PlayerTag>(entity))
            {
                Debug.Log("Game Over");
            }
            endEcb.DestroyEntity(entity);
        }
    }
}