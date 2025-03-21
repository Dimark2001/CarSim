using System;
using NaughtyAttributes;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Rigidbody _carRigidbody;

    [HideInInspector]
    public Vector2 MoveDirection;

    [HideInInspector]
    public bool IsHandbrake;

    #region CarSetup

    [Space(10)]
    [Header("CAR SETUP")]
    [Space(20)]
    [Range(20, 190)]
    [SerializeField]
    private int _maxSpeed = 120;

    [Range(10, 120)]
    [SerializeField]
    private int _maxReverseSpeed = 45;

    [Range(1, 10)]
    [SerializeField]
    private int _accelerationMultiplier = 5;

    [Space(10)]
    [Range(10, 45)]
    [SerializeField]
    private int _maxSteeringAngle = 35;

    [Range(0.1f, 1f)]
    [SerializeField]
    private float _steeringSpeed = 0.5f;

    [Space(10)]
    [Range(100, 600)]
    [SerializeField]
    private int _brakeForce = 450;

    [Range(1, 10)]
    [SerializeField]
    private int _decelerationMultiplier = 1;

    [Range(1, 10)]
    [SerializeField]
    private int _handbrakeDriftMultiplier = 5;

    [Space(10)]
    [SerializeField]
    private Vector3 _bodyMassCenter;

    #endregion

    #region Wheels

    [Space(20)]
    [Header("WHEELS")]
    
    [SerializeField]
    private WheelCollider _wheelColliderLp;

    [SerializeField]
    private WheelCollider _wheelColliderPp;

    [SerializeField]
    private WheelCollider _wheelColliderLz;

    [SerializeField]
    private WheelCollider _wheelColliderPz;

    #endregion

    #region Effects

    [Space(20)]
    [Header("EFFECTS")]
    [SerializeField]
    private bool _useEffects = false;

    [ShowIf("_useEffects")]
    [SerializeField]
    private ParticleSystem _rlwParticleSystem;

    [ShowIf("_useEffects")]
    [SerializeField]
    private ParticleSystem _rrwParticleSystem;

    [ShowIf("_useEffects")]
    [Space(10)]
    [SerializeField]
    private TrailRenderer _rlwTireSkid;

    [ShowIf("_useEffects")]
    [SerializeField]
    private TrailRenderer _rrwTireSkid;

    #endregion

    #region Sounds

    [Space(20)]
    [Header("SOUNDS")]
    [SerializeField]
    private bool _useSounds;

    [ShowIf("_useSounds")]
    [SerializeField]
    private AudioSource _carEngineSound;

    [ShowIf("_useSounds")]
    [SerializeField]
    private AudioSource _tireScreechSound;

    #endregion

    #region Private

    private float _initialCarEngineSoundPitch;
    private float _carSpeed;
    private float _steeringAxis;
    private float _throttleAxis;
    private float _driftingAxis;
    private float _localVelocityZ;
    private float _localVelocityX;

    private bool _deceleratingCar;
    private bool _isDrifting;
    private bool _isTractionLocked;

    private WheelFrictionCurve _fLWheelFriction;
    private float _flWExtremumSlip;
    private WheelFrictionCurve _fRWheelFriction;
    private float _frWExtremumSlip;
    private WheelFrictionCurve _rLWheelFriction;
    private float _rlWExtremumSlip;
    private WheelFrictionCurve _rRWheelFriction;
    private float _rrWExtremumSlip;

    #endregion

    public void Initialize()
    {
        _carRigidbody.centerOfMass = _bodyMassCenter;
        _fLWheelFriction = new WheelFrictionCurve
        {
            extremumSlip = _wheelColliderLp.sidewaysFriction.extremumSlip,
            extremumValue = _wheelColliderLp.sidewaysFriction.extremumValue,
            asymptoteSlip = _wheelColliderLp.sidewaysFriction.asymptoteSlip,
            asymptoteValue = _wheelColliderLp.sidewaysFriction.asymptoteValue,
            stiffness = _wheelColliderLp.sidewaysFriction.stiffness
        };
        _flWExtremumSlip = _wheelColliderLp.sidewaysFriction.extremumSlip;

        _fRWheelFriction = new WheelFrictionCurve
        {
            extremumSlip = _wheelColliderPp.sidewaysFriction.extremumSlip,
            extremumValue = _wheelColliderPp.sidewaysFriction.extremumValue,
            asymptoteSlip = _wheelColliderPp.sidewaysFriction.asymptoteSlip,
            asymptoteValue = _wheelColliderPp.sidewaysFriction.asymptoteValue,
            stiffness = _wheelColliderPp.sidewaysFriction.stiffness
        };
        _frWExtremumSlip = _wheelColliderPp.sidewaysFriction.extremumSlip;

        _rLWheelFriction = new WheelFrictionCurve
        {
            extremumSlip = _wheelColliderLz.sidewaysFriction.extremumSlip,
            extremumValue = _wheelColliderLz.sidewaysFriction.extremumValue,
            asymptoteSlip = _wheelColliderLz.sidewaysFriction.asymptoteSlip,
            asymptoteValue = _wheelColliderLz.sidewaysFriction.asymptoteValue,
            stiffness = _wheelColliderLz.sidewaysFriction.stiffness
        };
        _rlWExtremumSlip = _wheelColliderLz.sidewaysFriction.extremumSlip;

        _rRWheelFriction = new WheelFrictionCurve
        {
            extremumSlip = _wheelColliderPz.sidewaysFriction.extremumSlip,
            extremumValue = _wheelColliderPz.sidewaysFriction.extremumValue,
            asymptoteSlip = _wheelColliderPz.sidewaysFriction.asymptoteSlip,
            asymptoteValue = _wheelColliderPz.sidewaysFriction.asymptoteValue,
            stiffness = _wheelColliderPz.sidewaysFriction.stiffness
        };
        _rrWExtremumSlip = _wheelColliderPz.sidewaysFriction.extremumSlip;

        if (_carEngineSound != null)
        {
            _initialCarEngineSoundPitch = _carEngineSound.pitch;
        }

        if (_useSounds)
        {
            InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }
        else if (!_useSounds)
        {
            if (_carEngineSound != null)
            {
                _carEngineSound.Stop();
            }

            if (_tireScreechSound != null)
            {
                _tireScreechSound.Stop();
            }
        }

        if (!_useEffects)
        {
            if (_rlwParticleSystem != null)
            {
                _rlwParticleSystem.Stop();
            }

            if (_rrwParticleSystem != null)
            {
                _rrwParticleSystem.Stop();
            }

            if (_rlwTireSkid != null)
            {
                _rlwTireSkid.emitting = false;
            }

            if (_rrwTireSkid != null)
            {
                _rrwTireSkid.emitting = false;
            }
        }
        
        CrashSetting();
    }

    public void Move()
    {
        _carSpeed = 2 * Mathf.PI * _wheelColliderLp.radius * _wheelColliderLp.rpm * 60 / 1000;

        _localVelocityX = transform.InverseTransformDirection(_carRigidbody.linearVelocity).x;
        _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.linearVelocity).z;

        if (MoveDirection.y > 0)
        {
            CancelInvoke(nameof(DecelerateCar));
            _deceleratingCar = false;
            GoForward();
        }

        if (MoveDirection.y < 0)
        {
            CancelInvoke(nameof(DecelerateCar));
            _deceleratingCar = false;
            GoReverse();
        }

        if (MoveDirection.x < 0)
        {
            TurnLeft();
        }

        if (MoveDirection.x > 0)
        {
            TurnRight();
        }

        if (MoveDirection.x == 0 && MoveDirection.y == 0)
        {
            ThrottleOff();
        }

        if (MoveDirection.y == 0 && !IsHandbrake && !_deceleratingCar)
        {
            InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
            _deceleratingCar = true;
        }
    }

    private void FixedUpdate()
    {
        if(!G.IsInitialized)
            return;
        if (MoveDirection.x == 0 && _steeringAxis != 0f)
        {
            ResetSteeringAngle();
        }
        
        CrashCar();
    }

    public void DecelerateCar()
    {
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            _isDrifting = true;
            DriftCarPS();
        }
        else
        {
            _isDrifting = false;
            DriftCarPS();
        }

        if (_throttleAxis != 0f)
        {
            if (_throttleAxis > 0f)
            {
                _throttleAxis = _throttleAxis - (Time.deltaTime * 10f);
            }
            else if (_throttleAxis < 0f)
            {
                _throttleAxis = _throttleAxis + (Time.deltaTime * 10f);
            }

            if (Mathf.Abs(_throttleAxis) < 0.15f)
            {
                _throttleAxis = 0f;
            }
        }

        _carRigidbody.linearVelocity *= 1f / (1f + (0.025f * _decelerationMultiplier));
        _wheelColliderLp.motorTorque = 0;
        _wheelColliderPp.motorTorque = 0;
        _wheelColliderLz.motorTorque = 0;
        _wheelColliderPz.motorTorque = 0;
        if (_carRigidbody.linearVelocity.magnitude < 0.25f)
        {
            _carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    public void Brakes()
    {
        _wheelColliderLp.brakeTorque = _brakeForce;
        _wheelColliderPp.brakeTorque = _brakeForce;
        _wheelColliderLz.brakeTorque = _brakeForce;
        _wheelColliderPz.brakeTorque = _brakeForce;
    }

    public void Handbrake()
    {
        CancelInvoke(nameof(DecelerateCar));
        _deceleratingCar = false;
        CancelInvoke(nameof(RecoverTraction));
        _driftingAxis += Time.deltaTime;
        var secureStartingPoint = _driftingAxis * _flWExtremumSlip * _handbrakeDriftMultiplier;

        if (secureStartingPoint < _flWExtremumSlip)
        {
            _driftingAxis = _flWExtremumSlip / (_flWExtremumSlip * _handbrakeDriftMultiplier);
        }

        if (_driftingAxis > 1f)
        {
            _driftingAxis = 1f;
        }

        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            _isDrifting = true;
        }
        else
        {
            _isDrifting = false;
        }

        if (_driftingAxis < 1f)
        {
            _fLWheelFriction.extremumSlip = _flWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderLp.sidewaysFriction = _fLWheelFriction;

            _fRWheelFriction.extremumSlip = _frWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderPp.sidewaysFriction = _fRWheelFriction;

            _rLWheelFriction.extremumSlip = _rlWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderLz.sidewaysFriction = _rLWheelFriction;

            _rRWheelFriction.extremumSlip = _rrWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderPz.sidewaysFriction = _rRWheelFriction;
        }

        _isTractionLocked = true;
        DriftCarPS();
    }

    public void RecoverTraction()
    {
        _isTractionLocked = false;
        _driftingAxis = _driftingAxis - (Time.deltaTime / 1.5f);
        if (_driftingAxis < 0f)
        {
            _driftingAxis = 0f;
        }

        if (_fLWheelFriction.extremumSlip > _flWExtremumSlip)
        {
            _fLWheelFriction.extremumSlip = _flWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderLp.sidewaysFriction = _fLWheelFriction;

            _fRWheelFriction.extremumSlip = _frWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderPp.sidewaysFriction = _fRWheelFriction;

            _rLWheelFriction.extremumSlip = _rlWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderLz.sidewaysFriction = _rLWheelFriction;

            _rRWheelFriction.extremumSlip = _rrWExtremumSlip * _handbrakeDriftMultiplier * _driftingAxis;
            _wheelColliderPz.sidewaysFriction = _rRWheelFriction;

            Invoke("RecoverTraction", Time.deltaTime);
        }
        else if (_fLWheelFriction.extremumSlip < _flWExtremumSlip)
        {
            _fLWheelFriction.extremumSlip = _flWExtremumSlip;
            _wheelColliderLp.sidewaysFriction = _fLWheelFriction;

            _fRWheelFriction.extremumSlip = _frWExtremumSlip;
            _wheelColliderPp.sidewaysFriction = _fRWheelFriction;

            _rLWheelFriction.extremumSlip = _rlWExtremumSlip;
            _wheelColliderLz.sidewaysFriction = _rLWheelFriction;

            _rRWheelFriction.extremumSlip = _rrWExtremumSlip;
            _wheelColliderPz.sidewaysFriction = _rRWheelFriction;

            _driftingAxis = 0f;
        }
    }

    public void SetWheel(WheelCollider wheel, string wheelBracing)
    {
        if (wheelBracing.Contains("L P"))
        {
            _wheelColliderLp = wheel;
        }
        else if (wheelBracing.Contains("P P"))
        {
            _wheelColliderPp = wheel;
        }
        else if (wheelBracing.Contains("L Z"))
        {
            _wheelColliderLz = wheel;
        }
        else if (wheelBracing.Contains("P Z"))
        {
            _wheelColliderPz = wheel;
        }
    }

    private void GoForward()
    {
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            _isDrifting = true;
            DriftCarPS();
        }
        else
        {
            _isDrifting = false;
            DriftCarPS();
        }

        _throttleAxis = _throttleAxis + (Time.deltaTime * 3f);
        if (_throttleAxis > 1f)
        {
            _throttleAxis = 1f;
        }

        if (_localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(_carSpeed) < _maxSpeed)
            {
                _wheelColliderLp.brakeTorque = 0;
                _wheelColliderLp.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                _wheelColliderPp.brakeTorque = 0;
                _wheelColliderPp.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                _wheelColliderLz.brakeTorque = 0;
                _wheelColliderLz.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                _wheelColliderPz.brakeTorque = 0;
                _wheelColliderPz.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
            }
            else
            {
                _wheelColliderLp.motorTorque = 0;
                _wheelColliderPp.motorTorque = 0;
                _wheelColliderLz.motorTorque = 0;
                _wheelColliderPz.motorTorque = 0;
            }
        }
    }

    private void GoReverse()
    {
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            _isDrifting = true;
            DriftCarPS();
        }
        else
        {
            _isDrifting = false;
            DriftCarPS();
        }

        _throttleAxis = _throttleAxis - (Time.deltaTime * 3f);
        if (_throttleAxis < -1f)
        {
            _throttleAxis = -1f;
        }

        if (_localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(_carSpeed)) < _maxReverseSpeed)
            {
                _wheelColliderLp.brakeTorque = 0;
                _wheelColliderLp.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                _wheelColliderPp.brakeTorque = 0;
                _wheelColliderPp.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                _wheelColliderLz.brakeTorque = 0;
                _wheelColliderLz.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                _wheelColliderPz.brakeTorque = 0;
                _wheelColliderPz.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
            }
            else
            {
                _wheelColliderLp.motorTorque = 0;
                _wheelColliderPp.motorTorque = 0;
                _wheelColliderLz.motorTorque = 0;
                _wheelColliderPz.motorTorque = 0;
            }
        }
    }

    private void ThrottleOff()
    {
        _wheelColliderLp.motorTorque = 0;
        _wheelColliderPp.motorTorque = 0;
        _wheelColliderLz.motorTorque = 0;
        _wheelColliderPz.motorTorque = 0;
    }

    private void CarSounds()
    {
        if (_useSounds)
        {
            try
            {
                if (_carEngineSound != null)
                {
                    var engineSoundPitch = _initialCarEngineSoundPitch +
                                           Mathf.Abs(_carRigidbody.linearVelocity.magnitude) / 25f;
                    _carEngineSound.pitch = engineSoundPitch;
                }

                if (_isDrifting || (_isTractionLocked && Mathf.Abs(_carSpeed) > 12f))
                {
                    if (!_tireScreechSound.isPlaying)
                    {
                        _tireScreechSound.Play();
                    }
                }
                else if (!_isDrifting && (!_isTractionLocked || Mathf.Abs(_carSpeed) < 12f))
                {
                    _tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!_useSounds)
        {
            if (_carEngineSound != null && _carEngineSound.isPlaying)
            {
                _carEngineSound.Stop();
            }

            if (_tireScreechSound != null && _tireScreechSound.isPlaying)
            {
                _tireScreechSound.Stop();
            }
        }
    }

    private void TurnLeft()
    {
        _steeringAxis -= Time.deltaTime * 10f * _steeringSpeed;
        if (_steeringAxis < -1f)
        {
            _steeringAxis = -1f;
        }

        var steeringAngle = _steeringAxis * _maxSteeringAngle;
        _wheelColliderLp.steerAngle = Mathf.Lerp(_wheelColliderLp.steerAngle, steeringAngle, _steeringSpeed);
        _wheelColliderPp.steerAngle = Mathf.Lerp(_wheelColliderPp.steerAngle, steeringAngle, _steeringSpeed);
    }

    private void TurnRight()
    {
        _steeringAxis += (Time.deltaTime * 10f * _steeringSpeed);
        if (_steeringAxis > 1f)
        {
            _steeringAxis = 1f;
        }

        var steeringAngle = _steeringAxis * _maxSteeringAngle;
        _wheelColliderLp.steerAngle = Mathf.Lerp(_wheelColliderLp.steerAngle, steeringAngle, _steeringSpeed);
        _wheelColliderPp.steerAngle = Mathf.Lerp(_wheelColliderPp.steerAngle, steeringAngle, _steeringSpeed);
    }

    private void ResetSteeringAngle()
    {
        if (_steeringAxis < 0f)
        {
            _steeringAxis += Time.deltaTime * 10f * _steeringSpeed;
        }
        else if (_steeringAxis > 0f)
        {
            _steeringAxis -= Time.deltaTime * 10f * _steeringSpeed;
        }

        if (Mathf.Abs(_wheelColliderLp.steerAngle) < 1f)
        {
            _steeringAxis = 0f;
        }

        var steeringAngle = _steeringAxis * _maxSteeringAngle;
        _wheelColliderLp.steerAngle = Mathf.Lerp(_wheelColliderLp.steerAngle, steeringAngle, _steeringSpeed);
        _wheelColliderPp.steerAngle = Mathf.Lerp(_wheelColliderPp.steerAngle, steeringAngle, _steeringSpeed);
    }

    private void DriftCarPS()
    {
        if (_useEffects)
        {
            try
            {
                if (_isDrifting)
                {
                    _rlwParticleSystem.Play();
                    _rrwParticleSystem.Play();
                }
                else if (!_isDrifting)
                {
                    _rlwParticleSystem.Stop();
                    _rrwParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((_isTractionLocked || Mathf.Abs(_localVelocityX) > 5f) && Mathf.Abs(_carSpeed) > 12f)
                {
                    _rlwTireSkid.emitting = true;
                    _rrwTireSkid.emitting = true;
                }
                else
                {
                    _rlwTireSkid.emitting = false;
                    _rrwTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!_useEffects)
        {
            if (_rlwParticleSystem != null)
            {
                _rlwParticleSystem.Stop();
            }

            if (_rrwParticleSystem != null)
            {
                _rrwParticleSystem.Stop();
            }

            if (_rlwTireSkid != null)
            {
                _rlwTireSkid.emitting = false;
            }

            if (_rrwTireSkid != null)
            {
                _rrwTireSkid.emitting = false;
            }
        }
    }

    #region Crash
    
    private float _crashThreshold = 0.25f;
    private float _currentPosDelta = 0f;
    private float _lastDelta = 0f;
    private Vector3 _lastPosition = Vector3.zero;
    
    private void CrashSetting()
    {
        _lastPosition = transform.position;
    }
    
    private void CrashCar()
    {
        _currentPosDelta = (transform.position - _lastPosition).magnitude;
        var absDelta = Mathf.Abs(_currentPosDelta - _lastDelta);
        if (absDelta > 0.01f)
        {
            Debug.Log(absDelta);
        }
        if (absDelta > _crashThreshold)
        {
            G.Get<CarService>().Facade.Skeleton.CrashAll();
        }
        _lastPosition = transform.position;
        _lastDelta = _currentPosDelta;
    }

    #endregion
}