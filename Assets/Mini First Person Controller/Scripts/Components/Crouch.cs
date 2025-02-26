using UnityEngine;

public class Crouch : MonoBehaviour
{
    [Header("Slow Movement")]
    public FirstPersonMovement _movement;

    [SerializeField]
    private float _movementSpeed = 2;

    [Header("Low Head")]
    public Transform _headToLower;

    private float? _defaultHeadYLocalPosition;

    [SerializeField]
    private float _crouchYHeadPosition = 1;

    [Header("Collider")]
    public CapsuleCollider _colliderToLower;

    private float? _defaultColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    private InputController _inputController;

    private void Start()
    {
        _inputController = Root.GetReference<InputController>();
    }

    private void Reset()
    {
        _movement = GetComponentInParent<FirstPersonMovement>();
        _headToLower = _movement.GetComponentInChildren<Camera>().transform;
        _colliderToLower = _movement.GetComponentInChildren<CapsuleCollider>();
    }

    private void LateUpdate()
    {
        if (_inputController.IsCrouch())
        {
            if (_headToLower)
            {
                if (!_defaultHeadYLocalPosition.HasValue)
                {
                    _defaultHeadYLocalPosition = _headToLower.localPosition.y;
                }

                _headToLower.localPosition = new Vector3(_headToLower.localPosition.x, _crouchYHeadPosition,
                    _headToLower.localPosition.z);
            }

            if (_colliderToLower)
            {
                if (!_defaultColliderHeight.HasValue)
                {
                    _defaultColliderHeight = _colliderToLower.height;
                }

                float loweringAmount;
                if (_defaultHeadYLocalPosition.HasValue)
                {
                    loweringAmount = _defaultHeadYLocalPosition.Value - _crouchYHeadPosition;
                }
                else
                {
                    loweringAmount = _defaultColliderHeight.Value * .5f;
                }

                _colliderToLower.height = Mathf.Max(_defaultColliderHeight.Value - loweringAmount, 0);
                _colliderToLower.center = Vector3.up * (_colliderToLower.height * .5f);
            }

            if (!IsCrouched)
            {
                IsCrouched = true;
                SetSpeedOverrideActive(true);
                CrouchStart?.Invoke();
            }
        }
        else
        {
            if (IsCrouched)
            {
                if (_headToLower)
                {
                    _headToLower.localPosition = new Vector3(_headToLower.localPosition.x,
                        _defaultHeadYLocalPosition.Value, _headToLower.localPosition.z);
                }

                if (_colliderToLower)
                {
                    _colliderToLower.height = _defaultColliderHeight.Value;
                    _colliderToLower.center = Vector3.up * (_colliderToLower.height * .5f);
                }

                IsCrouched = false;
                SetSpeedOverrideActive(false);
                CrouchEnd?.Invoke();
            }
        }
    }

    private void SetSpeedOverrideActive(bool state)
    {
        if (!_movement)
        {
            return;
        }

        if (state)
        {
            if (!_movement.SpeedOverrides.Contains(SpeedOverride))
            {
                _movement.SpeedOverrides.Add(SpeedOverride);
            }
        }
        else
        {
            if (_movement.SpeedOverrides.Contains(SpeedOverride))
            {
                _movement.SpeedOverrides.Remove(SpeedOverride);
            }
        }
    }

    private float SpeedOverride() => _movementSpeed;
}