using System;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField]
    private float _jumpStrength = 2;

    [SerializeField]
    private Rigidbody _rigidbody;
    
    public event Action Jumped;

    [Tooltip("Prevents jumping when the transform is in mid-air.")]
    [SerializeField]
    private GroundCheck _groundCheck;
    
    private void Reset()
    {
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }

    public void Perform()
    {
        if (!_groundCheck || _groundCheck.isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * (100 * _jumpStrength));
            Jumped?.Invoke();
        }
    }
}