using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public struct PlayerTag : IComponentData
{
    // Empty
}

public struct PlayerCooldownExpirationTimestamp : IComponentData
{
    public double Value;
}

public struct PlayerAttackData : IComponentData
{
    public Entity BulletPrefab;
    public float CooldownTime;
    public float3 DetectionSize;
    public CollisionFilter CollisionFilter;
}

public class PlayerAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float CooldownTime = 1f;
    public float DetectionSize = 1f;

    private class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
            AddComponent<InitializeCameraTargetTag>(entity);
            AddComponent<CameraTargetData>(entity);

            var enemyLayer = LayerMask.NameToLayer("Enemy");
            var enemyLayerMask = (uint)math.pow(2, enemyLayer);

            var attackCollisionFilter = new CollisionFilter
            {
                BelongsTo = uint.MaxValue,
                CollidesWith = enemyLayerMask,
            };

            AddComponent(entity, new PlayerAttackData
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                CooldownTime = authoring.CooldownTime,
                DetectionSize = new float3(authoring.DetectionSize),
                CollisionFilter = attackCollisionFilter
            });
            AddComponent(entity, new PlayerCooldownExpirationTimestamp
            {
                Value = 0
            });
        }
    }
}