using Unity.Cinemachine;
using UnityEngine;

public class GetInTheCar : BaseInteract, IRayScannerUiHelper
{
    [SerializeField]
    private CinemachineCamera _camera;
    
    public string UiLabel { get; set; }

    public override void StartInteract()
    {
        base.StartInteract();
        _camera.Priority = 21;
    }
}