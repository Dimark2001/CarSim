using Unity.Cinemachine;
using UnityEngine;

public class CarSeat : BaseInteract, IInteractable
{
    [SerializeField]
    private CinemachineCamera _camera;
    
    [field: SerializeField]
    public string UiLabel { get; set; }

    public override void StartInteract()
    {
        base.StartInteract();
        _camera.Priority = 21;
    }
}