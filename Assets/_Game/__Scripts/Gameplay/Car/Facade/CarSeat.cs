using Unity.Cinemachine;
using UnityEngine;

public class CarSeat : BaseInteract
{
    [SerializeField]
    private CinemachineCamera _camera;

    public override void StartInteract()
    {
        base.StartInteract();
        _camera.Priority = 21;
        G.Get<PlayerService>().Disable();
        G.Get<CarService>().Enable();
    }

    public void Exit()
    {
        _camera.Priority = 10;
        G.Get<CarService>().Disable();
        G.Get<PlayerService>().Enable();
    }
}