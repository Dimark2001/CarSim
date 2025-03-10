using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public BaseInteract CurrentInteractTarget { get; private set; }

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private float _raycastDistance = 5f;

    private List<BaseInteract> _interactList;

    private GameplayWindow _gameplayWindow;

    private Camera _mainCamera;

    private void Start()
    {
        _interactList = new List<BaseInteract>();
        _gameplayWindow = G.Get<UIService>().UIFacade.GameplayWindow;
        _mainCamera = Camera.main; // Получаем основную камеру
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
        CurrentInteractTarget = null;
        var closestDistanceToCenter = 100f;

        foreach (var interact in _interactList)
        {
            var screenPos = _mainCamera.WorldToViewportPoint(interact.transform.position);

            var isObjectInScreen = screenPos.x >= 0 && screenPos.x <= 1 &&
                                   screenPos.y >= 0 && screenPos.y <= 1 &&
                                   screenPos.z > 0;
            if (!isObjectInScreen)
            {
                continue;
            }

            var screenCenter = new Vector2(0.5f, 0.5f);
            var objectScreenPosition = new Vector2(screenPos.x, screenPos.y);
            var distanceToCenter = Vector2.Distance(screenCenter, objectScreenPosition);

            var rayToObject = _mainCamera.ViewportPointToRay(screenPos);
            Debug.DrawRay(rayToObject.origin, rayToObject.direction.normalized * _raycastDistance, Color.red);

            var isFirstRaycastHit = Physics.Raycast(rayToObject, out var hitToObject, _raycastDistance, _layerMask);
            var isFirstRaycastValid = isFirstRaycastHit && hitToObject.collider.gameObject == interact.gameObject;

            var rayFromCamera = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            Debug.DrawRay(rayFromCamera.origin, rayFromCamera.direction.normalized * _raycastDistance, Color.blue);

            var isSecondRaycastHit = Physics.Raycast(rayFromCamera, out var hitFromCamera, _raycastDistance, _layerMask);
            var isSecondRaycastValid = isSecondRaycastHit && hitFromCamera.collider.gameObject == interact.gameObject;

            if (!isFirstRaycastValid && !isSecondRaycastValid)
            {
                continue;
            }

            if (!(distanceToCenter < closestDistanceToCenter))
            {
                continue;
            }

            closestDistanceToCenter = distanceToCenter;
            CurrentInteractTarget = interact;
        }

        if (CurrentInteractTarget)
        {
            _gameplayWindow.ShowSeeingInfo(CurrentInteractTarget);
        }
        else
        {
            _gameplayWindow.HideSeeingInfo();
        }
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

        if (_interactList.Count == 0)
        {
            CurrentInteractTarget = null;
            _gameplayWindow.HideSeeingInfo();
        }
    }

    private void Update()
    {
        if (!enabled)
        {
            return;
        }

        if (_interactList.Count == 0)
        {
            return;
        }

        FindClosestInteract();
    }
}