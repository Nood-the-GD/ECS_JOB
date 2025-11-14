using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputSystem : SystemBase
{
    private PlayerInput _playerInput;

    protected override void OnCreate()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
    }

    protected override void OnUpdate()
    {
        var moveDirection = (float2)_playerInput.PlayerInputMap.Move.ReadValue<Vector2>();
        foreach (var direction in SystemAPI.Query<RefRW<CharacterMoveDirection>>().WithAll<PlayerTag>())
        {
            direction.ValueRW.Value = moveDirection;
        }
    }
}