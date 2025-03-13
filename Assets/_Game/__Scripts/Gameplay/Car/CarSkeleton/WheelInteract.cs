using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WheelInteract : SkeletonInteract
{
    [field: SerializeField]
    public WheelCollider WheelCollider { get; private set; }

    [field: SerializeField]
    public Transform Visual { get; private set; }

    [field: SerializeField]
    public Transform Bracing { get; private set; }

    protected override async void InteractState()
    {
        if (ScState == SCState.Car)
        {
            CarService.Facade.Skeleton.RemoveWheel(this);
        }

        Rb ??= gameObject.AddComponent<Rigidbody>();
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rb.isKinematic = true;

        transform.parent = null;

        ScState = SCState.Interact;
        _meshCollider.isTrigger = true;
        await UniTask.WaitForSeconds(0.1f);
        _meshCollider.isTrigger = false;
        WheelCollider.enabled = false;
    }

    protected override void CarState()
    {
        if (Rb != null)
        {
            Destroy(Rb);
            Rb = null;
        }

        _meshCollider.isTrigger = true;
        ScState = SCState.Car;
        WheelCollider.enabled = true;
    }

    protected override void DownState()
    {
        if (ScState == SCState.Car)
        {
            CarService.Facade.Skeleton.RemoveWheel(this);
        }

        Rb ??= gameObject.AddComponent<Rigidbody>();
        Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        transform.parent = null;
        Rb.isKinematic = false;

        _meshCollider.isTrigger = false;
        ScState = SCState.Down;
        WheelCollider.enabled = false;
    }

    protected override void TryAttachToCar()
    {
        var colliders = Physics.OverlapSphere(transform.position, _sphereRadius, 1 << LayerMask.NameToLayer("Skeleton"))
            .ToList();

        foreach (var c in colliders)
        {
            var q = c.GetComponentInParent<CarSkeleton>();
            if (q.TryConnectToWheel(this, c.transform))
            {
                CarState();
                return;
            }
        }

        DownState();
    }

    protected override void Reset()
    {
        base.Reset();
        if (UiLabel == "")
            UiLabel = "Grab Wheel";
        WheelCollider = GetComponent<WheelCollider>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (ScState == SCState.Car)
        {
            CheckWheelCollisions();
        }
    }

    protected void Update()
    {
        UpdateWheelVisual();
    }

    private void CheckWheelCollisions()
    {
        if (WheelCollider.GetGroundHit(out var hit))
        {
            var layer = LayerHelper.NamesToLayerMask(new[] { "Environment", "Interactable", });
            if (layer.Contains(hit.collider.gameObject.layer))
            {
                var carVelocity = CarService.Facade.Movement._carRigidbody.linearVelocity;
                var hitVelocity = hit.collider.attachedRigidbody?.linearVelocity ?? Vector3.zero;
                var relativeVelocity = carVelocity - hitVelocity;

                var speed = Mathf.Abs(relativeVelocity.magnitude);

                Hp -= speed;

                //Debug.Log(Hp);
                if (Hp <= 0)
                {
                    CarService.Facade.Skeleton.RemoveWheel(this);
                    DownState();
                }
            }
        }
    }

    private void UpdateWheelVisual()
    {
        Quaternion wheelRotation;
        if (WheelCollider.enabled)
        {
            WheelCollider.GetWorldPose(out _, out var quaternion);
            wheelRotation = Quaternion.Euler(quaternion.eulerAngles + new Vector3(0, Bracing.transform.rotation.eulerAngles.y, 0));
        }
        else
        {
            wheelRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 0));
        }

        Visual.SetPositionAndRotation(transform.position, wheelRotation);
    }
}