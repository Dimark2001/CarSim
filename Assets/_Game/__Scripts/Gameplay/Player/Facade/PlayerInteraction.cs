using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    [field: SerializeField]
    public Transform HoldingParentTransform { get; private set; }
    public BaseInteract CurrentInteractObject;

    private PlayerInputController _playerInputController;
    
    private bool _isInteracting;

    public void Interact()
    {
        _isInteracting = TryInteractWithCurrent();
    }
    
    public void StopInteract()
    {
        if (_isInteracting)
        {
            CurrentInteractObject?.StopInteract();
            CurrentInteractObject = null;
        }
    }

    private bool TryInteractWithCurrent()
    {
        var interact = G.Get<PlayerService>().Player.Trigger.CurrentInteractTarget;
        if (interact == null)
        {
            return false;
        }

        CurrentInteractObject = interact;
        if (CurrentInteractObject.IsInteractionBlocked)
        {
            return false;
        }

        CurrentInteractObject.StartInteract();
        return true;
    }
}