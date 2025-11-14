using UnityEngine;
using System.Linq;

public class MouseInput : MonoBehaviour
{
    [Header("use Manager")]
    [SerializeField] private bool _isUseManager;
    [SerializeField] private CubeManager _cubeManager;
    private CubeSpawner _cubeSpawner;
    private PlayerInput _playerInput;

    void Awake()
    {
        _cubeSpawner = GameObject.FindObjectOfType<CubeSpawner>();
        // if (_cubeManager != null)
        // {
        //     _isUseManager = _cubeSpawner._isUseManager;
        // }

        _playerInput = new PlayerInput();
        _playerInput.Enable();
    }

    void Update()
    {
        var mousePos = _playerInput.PlayerInputMap.Mouse.ReadValue<Vector2>();
        if (_playerInput.PlayerInputMap.MousePress.IsPressed())
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

            if (_isUseManager && _cubeManager != null)
            {
                _cubeManager.SetTargetPos(worldPosition);
            }
            else
            {
                GameObject.FindObjectsByType<CubeItem>(FindObjectsSortMode.None).ToList().ForEach(cube =>
                {
                    cube.SetTargetPos(worldPosition);
                });
            }

        }
    }
}
