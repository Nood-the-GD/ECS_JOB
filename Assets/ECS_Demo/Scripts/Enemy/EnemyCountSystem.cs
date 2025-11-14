using UnityEngine;
using Unity.Entities;
using System.Linq;

public partial struct EnemyCountSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var enemyCount = 0;
        foreach (var _ in SystemAPI.Query<EnemyTag>())
        {
            enemyCount++;
        }
        UIManager.Instance.UpdateEnemyCount(enemyCount);
    }
}