using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [field: SerializeField]
    public Transform HoldingParentTransform { get; private set; }

    private PlayerInputController _playerInputController;
    private BaseInteract _currentInteractObject;
    
    private bool _isInteracting;

    public void Interact()
    {
        _isInteracting = TryInteractWithCurrent();
    }
    
    public void StopInteract()
    {
        if (_isInteracting)
        {
            _currentInteractObject?.StopInteract();
        }
    }

    private bool TryInteractWithCurrent()
    {
        var interact = G.Get<PlayerService>().Player.Trigger.CurrentInteractTarget;
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