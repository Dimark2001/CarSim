using Cysharp.Threading.Tasks;
using UnityEngine;

public class CarService : GSingleton
{
    public CarFacade Facade { get; private set; }
    
    public override async UniTask Initialize()
    {
        CreateCar(new Vector3(0f, 0.8f, 0));
        await UniTask.CompletedTask;
    }

    public override async UniTask AfterInitialize()
    {
        Facade.Input.Initialize();
        Disable();
        await UniTask.CompletedTask;
    }

    private void CreateCar(Vector3 vector3)
    {
        var prefab = GameResources.Prefabs.Car;
        Facade = Object.Instantiate(prefab, vector3, Quaternion.identity);
    }

    public void Disable()
    {
        Facade.Input.enabled = false;
        Facade.Camera.ResetPosition();
        Facade.Movement.Brakes();
    }
    
    public void Enable()
    {
        Facade.Input.enabled = true;
        Facade.Camera.ResetPosition();
    }
}