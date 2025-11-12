using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct CharacterInitialization : IComponentData, IEnableableComponent { }

public struct CharacterMoveDirection : IComponentData
{
    public float2 Value;
}

public struct CharacterMoveSpeed : IComponentData
{
    public float Value;
}

public class CharacterAuthoring : MonoBehaviour
{
    public float MoveSpeed = 10f;

    private class Baker : Baker<CharacterAuthoring>
    {
        public override void Bake(CharacterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<CharacterInitialization>(entity);
            AddComponent(entity, new CharacterMoveDirection { Value = new float2(0, 0) });
            AddComponent(entity, new CharacterMoveSpeed
            {
                Value = authoring.MoveSpeed
            });
        }
    }
}
