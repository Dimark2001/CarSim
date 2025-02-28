using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public BaseInteract CurrentInteractTarget { get; private set; }

    [SerializeField]
    private float _interactionRayDistance = 2.3f;

    [SerializeField]
    private float _interactionSphereRadius = 0.25f;

    [SerializeField]
    private LayerMask _layerMask;

    private IInteractable _currentTriggerInfo;
    private GameplayWindow _gameplayWindow;
    private int _frameOptimizeCounter;

    private void UpdateTriggerUI()
    {
        if (_currentTriggerInfo != null)
        {
            _gameplayWindow.ShowSeeingInfo(_currentTriggerInfo);
        }
        else
        {
            _gameplayWindow.HideSeeingInfo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CurrentInteractTarget && other.TryGetComponent<BaseInteract>(out var interact) &&
            !interact.IsInteractionBlocked)
        {
            CurrentInteractTarget = interact;
        }

        if (CurrentInteractTarget && !CurrentInteractTarget.IsInteractionBlocked)
        {
            var isInteractionSame = other.gameObject == CurrentInteractTarget.gameObject;
            if (isInteractionSame)
            {
                var uiInfo = other.GetComponentInParent<IInteractable>();
                if (uiInfo != null)
                {
                    _currentTriggerInfo = uiInfo;
                }
            }
        }

        UpdateTriggerUI();
    }

    private void OnTriggerExit(Collider other)
    {
        if (CurrentInteractTarget && other.gameObject == CurrentInteractTarget.gameObject)
        {
            CurrentInteractTarget = null;
            _currentTriggerInfo = null;
        }

        UpdateTriggerUI();
    }
}