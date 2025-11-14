using Unity.Entities;
using UnityEngine;

public struct EnemyTag : IComponentData
{
    // Empty
}

public struct EnemyAttackData : IComponentData
{
    public int HitPoint;
    public float CooldownTime;
}

public struct EnemyCooldownExpirationTimestamp : IComponentData, IEnableableComponent
{
    public double Value;
}

[RequireComponent(typeof(CharacterAuthoring))]
public class EnemyAuthoring : MonoBehaviour
{
    public int HitPoint = 10;
    public float CooldownTime = 1f;

    private class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<EnemyTag>(entity);
            AddComponent(entity, new EnemyAttackData
            {
                HitPoint = authoring.HitPoint,
                CooldownTime = authoring.CooldownTime
            });
            AddComponent<EnemyCooldownExpirationTimestamp>(entity);
            SetComponentEnabled<EnemyCooldownExpirationTimestamp>(entity, false);
        }
    }
}