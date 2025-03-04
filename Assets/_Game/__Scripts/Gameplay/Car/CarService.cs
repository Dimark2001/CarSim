using Cysharp.Threading.Tasks;
using UnityEngine;

public class CarService : GSingleton
{
    public CarFacade Car { get; private set; }
    
    public override async UniTask Initialize()
    {
        CreateCar(new Vector3(0f, 0.8f, 0));
        await UniTask.CompletedTask;
    }

    public override async UniTask AfterInitialize()
    {
        Car.Input.Initialize();
        Disable();
        await UniTask.CompletedTask;
    }

    private void CreateCar(Vector3 vector3)
    {
        var prefab = GameResources.Prefabs.Car;
        Car = Object.Instantiate(prefab, vector3, Quaternion.identity);
    }

    public void Disable()
    {
        Car.Input.enabled = false;
        Car.Camera.ResetPosition();
    }
    
    public void Enable()
    {
        Car.Input.enabled = true;
        Car.Camera.ResetPosition();
    }
}