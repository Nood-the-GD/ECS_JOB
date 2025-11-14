using Unity.Entities;
using UnityEngine;

public struct BulletData : IComponentData
{
    public float MoveSpeed;
    public int Damage;
}

public class BulletAuthoring : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public int Damage = 10;

    private class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletData { MoveSpeed = authoring.MoveSpeed, Damage = authoring.Damage });
            AddComponent<DestroyEntityFlag>(entity);
            SetComponentEnabled<DestroyEntityFlag>(entity, false);
        }
    }
}