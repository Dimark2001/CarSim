using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public BaseInteract CurrentInteractTarget { get; private set; }

    [SerializeField]
    private LayerMask _layerMask;
    
    private List<BaseInteract> _interactList;

    private GameplayWindow _gameplayWindow;
    
    private void Start()
    {
        _interactList = new List<BaseInteract>();
        _gameplayWindow = G.Get<UIService>().UIFacade.GameplayWindow;
    }

    public void Disable()
    {
        _interactList.Clear();
        _gameplayWindow.HideSeeingInfo();
        CurrentInteractTarget = null;
        enabled = false;
    }

    private void FindClosestInteract()
    {
        if (_interactList.Count == 0)
        {
            CurrentInteractTarget = null;
            _gameplayWindow.HideSeeingInfo();
            return;
        }

        var orderedEnumerable = _interactList.OrderBy(i => Vector3.Distance(transform.position, i.transform.position));
        CurrentInteractTarget = orderedEnumerable.FirstOrDefault();
        _gameplayWindow.ShowSeeingInfo(CurrentInteractTarget);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
        {
            return;
        }
        
        if (!other.TryGetComponent<BaseInteract>(out var interact))
        {
            return;
        }

        if (_interactList.Contains(interact))
        {
            return;
        }

        _interactList.Add(interact);
        FindClosestInteract();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled)
        {
            return;
        }
        
        if (!other.TryGetComponent<BaseInteract>(out var interact))
        {
            return;
        }

        _interactList.Remove(interact);
        FindClosestInteract();
    }
}