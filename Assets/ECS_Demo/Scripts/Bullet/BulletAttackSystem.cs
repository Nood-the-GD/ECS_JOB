using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateBefore(typeof(AfterPhysicsSystemGroup))]
public partial struct BulletAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var attackJob = new BulletAttackJob
        {
            BulletDataLookup = SystemAPI.GetComponentLookup<BulletData>(true),
            EnemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            DamageThisFrameLookup = SystemAPI.GetBufferLookup<DamageThisFrame>(),
            DestroyEntityFlagLookup = SystemAPI.GetComponentLookup<DestroyEntityFlag>(),
        };

        var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = attackJob.Schedule(simulationSingleton, state.Dependency);
    }
}

public partial struct BulletAttackJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<BulletData> BulletDataLookup;
    [ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;
    public BufferLookup<DamageThisFrame> DamageThisFrameLookup;
    public ComponentLookup<DestroyEntityFlag> DestroyEntityFlagLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity bulletEntity;
        Entity enemyEntity;

        if (BulletDataLookup.HasComponent(triggerEvent.EntityA) && EnemyLookup.HasComponent(triggerEvent.EntityB))
        {
            bulletEntity = triggerEvent.EntityA;
            enemyEntity = triggerEvent.EntityB;
        }
        else if (BulletDataLookup.HasComponent(triggerEvent.EntityB) && EnemyLookup.HasComponent(triggerEvent.EntityA))
        {
            bulletEntity = triggerEvent.EntityB;
            enemyEntity = triggerEvent.EntityA;
        }
        else
        {
            return;
        }

        var bulletData = BulletDataLookup[bulletEntity];
        DamageThisFrameLookup[enemyEntity].Add(new DamageThisFrame
        {
            Value = bulletData.Damage
        });
        DestroyEntityFlagLookup.SetComponentEnabled(bulletEntity, true);
    }
}