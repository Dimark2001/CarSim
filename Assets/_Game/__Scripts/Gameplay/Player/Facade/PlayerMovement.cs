using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField]
    public MovementWrapper Wrapper { get; private set; }

    [SerializeField]
    private float _speed = 5;

    [SerializeField]
    private float _runSpeed = 5;

    [SerializeField]
    private bool _canRun = true;

    [SerializeField]
    private Rigidbody _rigidbody;

    public bool IsRunning { get; private set; }

    public List<Func<float>> SpeedOverrides = new();

    public void Move(Vector2 direction)
    {
        var targetMovingSpeed = IsRunning ? _runSpeed : _speed;

        if (SpeedOverrides.Count > 0)
        {
            targetMovingSpeed = SpeedOverrides[^1]();
        }

        var targetVelocity = new Vector2(direction.x * targetMovingSpeed, direction.y * targetMovingSpeed);

        _rigidbody.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.y);
    }

    public void Run(bool isRun)
    {
        if (_canRun)
        {
            IsRunning = isRun;
        }
        else
        {
            IsRunning = false;
        }
    }
    
    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }
    
    [Serializable]
    public class MovementWrapper
    {
        [field: SerializeField]
        public Crouch Crouch { get; private set; }

        [field: SerializeField]
        public Jump Jump { get; private set; }
    }

    public void ResetPosition()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}