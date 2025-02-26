using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRayScanner : MonoBehaviour
{
    public BaseInteract CurrentInteractTarget { get; set; }
    private Transform _cameraTransform;

    [SerializeField]
    private float _interactionRayDistance = 2.3f;

    [SerializeField]
    private float _interactionSphereRadius = 0.25f;

    [SerializeField]
    private LayerMask _layerMask;

    private IRayScannerUiHelper _currentRayScanInfo;
    private GameplayWindow _gameplayWindow;
    private int _frameOptimizeCounter;

    private void Awake()
    {
        if (Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }

        Root.RegisterComponent(nameof(PlayerRayScanner), this);
    }

    private void Start()
    {
        _gameplayWindow = Root.GetReference<GameplayWindow>();
    }

    private void Update()
    {
        _frameOptimizeCounter++;
        if (_frameOptimizeCounter < 7)
        {
            return;
        }

        _currentRayScanInfo = null;
        CurrentInteractTarget = null;
        var ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        var sphereCastInfo = Physics.SphereCastAll(ray, _interactionSphereRadius, _interactionRayDistance, _layerMask.value);

        foreach (var hit in sphereCastInfo)
        {
            if (!hit.collider)
            {
                continue;
            }

            if (!CurrentInteractTarget && hit.collider.TryGetComponent<BaseInteract>(out var interact) && !interact.IsInteractionBlocked)
            {
                CurrentInteractTarget = interact;
            }

            if (CurrentInteractTarget && !CurrentInteractTarget.IsInteractionBlocked)
            {
                var isInteractionSame = hit.collider.gameObject == CurrentInteractTarget.gameObject;
                if (isInteractionSame)
                {
                    var uiInfo = hit.collider.GetComponent<IRayScannerUiHelper>();
                    if (uiInfo != null)
                    {
                        _currentRayScanInfo = uiInfo;
                    }
                }
            }
        }

        UpdateRaySeeUI();
        _frameOptimizeCounter = 0;
    }

    private void UpdateRaySeeUI()
    {
        return;
        if (_currentRayScanInfo != null)
        {
            _gameplayWindow.ShowSeeingInfo(_currentRayScanInfo);
        }
        else
        {
            _gameplayWindow.HideSeeingInfo();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Camera.main == null || Camera.main.transform == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * _interactionRayDistance);
        Gizmos.DrawSphere(Camera.main.transform.position, _interactionSphereRadius);
    }
}