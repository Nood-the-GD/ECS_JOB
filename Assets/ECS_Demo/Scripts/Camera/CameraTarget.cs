using Unity.Entities;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public static CameraTarget Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}

public struct CameraTargetData : IComponentData
{
    public UnityObjectRef<Transform> CameraTransform;
}

public struct InitializeCameraTargetTag : IComponentData { }
