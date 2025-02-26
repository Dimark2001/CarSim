using UnityEngine;
using System.Collections.Generic;

public class FirstPersonMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;

    [SerializeField]
    private bool _canRun = true;

    public bool IsRunning { get; private set; }

    private Rigidbody _rigidbody;
    private InputController _inputController;
    public List<System.Func<float>> SpeedOverrides = new();

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputController = Root.GetReference<InputController>();
    }

    private void FixedUpdate()
    {
        IsRunning = _canRun && _inputController.IsRun();

        var targetMovingSpeed = IsRunning ? 9 : _speed;

        if (SpeedOverrides.Count > 0)
        {
            targetMovingSpeed = SpeedOverrides[^1]();
        }

        var targetVelocity = new Vector2(_inputController.Move().x * targetMovingSpeed,
            _inputController.Move().y * targetMovingSpeed);

        _rigidbody.linearVelocity = transform.rotation *
                                    new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.y);
    }
}