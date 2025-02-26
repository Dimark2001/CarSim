using System;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField]
    private float _jumpStrength = 2;

    private Rigidbody _rigidbody;
    public event Action Jumped;

    [Tooltip("Prevents jumping when the transform is in mid-air.")]
    [SerializeField]
    private GroundCheck _groundCheck;

    private InputController _inputController;

    private void Start()
    {
        _inputController = Root.GetReference<InputController>();
    }

    private void Reset()
    {
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (_inputController.IsJump() && (!_groundCheck || _groundCheck.isGrounded))
        {
            _rigidbody.AddForce(Vector3.up * (100 * _jumpStrength));
            Jumped?.Invoke();
        }
    }
}