using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class CarSeat : BaseInteract
{
    [SerializeField]
    private CinemachineCamera _camera;

    [SerializeField]
    private Transform _exitTransform;

    private PlayerService _playerService;
    private CarService _carService;
    private CameraService _cameraService;

    private void Start()
    {
        _playerService = G.Get<PlayerService>();
        _carService = G.Get<CarService>();
        _cameraService = G.Get<CameraService>();
    }
    
    public override async void StartInteract()
    {
        base.StartInteract();
        _camera.Priority = 21;
        
        _playerService.Player.Input.enabled = false;
        _playerService.Player.Visual.Hide();
        await UniTask.WaitForSeconds(_cameraService.Camera.Brain.DefaultBlend.Time);
        _playerService.Disable();
        
        _carService.Enable();
    }

    public async void Exit()
    {
        _camera.Priority = 10;
       
        _carService.Car.Input.enabled = false;
        _playerService.Player.Movement.Teleport(_exitTransform.position);
        await UniTask.WaitForSeconds(_cameraService.Camera.Brain.DefaultBlend.Time);
        _carService.Disable();
        
        _playerService.Enable();
    }
}