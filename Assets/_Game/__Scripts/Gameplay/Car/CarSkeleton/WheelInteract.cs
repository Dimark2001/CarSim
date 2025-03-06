using System.Linq;
using DG.Tweening;
using UnityEngine;

public class WheelInteract : BaseInteract
{
    [SerializeField]
    private WheelCollider _wheel;

    [SerializeField]
    private MeshCollider _meshCollider;

    [SerializeField]
    private float _sphereRadius = 0.5f;

    private Rigidbody _rb;

    private Transform _holdingParent;
    private CarService _carService;
    private SCState _scState = SCState.Car;

    private void Start()
    {
        _holdingParent = G.Get<PlayerService>().Player.Interaction.HoldingParentTransform;
        _carService = G.Get<CarService>();
    }

    private void OnDestroy()
    {
        transform?.DOKill();
    }

    public override void StartInteract()
    {
        InteractState();

        transform.DOMove(_holdingParent.position, 0.1f);
    }

    public override void StopInteract()
    {
        base.StopInteract();
        TryAttachToCar();
    }

    private void TryAttachToCar()
    {
        var colliders = Physics.OverlapSphere(transform.position, _sphereRadius, 1 << LayerMask.NameToLayer("Skeleton"))
            .ToList();

        foreach (var c in colliders)
        {
            var q = c.GetComponentInParent<CarSkeleton>();
            if (q.TryConnectToBracing(transform, c.transform))
            {
                CarState();
                return;
            }
        }

        DownState();
    }

    private void InteractState()
    {
        if (_scState == SCState.Car)
        {
            _carService.Car.Skeleton.RemoveComponent(transform);
        }

        _scState = SCState.Interact;
        if (_rb != null)
        {
            Destroy(_rb);
            _rb = null;
        }

        transform.parent = _holdingParent;

        _wheel.enabled = false;
        _meshCollider.isTrigger = true;
    }

    private void CarState()
    {
        _scState = SCState.Car;
        if (_rb != null)
        {
            Destroy(_rb);
            _rb = null;
        }

        _wheel.enabled = true;
        _meshCollider.isTrigger = true;
    }

    private void DownState()
    {
        if (_scState == SCState.Car)
        {
            _carService.Car.Skeleton.RemoveComponent(transform);
        }

        _scState = SCState.Down;
        _rb ??= gameObject.AddComponent<Rigidbody>();
        transform.parent = null;

        _wheel.enabled = false;
        _meshCollider.isTrigger = false;
    }

    private void Reset()
    {
        UiLabel = "Grab Wheel";
        _wheel = GetComponent<WheelCollider>();
        _meshCollider = GetComponent<MeshCollider>();
    }
}