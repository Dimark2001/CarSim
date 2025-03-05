using System;
using System.Collections.Generic;
using UnityEngine;

public class CarSkeleton : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _sComponents;
    [SerializeField]
    private List<Transform> _bracingList;

    [SerializeField]
    private Rigidbody _rb;

    private List<SkeletonComponent> _removedSComponents = new();
    
    public void RemoveComponent(Transform c)
    {
        _sComponents.Remove(c);
        var skeletonComponent = new SkeletonComponent
        {
            Transform = c,
            Parent = c.parent,
        };

        _removedSComponents.Add(skeletonComponent);
        UpdateCar();
    }

    public bool TryConnectToBracing(Transform component, Transform bracing)
    {
        if (component.name != bracing.name)
        {
            return false;
        }
        
        _sComponents.Add(component);
        var comp = _removedSComponents.Find(w => w.Transform == component);

        component.SetParent(comp.Parent, true);
        component.SetLocalPositionAndRotation(bracing.localPosition, bracing.localRotation);
        
        _removedSComponents.Remove(comp);
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