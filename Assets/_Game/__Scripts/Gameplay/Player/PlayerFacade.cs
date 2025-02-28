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
}