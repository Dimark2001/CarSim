using UnityEngine;
using UnityEngine.InputSystem;

public class CarInputController : MonoBehaviour, InputSystem.ICarActions
{
    private bool _isInitialize;

    private CarService _cs;
    private Vector2 _cameraDirection;
    private InputSystem _carInput;

    public void Initialize()
    {
        _cs = G.Get<CarService>();
        _carInput = new InputSystem();
        _carInput.Car.SetCallbacks(this);
        _carInput.Enable();

        _isInitialize = true;
    }

    private void OnEnable()
    {
        if (!_isInitialize)
        {
            return;
        }

        _carInput.Enable();
        _carInput.Car.SetCallbacks(this);
    }

    private void OnDisable()
    {
        _carInput?.Car.RemoveCallbacks(this);
        _carInput?.Disable();
    }

    private void FixedUpdate()
    {
        _cs.Facade.Movement.Move();
    }

    private void LateUpdate()
    {
        _cs.Facade.Camera.Move(_cameraDirection);
    }

    public void OnMove(InputAction.CallbackContext c)
    {
        _cs.Facade.Movement.MoveDirection = c.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext c)
    {
        _cameraDirection = c.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            _cs.Facade.Seat.Exit();
        }
    }

    public void OnHandbrake(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _cs.Facade.Movement.Handbrake();
            _cs.Facade.Movement.IsHandbrake = true;
        }

        if (context.canceled)
        {
            _cs.Facade.Movement.RecoverTraction();
            _cs.Facade.Movement.IsHandbrake = false;
        }
    }
}