using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour, InputSystem.IPlayerActions
{
    private PlayerService _ps;
    private InputSystem _inputSystem;
    
    private Vector2 _moveDirection;
    private Vector2 _cameraDirection;
    private float _scrollValue;

    private bool _isInitialize;
    private bool _isRotate;

    public void Initialize()
    {
        _isInitialize = true;
        _ps = G.Get<PlayerService>();
        _inputSystem = new InputSystem();
        _inputSystem.Enable();
        _inputSystem.Player.SetCallbacks(this);
    }

    public void OnEnable()
    {
        if (!_isInitialize)
        {
            return;
        }

        _inputSystem.Enable();
        _inputSystem.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        _inputSystem?.Player.RemoveCallbacks(this);
        _inputSystem?.Disable();
    }

    private void FixedUpdate()
    {
        _ps.Player.Movement.Move(_moveDirection);
    }

    private void LateUpdate()
    {
        _ps.Player.Camera.Move(_cameraDirection);

        if (_isRotate)
        {
            var baseInteract = _ps.Player.Interaction.CurrentInteractObject;
            if (baseInteract)
            {
                baseInteract.RotateObject(_cameraDirection);
            }
            else
            {
                _isRotate = false;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext c)
    {
        _moveDirection = c.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext c)
    {
        _cameraDirection = c.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            _ps.Player.Interaction.Interact();
        }

        if (c.canceled)
        {
            _ps.Player.Interaction.StopInteract();
        }
    }

    public void OnCrouch(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            _ps.Player.Movement.Wrapper.Crouch.Perform();
        }

        if (c.canceled)
        {
            _ps.Player.Movement.Wrapper.Crouch.Cancel();
        }
    }

    public void OnJump(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            _ps.Player.Movement.Wrapper.Jump.Perform();
        }
    }

    public void OnSprint(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            _ps.Player.Movement.Run(true);
        }

        if (c.canceled)
        {
            _ps.Player.Movement.Run(false);
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (_ps.Player.Interaction.CurrentInteractObject != null && context.performed)
        {
            _ps.CameraState(false);
            _ps.MovementState(false);
            _isRotate = true;
        }

        if (context.canceled)
        {
            _ps.CameraState(true);
            _ps.MovementState(true);
            _isRotate = false;
        }
    }

    public void OnMouseScroll(InputAction.CallbackContext context)
    {
        _scrollValue = context.ReadValue<Vector2>().y;
    }
}