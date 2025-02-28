using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [field: SerializeField]
    public Transform HoldingParentTransform { get; private set; }

    private PlayerInputController _playerInputController;
    private bool _isInteracting;
    private BaseInteract _currentInteractObject;

    public void Interact()
    {
        _isInteracting = TryInteractFromRayCast();
    }
    
    public void StopInteract()
    {
        if (_isInteracting)
        {
            _currentInteractObject?.StopInteract();
        }
    }

    private bool TryInteractFromRayCast()
    {
        var interact = G.Get<PlayerService>().Player.Interaction._currentInteractObject;
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