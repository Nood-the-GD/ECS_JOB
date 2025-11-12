using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private int _cubeCount = 10;
    [SerializeField] private Transform _cubePrefab;
    [Header("use Manager")]
    public bool _isUseManager;
    [SerializeField] private CubeManager _cubeManager;
    [SerializeField] private Transform _cubeManagedPrefab;
    List<CubeItem> _spawnCube = new List<CubeItem>();

    void Start()
    {
        Transform cube = null;
        if (_isUseManager)
        {
            for (int i = 0; i < _cubeCount; i++)
            {
                cube = Instantiate(_cubeManagedPrefab, transform.position + new Vector3(i * 0.1f, 0, 0), Quaternion.identity);
                _cubeManager.AddCube(cube.GetComponent<CubeItemManaged>());
                if (cube != null)
                {
                    RandomColor(cube);
                }
            }
            _cubeManager.CanStart = true;
        }
        else
        {
            for (int i = 0; i < _cubeCount; i++)
            {
                cube = Instantiate(_cubePrefab, transform.position + new Vector3(i * 0.1f, 0, 0), Quaternion.identity);
                foreach (var spawnCube in _spawnCube)
                {
                    spawnCube.AddCube(cube.transform);
                    cube.GetComponent<CubeItem>().AddCube(spawnCube.transform);
                }
                if (cube != null)
                {
                    RandomColor(cube);
                }
                _spawnCube.Add(cube.GetComponent<CubeItem>());
            }
        }

    }

    private void RandomColor(Transform cube)
    {
        var c = Random.ColorHSV(0, 1, 0, 1, 0, 1);
        cube.GetComponent<SpriteRenderer>().color = c;
    }
}