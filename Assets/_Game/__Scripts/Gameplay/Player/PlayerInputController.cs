using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour, InputSystem.IPlayerActions
{
    private PlayerService _ps;
    private Vector2 _moveDirection;
    private Vector2 _cameraDirection;
    private InputSystem _inputSystem;

    private void Start()
    {
        _ps = G.Get<PlayerService>();
    }

    public void OnEnable()
    {
        _inputSystem ??= new InputSystem();
        _inputSystem.Enable();
        _inputSystem.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        _inputSystem.Player.RemoveCallbacks(this);
        _inputSystem.Disable();
    }

    private void FixedUpdate()
    {
        _ps.Player.Movement.Move(_moveDirection);
    }

    private void LateUpdate()
    {
        _ps.Player.Camera.Move(_cameraDirection);
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
}