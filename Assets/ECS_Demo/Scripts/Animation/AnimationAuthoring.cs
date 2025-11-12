using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct AnimationData : IComponentData
{
    public UnityObjectRef<Material> Material;
    public float FramePerSecond;
}

[MaterialProperty("_GridPosition")]
public struct AnimationGridPosition : IComponentData
{
    public Vector2 Value;
}


public class AnimationAuthoring : MonoBehaviour
{
    [SerializeField] private Material _material;

    public class Baker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AnimationData
            {
                Material = authoring._material,
                FramePerSecond = 3
            });
            AddComponent(entity, new AnimationGridPosition { Value = new Vector2(0, 0) });
        }
    }
}