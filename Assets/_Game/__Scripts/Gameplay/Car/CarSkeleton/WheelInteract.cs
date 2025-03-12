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
}