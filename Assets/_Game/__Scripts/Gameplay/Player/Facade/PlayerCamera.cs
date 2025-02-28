using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _character;

    [SerializeField]
    private float _sensitivity = 2;

    [SerializeField]
    private float _smoothing = 1.5f;

    private Vector2 _velocity;
    private Vector2 _frameVelocity;
    
    public void Move(Vector2 mouseDelta)
    {
        var rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * _sensitivity);
        _frameVelocity = Vector2.Lerp(_frameVelocity, rawFrameVelocity, 1 / _smoothing);
        _velocity += _frameVelocity;
        _velocity.y = Mathf.Clamp(_velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-_velocity.y, Vector3.right);
        _character.localRotation = Quaternion.AngleAxis(_velocity.x, Vector3.up);
    }

    public void ResetPosition()
    {
        Move(Vector2.zero);
    }
}