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

public struct CharacterMaxHP : IComponentData
{
    public int Value;
}

public struct CharacterCurrentHP : IComponentData
{
    public int Value;
}

public struct DamageThisFrame : IBufferElementData
{
    public int Value;
}

public class CharacterAuthoring : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public int MaxHP = 100;

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
            AddComponent(entity, new CharacterMaxHP { Value = authoring.MaxHP });
            AddComponent(entity, new CharacterCurrentHP { Value = authoring.MaxHP });
            AddBuffer<DamageThisFrame>(entity);
            AddComponent<DestroyEntityFlag>(entity);
            SetComponentEnabled<DestroyEntityFlag>(entity, false);
        }
    }
}
