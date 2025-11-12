using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct AnimationSystem : ISystem
{
    private float _time;
    private float2 _gridPosition;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _time = 0f;
        _gridPosition = new float2(0f, 0f);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (animationData, gridPosition) in SystemAPI.Query<RefRW<AnimationData>, RefRW<AnimationGridPosition>>())
        {
            var material = animationData.ValueRW.Material.Value;
            var minMaxX = material.GetVector("_MinMaxX");
            var minMaxY = material.GetVector("_MinMaxY");
            var deltaTime = SystemAPI.Time.DeltaTime;
            _time += deltaTime;
            var time = 1f / animationData.ValueRW.FramePerSecond;
            var x = _gridPosition.x;
            var y = _gridPosition.y;
            if (_time >= time)
            {
                _time = 0f;
                x++;
                if (x > minMaxX.y)
                {
                    x = minMaxX.x;
                    y++;
                    if (y > minMaxY.y)
                    {
                        y = minMaxY.x;
                    }
                }
                _gridPosition = new float2(x, y);
            }

            gridPosition.ValueRW.Value = new Vector2(_gridPosition.x, _gridPosition.y);
        }
    }
}