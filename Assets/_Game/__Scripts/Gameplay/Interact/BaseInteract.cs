using System;
using UnityEngine;

public abstract class BaseInteract : MonoBehaviour
{
    public event Action OnStartInteractCall;
    public event Action OnStopInteractCall;

    [field: SerializeField]
    public string UiLabel { get; set; }

    [field: SerializeField]
    public bool IsInteractionBlocked { get; protected set; }

    public bool IsInInteraction { get; protected set; }

    protected float RotationSpeed = 0.2f;
    protected Rigidbody Rb;

    protected virtual void Start()
    {
    }

    protected virtual void OnDestroy()
    {
    }

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

    public void RotateObject(Vector2 mouseDelta)
    {
        var torque = new Vector3(mouseDelta.y, -mouseDelta.x, 0) * RotationSpeed;
        Rb.MoveRotation(Rb.rotation * Quaternion.Euler(new Vector3(torque.x, 0, torque.y)));
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