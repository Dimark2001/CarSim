using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [field: SerializeField]
    public Transform HoldingParentTransform { get; private set; }

    private InputController _inputController;
    private bool _isInteracting;
    private BaseInteract _currentInteractObject;

    private void Awake()
    {
        Root.RegisterComponent(this);
    }

    private void Start()
    {
        _inputController = Root.GetReference<InputController>();
        _inputController.OnInteractCall += OnInteractCall;
    }

    private void OnDestroy()
    {
        _inputController.OnInteractCall -= OnInteractCall;
    }

    private void OnInteractCall(InputAction.CallbackContext info)
    {
        if (info.performed)
        {
            _isInteracting = TryInteractFromRayCast();
        }
        if (info.canceled /*&& _isInteracting*/)
        {
            _currentInteractObject?.StopInteract();
        }
    }

    private bool TryInteractFromRayCast()
    {
        var interact = Root.GetReference<PlayerRayScanner>().CurrentInteractTarget;
        if (interact == null)
        {
            return false;
        }

        _currentInteractObject = interact;
        if (_currentInteractObject.IsInteractionBlocked)
        {
            return false;
        }

        _currentInteractObject.StartInteract();
        return true;
    }
}