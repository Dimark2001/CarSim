using System.Linq;
using DG.Tweening;
using UnityEngine;

public class WheelInteract : BaseInteract
{
    [SerializeField]
    private WheelCollider _wheel;

    [SerializeField]
    private MeshCollider _meshCollider;

    private Rigidbody _rb;

    private Transform _holdingParent;
    private CarService _carService;
    private WheelState _wheelState = WheelState.Car;

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

        transform.DOMove(_holdingParent.position, 0.2f);
        transform.DORotate(_holdingParent.rotation.eulerAngles, 0.2f);
    }

    public override void StopInteract()
    {
        base.StopInteract();
        TryAttachToCar();
    }

    private void TryAttachToCar()
    {
        var colliders = Physics.OverlapSphere(transform.position, 1f).ToList();
        var find = colliders.Find(c =>
        {
            var q = c.GetComponentInParent<CarSkeleton>();
            if (q != null)
            {
                CarState();
                q.ReturnComponent(transform);
                return true;
            }

            DownState();
            return false;
        });
    }

    private void InteractState()
    {
        if (_wheelState == WheelState.Car)
        {
            _carService.Car.Skeleton.RemoveComponent(transform);
        }

        _wheelState = WheelState.Interact;
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
        _wheelState = WheelState.Car;
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
        if (_wheelState == WheelState.Car)
        {
            _carService.Car.Skeleton.RemoveComponent(transform);
        }

        _wheelState = WheelState.Down;
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

    public enum WheelState
    {
        Interact,
        Down,
        Car,
    }
}