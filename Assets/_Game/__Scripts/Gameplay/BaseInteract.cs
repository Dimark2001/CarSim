using System;
using UnityEngine;

public abstract class BaseInteract : MonoBehaviour
{
    public event Action OnStartInteractCall;
    public event Action OnStopInteractCall;

    [field: SerializeField]
    public bool IsInteractionBlocked { get; protected set; }

    public bool IsInInteraction { get; protected set; }

    public virtual void StartInteract()
    {
        OnStartInteractCall?.Invoke();
        IsInInteraction = true;
    }

    public virtual void StopInteract()
    {
        OnStopInteractCall?.Invoke();
        IsInInteraction = false;
    }

    public void UnBlockInteraction()
    {
        IsInteractionBlocked = false;
    }

    public void BlockInteraction()
    {
        IsInteractionBlocked = true;
    }
}