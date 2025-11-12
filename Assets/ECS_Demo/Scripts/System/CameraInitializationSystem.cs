using UnityEngine;
using Unity.Entities;
using Unity.Collections;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CameraInitializationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InitializeCameraTargetTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (CameraTarget.Instance == null) return;

        var cameraTargetTransform = CameraTarget.Instance.transform;

        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        foreach (var (cameraTarget, entity) in SystemAPI.Query<RefRW<CameraTargetData>>().WithAll<InitializeCameraTargetTag, PlayerTag>().WithEntityAccess())
        {
            cameraTarget.ValueRW.CameraTransform = cameraTargetTransform;
            ecb.RemoveComponent<InitializeCameraTargetTag>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}