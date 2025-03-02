using System;
using System.Collections.Generic;
using UnityEngine;

public class CarSkeleton : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _sComponents;

    [SerializeField]
    private Rigidbody _rb;

    private List<SkeletonComponent> _removedSComponents = new();
    
    public void RemoveComponent(Transform wheel)
    {
        _sComponents.Remove(wheel);
        var skeletonComponent = new SkeletonComponent
        {
            Transform = wheel,
            Parent = wheel.parent,
            Position = wheel.localPosition,
            Rotation = wheel.localRotation,
        };

        _removedSComponents.Add(skeletonComponent);
        UpdateCar();
    }

    public void ReturnComponent(Transform sComponent)
    {
        _sComponents.Add(sComponent);
        var comp = _removedSComponents.Find(w => w.Transform == sComponent);

        sComponent.SetParent(comp.Parent);
        sComponent.SetLocalPositionAndRotation(comp.Position, comp.Rotation);
        
        _removedSComponents.Remove(comp);
        UpdateCar();
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
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}