using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MouseInput : MonoBehaviour
{
    [Header("use Manager")]
    private bool _isUseManager;
    [SerializeField] private CubeManager _cubeManager;
    private CubeSpawner _cubeSpawner;

    void Awake()
    {
        _cubeSpawner = GameObject.FindObjectOfType<CubeSpawner>();
        if (_cubeManager == null)
            _isUseManager = _cubeSpawner._isUseManager;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // if (_isUseManager && _cubeManager != null)
            // {
            //     _cubeManager.SetTargetPos(mousePos);
            // }
            // else
            // {
            //     GameObject.FindObjectsByType<CubeItem>(FindObjectsSortMode.None).ToList().ForEach(cube =>
            //     {
            //         cube.SetTargetPos(mousePos);
            //     });
            // }

            _cubeManager.SetTargetPos(mousePos);
        }
    }
}
