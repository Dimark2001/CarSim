using System;
using System.Collections.Generic;
using UnityEngine;

public class CarSkeleton : MonoBehaviour
{
    [SerializeField]
    private List<SkeletonInteract> _sComponents;

    [SerializeField]
    private List<Transform> _bracingList;

    [SerializeField]
    private Rigidbody _rb;

    private List<SkeletonComponent> _removedSComponents = new();

    public void RemoveComponent(SkeletonInteract c)
    {
        _sComponents.Remove(c);
        var skeletonComponent = new SkeletonComponent
        {
            Transform = c.transform,
            Parent = c.transform.parent,
        };

        _removedSComponents.Add(skeletonComponent);
        UpdateCar();
    }
    
    public void RemoveWheel(WheelInteract c)
    {
        _sComponents.Remove(c);
        var skeletonComponent = new SkeletonComponent
        {
            Transform = c.transform,
            Parent = c.transform.parent,
        };

        _removedSComponents.Add(skeletonComponent);
        UpdateCar();
    }

    public bool TryConnectToBracing(SkeletonInteract component, Transform bracing)
    {
        if (component.name != bracing.name)
        {
            return false;
        }

        _sComponents.Add(component);
        var comp = _removedSComponents.Find(w => w.Transform == component.transform);

        component.transform.SetParent(comp.Parent, true);
        component.transform.SetLocalPositionAndRotation(bracing.localPosition, bracing.localRotation);

        _removedSComponents.Remove(comp);
        UpdateCar();
        return true;
    }
    
    public bool TryConnectToWheel(WheelInteract component, Transform bracing)
    {
        if (!bracing.name.Contains("Wheel"))
        {
            return false;
        }

        _sComponents.Add(component);
        var comp = _removedSComponents.Find(w => w.Transform == component.transform);

        component.transform.SetParent(comp.Parent, true);
        component.transform.SetLocalPositionAndRotation(bracing.localPosition, bracing.localRotation);
        _removedSComponents.Remove(comp);
        
        G.Get<CarService>().Facade.Movement.SetWheel(component.WheelCollider, bracing.name);
        UpdateCar();
        return true;
    }

    private void UpdateCar()
    {
        _rb.WakeUp();
    }

    [Serializable]
    private class SkeletonComponent
    {
        public Transform Transform { get; set; }
        public Transform Parent { get; set; }
    }
}