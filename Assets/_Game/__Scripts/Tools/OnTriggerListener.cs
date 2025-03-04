using System;
using NaughtyAttributes;
using UnityEngine;

public class OnTriggerListener : MonoBehaviour
{
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerExitEvent;

    [SerializeField]
    private bool _useTriggerLayerMask;
    
    [ShowIf(nameof(_useTriggerLayerMask))]
    [SerializeField]
    private LayerMask _layerMask;

    private void OnTriggerEnter(Collider other)
    {
        if (_useTriggerLayerMask)
        {
            if (LayerMask.LayerToName(_layerMask) == LayerMask.LayerToName(other.gameObject.layer))
            {
                OnTriggerEnterEvent?.Invoke(other);
            }
            return;
        }
        
        OnTriggerEnterEvent?.Invoke(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_useTriggerLayerMask)
        {
            if (LayerMask.LayerToName(_layerMask) == LayerMask.LayerToName(other.gameObject.layer))
            {
                OnTriggerExitEvent?.Invoke(other);
            }
            return;
        }
        
        OnTriggerExitEvent?.Invoke(other);
    }
}