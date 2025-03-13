using UnityEngine;

public class WheelInteract : SkeletonInteract
{
    [SerializeField]
    private WheelCollider _wheel;

    protected override void InteractState()
    {
        base.InteractState();
        _wheel.enabled = false;
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
        _wheel.enabled = true;
    }

    protected override void DownState()
    {
        base.DownState();
        _wheel.enabled = false;
    }

    protected override void Reset()
    {
        base.Reset();
        UiLabel = "Grab Wheel";
        _wheel = GetComponent<WheelCollider>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (ScState == SCState.Car)
        {
            CheckWheelCollisions();
        }
    }

    private void CheckWheelCollisions()
    {
        if (_wheel.GetGroundHit(out var hit))
        {
            var layer = LayerHelper.NamesToLayerMask(new[] { "Environment", "Interactable", });
            if (layer.Contains(hit.collider.gameObject.layer))
            {
                var relativeVelocity =
                    CarService.Facade.Movement._carRigidbody.GetPointVelocity(hit.point) -
                    hit.collider.attachedRigidbody?.linearVelocity ?? Vector3.zero;
                
                var speed = relativeVelocity.magnitude;

                Hp -= speed;

                Debug.Log("CheckWheelCollisions: " + speed);
                if (Hp <= 0)
                {
                    CarService.Facade.Skeleton.RemoveComponent(this);
                    DownState();
                }
            }
        }
    }
}