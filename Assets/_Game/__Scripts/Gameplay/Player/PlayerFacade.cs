using UnityEngine;

public class PlayerFacade : MonoBehaviour
{
    [field: SerializeField]
    public PlayerTrigger Trigger { get; private set; }
    
    [field: SerializeField]
    public PlayerMovement Movement { get; private set; }
    
    [field: SerializeField]
    public PlayerCamera Camera { get; private set; }
    
    [field: SerializeField]
    public PlayerInteraction Interaction { get; private set; }
    
    [field: SerializeField]
    public PlayerInputController Input { get; private set; }

    private void Reset()
    {
        Input = GetComponentInChildren<PlayerInputController>();
        Trigger = GetComponentInChildren<PlayerTrigger>();
        Movement = GetComponentInChildren<PlayerMovement>();
        Camera = GetComponentInChildren<PlayerCamera>();
        Interaction = GetComponentInChildren<PlayerInteraction>();
    }
}