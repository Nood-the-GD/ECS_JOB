using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;
using Unity.Jobs;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateBefore(typeof(AfterPhysicsSystemGroup))]
public partial struct EnemyAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var elapsedTime = SystemAPI.Time.ElapsedTime;
        foreach (var (cooldownExpirationTimestamp, enabledRefRW) in SystemAPI.Query<EnemyCooldownExpirationTimestamp, EnabledRefRW<EnemyCooldownExpirationTimestamp>>())
        {
            if (cooldownExpirationTimestamp.Value > elapsedTime) continue;
            enabledRefRW.ValueRW = false;
        }

        var attackJob = new EnemyAttackJob
        {
            ElapsedTime = elapsedTime,
            PlayerTagLookup = SystemAPI.GetComponentLookup<PlayerTag>(true),
            EnemyAttackDataLookup = SystemAPI.GetComponentLookup<EnemyAttackData>(true),
            CooldownLookup = SystemAPI.GetComponentLookup<EnemyCooldownExpirationTimestamp>(),
            DamageThisFrameLookup = SystemAPI.GetBufferLookup<DamageThisFrame>(),
        };

        var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = attackJob.Schedule(simulationSingleton, state.Dependency);
    }
}


[BurstCompile]
public partial struct EnemyAttackJob : ICollisionEventsJob
{
    [ReadOnly] public ComponentLookup<PlayerTag> PlayerTagLookup;
    [ReadOnly] public ComponentLookup<EnemyAttackData> EnemyAttackDataLookup;
    public ComponentLookup<EnemyCooldownExpirationTimestamp> CooldownLookup;
    public BufferLookup<DamageThisFrame> DamageThisFrameLookup;
    public double ElapsedTime;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity playerEntity;
        Entity enemyEntity;

        if (PlayerTagLookup.HasComponent(collisionEvent.EntityA) && EnemyAttackDataLookup.HasComponent(collisionEvent.EntityB))
        {
            playerEntity = collisionEvent.EntityA;
            enemyEntity = collisionEvent.EntityB;
        }
        else if (PlayerTagLookup.HasComponent(collisionEvent.EntityB) && EnemyAttackDataLookup.HasComponent(collisionEvent.EntityA))
        {
            playerEntity = collisionEvent.EntityB;
            enemyEntity = collisionEvent.EntityA;
        }
        else
        {
            return;
        }

        if (CooldownLookup.IsComponentEnabled(enemyEntity))
        {
            return;
        }

        var attackData = EnemyAttackDataLookup[enemyEntity];
        CooldownLookup[enemyEntity] = new EnemyCooldownExpirationTimestamp
        {
            Value = ElapsedTime + attackData.CooldownTime
        };
        CooldownLookup.SetComponentEnabled(enemyEntity, true);
        DamageThisFrameLookup[playerEntity].Add(new DamageThisFrame
        {
            Value = attackData.HitPoint
        });
    }
}