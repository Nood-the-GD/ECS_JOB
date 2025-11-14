using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public partial struct EnemyMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>(); // Require player tag to be present in the world
    }

    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position.xy;
        var job = new EnemyMoveToPlayerJob
        {
            TargetPosition = playerPosition
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
[WithAll(typeof(EnemyTag))]
public partial struct EnemyMoveToPlayerJob : IJobEntity
{
    public float2 TargetPosition;

    private void Execute(ref CharacterMoveDirection direction, in LocalTransform transform)
    {
        var toPlayerDirection = TargetPosition - transform.Position.xy;
        direction.Value = math.normalize(toPlayerDirection);
    }
}

