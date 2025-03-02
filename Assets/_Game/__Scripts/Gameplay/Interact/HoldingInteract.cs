using DG.Tweening;
using UnityEngine;

public class HoldingInteract : BaseInteract
{
    private Transform _parent;
    private Transform _holdingParent;
    private void Start()
    {
        _parent = transform.parent;
        _holdingParent = G.Get<PlayerService>().Player.Interaction.HoldingParentTransform;
    }

    private void OnDestroy()
    {
        transform?.DOKill();
    }

    public override void StartInteract()
    {
        transform.parent = _holdingParent;
        transform.DOMove(_holdingParent.position, 0.5f);
        transform.DORotate(_holdingParent.rotation.eulerAngles, 0.5f).OnComplete( () =>  
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        });
    }

    public override void StopInteract()
    {
        base.StopInteract();
    }
}