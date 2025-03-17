using DG.Tweening;
using UnityEngine;

public class HoldingInteract : BaseInteract
{
    private bool _isInteract;

    protected override void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    protected override void OnDestroy()
    {
        transform?.DOKill();
    }

    public override void StartInteract()
    {
        base.StartInteract();
        _isInteract = true;
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rb.useGravity = false;
    }

    public override void StopInteract()
    {
        base.StopInteract();
        _isInteract = false;
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rb.useGravity = true;
    }

    protected void FixedUpdate()
    {
        if (!_isInteract)
        {
            return;
        }
        
        var holdingParent = G.Get<PlayerService>().Player.Interaction.HoldingParentTransform;
        var direction = (holdingParent.position - Rb.position).normalized;
        var f = Mathf.Lerp(0.1f, 20f, Vector3.Distance(Rb.position, holdingParent.position));
        if (Vector3.Distance(Rb.position, holdingParent.position) > 0.1f)
        {
            Rb.MovePosition(Rb.position + direction * (f * Time.fixedDeltaTime));
        }
        Rb.angularVelocity = Vector3.zero;
        Rb.linearVelocity = Vector3.zero;
        Rb.linearDamping = 0f;
    }
}