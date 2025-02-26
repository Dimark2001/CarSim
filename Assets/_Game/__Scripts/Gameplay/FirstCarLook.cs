using UnityEngine;

public class FirstCarLook : MonoBehaviour
{
    [SerializeField]
    private Transform _character;

    [SerializeField]
    private float _sensitivity = 2;

    [SerializeField]
    private float _smoothing = 1.5f;

    private Vector2 _velocity;
    private Vector2 _frameVelocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        var mouseDelta = Root.GetReference<InputController>().Look();
        var rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * _sensitivity);
        _frameVelocity = Vector2.Lerp(_frameVelocity, rawFrameVelocity, 1 / _smoothing);
        _velocity += _frameVelocity;
        _velocity.y = Mathf.Clamp(_velocity.y, -90, 90);

        _character.localRotation = Quaternion.Euler(_velocity.y, _velocity.x, 0);
    }
}