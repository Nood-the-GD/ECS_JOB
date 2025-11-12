using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))] // Have this b/c we use LocalToWorld
public partial struct CameraMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (cameraTargetData, playerTransform) in SystemAPI.Query<CameraTargetData, LocalToWorld>().WithAll<PlayerTag>().WithNone<InitializeCameraTargetTag>())
        {
            var cameraTransform = cameraTargetData.CameraTransform.Value;
            if (cameraTransform == null) continue;

            var playerPos = playerTransform.Position;
            cameraTransform.position = playerPos;
        }
    }
}