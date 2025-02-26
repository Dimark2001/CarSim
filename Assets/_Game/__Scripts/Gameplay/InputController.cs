using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _playerInput;

    private InputAction _lookAction;
    private InputAction _moveAction;
    private InputAction _runInputAction;
    private InputAction _crouchAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;
    public event Action<InputAction.CallbackContext> OnInteractCall;

    private void Awake()
    {
        Root.RegisterComponent(this);

        _lookAction = _playerInput.actions.FindAction("Look");
        _moveAction = _playerInput.actions.FindAction("Move");
        _runInputAction = _playerInput.actions.FindAction("Sprint");
        _crouchAction = _playerInput.actions.FindAction("Crouch");
        _jumpAction = _playerInput.actions.FindAction("Jump");
        _interactAction = _playerInput.actions.FindAction("Interact");

        _interactAction.performed += o => OnInteractCall?.Invoke(o);
        _interactAction.canceled += o => OnInteractCall?.Invoke(o);
    }

    private void OnDestroy()
    {
        _jumpAction.performed -= OnInteractCall;
        _jumpAction.canceled -= OnInteractCall;
    }

    public Vector2 Look()
    {
        return _lookAction.ReadValue<Vector2>();
    }

    public Vector2 Move()
    {
        return _moveAction.ReadValue<Vector2>();
    }

    public bool IsRun()
    {
        return _runInputAction.IsPressed();
    }

    public bool IsCrouch()
    {
        return _crouchAction.IsPressed();
    }

    public bool IsJump()
    {
        return _jumpAction.IsPressed();
    }
}