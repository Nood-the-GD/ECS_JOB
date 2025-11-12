using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct PlayerTag : IComponentData
{
    // Empty
}

public class PlayerAuthoring : MonoBehaviour
{
    private class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
            AddComponent<InitializeCameraTargetTag>(entity);
            AddComponent<CameraTargetData>(entity);
        }
    }
}