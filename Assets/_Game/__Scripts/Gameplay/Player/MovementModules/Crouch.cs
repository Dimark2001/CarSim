using UnityEngine;

public class Crouch : MonoBehaviour
{
    public PlayerMovement _movement;

    [SerializeField]
    private float _movementSpeed = 2;

    public Transform _headToLower;

    private float? _defaultHeadYLocalPosition;

    [SerializeField]
    private float _crouchYHeadPosition = 1;

    public CapsuleCollider _colliderToLower;

    private float? _defaultColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    public void Perform()
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

    public void Cancel()
    {
        if (IsCrouched)
        {
            if (_headToLower)
            {
                _headToLower.localPosition = new Vector3(_headToLower.localPosition.x, _defaultHeadYLocalPosition.Value, _headToLower.localPosition.z);
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