using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraService : GSingleton
{
    public CameraFacade Camera { get; private set; }

    public override async UniTask Initialize()
    {
        CreateCamera();
        await UniTask.CompletedTask;
    }

    public override async UniTask AfterInitialize()
    {
        await UniTask.CompletedTask;
    }

    private void CreateCamera()
    {
        var prefab = GameResources.Prefabs.MainCamera;
        Camera = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
    }
}