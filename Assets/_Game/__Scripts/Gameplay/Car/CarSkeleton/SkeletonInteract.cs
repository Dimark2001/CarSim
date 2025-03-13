using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SkeletonInteract : BaseInteract
{
    [SerializeField]
    protected MeshCollider _meshCollider;

    [SerializeField]
    protected float _sphereRadius = 0.5f;

    protected Transform HoldingParent;
    protected CarService CarService;
    protected SCState ScState = SCState.Car;

    [SerializeField]
    protected float _maxHp = 10f;

    [SerializeField]
    protected float Hp = 10f;

    protected override void Start()
    {
        HoldingParent = G.Get<PlayerService>().Player.Interaction.HoldingParentTransform;
        CarService = G.Get<CarService>();

        Rb ??= gameObject.AddComponent<Rigidbody>();
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rb.isKinematic = true;
        CarState();
        Hp = _maxHp;
    }

    protected override void OnDestroy()
    {
        transform?.DOKill();
    }

    public override void StartInteract()
    {
        InteractState();
    }

    public override void StopInteract()
    {
        base.StopInteract();
        TryAttachToCar();
    }

    public void AddForce(Vector3 force)
    {
        DownState();
        Rb.AddForce(force * 10f, ForceMode.Force);
    }

    protected virtual void TryAttachToCar()
    {
        var colliders = Physics.OverlapSphere(transform.position, _sphereRadius, 1 << LayerMask.NameToLayer("Skeleton"))
            .ToList();

        foreach (var c in colliders)
        {
            var q = c.GetComponentInParent<CarSkeleton>();
            if (q.TryConnectToBracing(this, c.transform))
            {
                CarState();
                return;
            }
        }

        DownState();
    }

    protected virtual async void InteractState()
    {
        if (ScState == SCState.Car)
        {
            CarService.Facade.Skeleton.RemoveComponent(this);
        }

        Rb ??= gameObject.AddComponent<Rigidbody>();
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rb.isKinematic = true;

        transform.parent = null;

        ScState = SCState.Interact;
        _meshCollider.isTrigger = true;
        await UniTask.WaitForSeconds(0.1f);
        _meshCollider.isTrigger = false;
    }

    protected virtual void CarState()
    {
        _meshCollider.isTrigger = true;
        ScState = SCState.Car;
        Hp = _maxHp;
    }

    protected virtual void DownState()
    {
        if (ScState == SCState.Car)
        {
            CarService.Facade.Skeleton.RemoveComponent(this);
        }

        Rb ??= gameObject.AddComponent<Rigidbody>();
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        transform.parent = null;
        Rb.isKinematic = false;

        _meshCollider.isTrigger = false;
        ScState = SCState.Down;
    }


    protected virtual void FixedUpdate()
    {
        if (ScState == SCState.Interact)
        {
            var direction = (HoldingParent.position - Rb.position).normalized;
            var f = Mathf.Lerp(0.1f, 20f, Vector3.Distance(Rb.position, HoldingParent.position));
            if (Vector3.Distance(Rb.position, HoldingParent.position) > 0.1f)
            {
                Rb.MovePosition(Rb.position + direction * (f * Time.fixedDeltaTime));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ScState != SCState.Car)
        {
            return;
        }

        var layer = LayerHelper.NamesToLayerMask(new[] { "Environment", "Interactable", });
        if (layer.Contains(other.gameObject.layer))
        {
            var otherRb = other.GetComponent<Rigidbody>();

            if (otherRb != null && !otherRb.isKinematic && Rb != null)
            {
                var relativeVelocity = otherRb.linearVelocity - Rb.linearVelocity;
                if (relativeVelocity.magnitude == 0)
                {
                    relativeVelocity = CarService.Facade.Movement._carRigidbody.linearVelocity - Rb.linearVelocity;
                }

                var speed = Mathf.Abs(relativeVelocity.magnitude);

                Hp -= speed;
            }

            //Debug.Log(Hp);
            if (Hp <= 0)
            {
                CarService.Facade.Skeleton.RemoveComponent(this);
                DownState();
            }
        }
    }

    protected virtual void Reset()
    {
        if(UiLabel == "")
            UiLabel = $"Grab {gameObject.name}";
        _meshCollider = GetComponent<MeshCollider>();
    }
}