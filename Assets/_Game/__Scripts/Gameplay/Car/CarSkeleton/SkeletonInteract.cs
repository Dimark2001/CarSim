using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SkeletonInteract : BaseInteract
{
    [field: SerializeField]
    public SkeletonStats Stats { get; private set; }
    
    [SerializeField]
    protected MeshCollider _meshCollider;

    [SerializeField]
    protected float _sphereRadius = 0.5f;

    protected Transform HoldingParent;
    protected CarService CarService;
    protected SCState ScState = SCState.Car;


    protected override void Start()
    {
        HoldingParent = G.Get<PlayerService>().Player.Interaction.HoldingParentTransform;
        CarService = G.Get<CarService>();

        Rb ??= gameObject.AddComponent<Rigidbody>();
        Rb.interpolation = RigidbodyInterpolation.Interpolate;
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rb.isKinematic = true;
        Rb.useGravity = false;
        CarState();
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
        Rb.isKinematic = false;
        Rb.useGravity = false;

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
        Stats.ResetHp();
        Rb.isKinematic = true;
        Rb.useGravity = true;
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
        Rb.useGravity = true;

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

            Rb.angularVelocity = Vector3.zero;
            Rb.linearVelocity = Vector3.zero;
            Rb.linearDamping = 0f;
        }

        if (Rb && !Rb.isKinematic)
        {
            var v = Rb.linearVelocity;
            v.x = Mathf.Clamp(v.x, -10f, 10f);
            v.y = Mathf.Clamp(v.y, -10f, 10f);
            v.z = Mathf.Clamp(v.z, -10f, 10f);
            Rb.linearVelocity = v;
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

                Stats.Damage(speed);
            }

            if (Stats.isAlive == false)
            {
                CarService.Facade.Skeleton.RemoveComponent(this);
                DownState();
            }
        }
    }

    protected virtual void Reset()
    {
        if (UiLabel == "")
            UiLabel = $"Grab {gameObject.name}";
        _meshCollider = GetComponent<MeshCollider>();
    }

    [Serializable]
    public class SkeletonStats
    {
        [SerializeField]
        private float _maxHp = 10f;

        private float _hp = 10f;

        public void ResetHp() => _hp = _maxHp;

        public void Damage(float damage) => _hp -= damage;
        
        public bool isAlive => _hp > 0;
    }
}