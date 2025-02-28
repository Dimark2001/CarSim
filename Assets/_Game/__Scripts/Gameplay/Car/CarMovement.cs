using System;
using UnityEngine;
using UnityEngine.UI;

public class CarMovement : MonoBehaviour
{
    [Space(20)]
    [Range(20, 190)]
    public int maxSpeed = 90;

    [Range(10, 120)]
    public int maxReverseSpeed = 45;

    [Range(1, 10)]
    public int accelerationMultiplier = 2;

    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27;

    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f;

    [Space(10)]
    [Range(100, 600)]
    public int brakeForce = 350;

    [Range(1, 10)]
    public int decelerationMultiplier = 2;

    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5;

    [Space(10)]
    public Vector3 bodyMassCenter;

    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;

    [Space(10)]
    public GameObject frontRightMesh;

    public WheelCollider frontRightCollider;

    [Space(10)]
    public GameObject rearLeftMesh;

    public WheelCollider rearLeftCollider;

    [Space(10)]
    public GameObject rearRightMesh;

    public WheelCollider rearRightCollider;

    [Space(20)]
    public bool useEffects = false;

    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    [Space(10)]
    public TrailRenderer RLWTireSkid;

    public TrailRenderer RRWTireSkid;

    [Space(10)]
    public bool useUI = false;

    public Text carSpeedText;

    [Space(20)]
    public bool useSounds = false;

    public AudioSource carEngineSound;
    public AudioSource tireScreechSound;

    private float _initialCarEngineSoundPitch;

    public GameObject throttleButton;
    public GameObject reverseButton;
    public GameObject turnRightButton;
    public GameObject turnLeftButton;
    public GameObject handbrakeButton;

    [HideInInspector]
    public float carSpeed;

    [HideInInspector]
    public bool isDrifting;

    [HideInInspector]
    public bool isTractionLocked;

    public Rigidbody _carRigidbody;
    private float _steeringAxis;
    private float _throttleAxis;
    private float _driftingAxis;
    private float _localVelocityZ;
    private float _localVelocityX;
    private bool _deceleratingCar;

    private bool _touchControlsSetup = false;

    private WheelFrictionCurve _fLwheelFriction;
    private float _flWextremumSlip;
    private WheelFrictionCurve _fRwheelFriction;
    private float _frWextremumSlip;
    private WheelFrictionCurve _rLwheelFriction;
    private float _rlWextremumSlip;
    private WheelFrictionCurve _rRwheelFriction;
    private float _rrWextremumSlip;

    public Vector2 MoveDirection;
    public bool IsHandbrake;

    private void Awake()
    {
        _carRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _carRigidbody = GetComponent<Rigidbody>();
        _carRigidbody.centerOfMass = bodyMassCenter;

        _fLwheelFriction = new WheelFrictionCurve
        {
            extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip,
            extremumValue = frontLeftCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue,
            stiffness = frontLeftCollider.sidewaysFriction.stiffness
        };
        _flWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;

        _fRwheelFriction = new WheelFrictionCurve
        {
            extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip,
            extremumValue = frontRightCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue,
            stiffness = frontRightCollider.sidewaysFriction.stiffness
        };
        _frWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;

        _rLwheelFriction = new WheelFrictionCurve
        {
            extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip,
            extremumValue = rearLeftCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue,
            stiffness = rearLeftCollider.sidewaysFriction.stiffness
        };
        _rlWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;

        _rRwheelFriction = new WheelFrictionCurve
        {
            extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip,
            extremumValue = rearRightCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue,
            stiffness = rearRightCollider.sidewaysFriction.stiffness
        };
        _rrWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;

        if (carEngineSound != null)
        {
            _initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        if (useUI)
        {
            InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
        }
        else if (!useUI && carSpeedText != null)
        {
            carSpeedText.text = "0";
        }

        if (useSounds)
        {
            InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }
        else if (!useSounds)
        {
            if (carEngineSound != null)
            {
                carEngineSound.Stop();
            }

            if (tireScreechSound != null)
            {
                tireScreechSound.Stop();
            }
        }

        if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }

            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }

            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }

            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }
    }

    public void Move()
    {
        carSpeed = 2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60 / 1000;

        _carRigidbody ??= GetComponent<Rigidbody>();
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

        if (MoveDirection.x == 0)
        {
            ThrottleOff();
        }

        if (MoveDirection.y == 0 && !IsHandbrake && !_deceleratingCar)
        {
            InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
            _deceleratingCar = true;
        }

        if (MoveDirection.x == 0 && _steeringAxis != 0f)
        {
            ResetSteeringAngle();
        }

        AnimateWheelMeshes();
    }

    private void CarSpeedUI()
    {
        if (useUI)
        {
            try
            {
                float absoluteCarSpeed = Mathf.Abs(carSpeed);
                carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
    }

    private void CarSounds()
    {
        if (useSounds)
        {
            try
            {
                if (carEngineSound != null)
                {
                    float engineSoundPitch = _initialCarEngineSoundPitch +
                                             (Mathf.Abs(_carRigidbody.linearVelocity.magnitude) / 25f);
                    carEngineSound.pitch = engineSoundPitch;
                }

                if (isDrifting || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                {
                    if (!tireScreechSound.isPlaying)
                    {
                        tireScreechSound.Play();
                    }
                }
                else if (!isDrifting && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                {
                    tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useSounds)
        {
            if (carEngineSound != null && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }

            if (tireScreechSound != null && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }
    }

    private void TurnLeft()
    {
        _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (_steeringAxis < -1f)
        {
            _steeringAxis = -1f;
        }

        float steeringAngle = _steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void TurnRight()
    {
        _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (_steeringAxis > 1f)
        {
            _steeringAxis = 1f;
        }

        float steeringAngle = _steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void ResetSteeringAngle()
    {
        if (_steeringAxis < 0f)
        {
            _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (_steeringAxis > 0f)
        {
            _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }

        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            _steeringAxis = 0f;
        }

        float steeringAngle = _steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void AnimateWheelMeshes()
    {
        try
        {
            Quaternion FLWRotation;
            Vector3 FLWPosition;
            frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
            frontLeftMesh.transform.position = FLWPosition;
            frontLeftMesh.transform.rotation = FLWRotation;

            Quaternion FRWRotation;
            Vector3 FRWPosition;
            frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
            frontRightMesh.transform.position = FRWPosition;
            frontRightMesh.transform.rotation = FRWRotation;

            Quaternion RLWRotation;
            Vector3 RLWPosition;
            rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
            rearLeftMesh.transform.position = RLWPosition;
            rearLeftMesh.transform.rotation = RLWRotation;

            Quaternion RRWRotation;
            Vector3 RRWPosition;
            rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
            rearRightMesh.transform.position = RRWPosition;
            rearRightMesh.transform.rotation = RRWRotation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    private void GoForward()
    {
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
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
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void GoReverse()
    {
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
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
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    public void DecelerateCar()
    {
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
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

        _carRigidbody.linearVelocity *= 1f / (1f + (0.025f * decelerationMultiplier));
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
        if (_carRigidbody.linearVelocity.magnitude < 0.25f)
        {
            _carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    public void Handbrake()
    {
        CancelInvoke(nameof(DecelerateCar));
        _deceleratingCar = false;
        CancelInvoke("RecoverTraction");
        _driftingAxis += Time.deltaTime;
        float secureStartingPoint = _driftingAxis * _flWextremumSlip * handbrakeDriftMultiplier;

        if (secureStartingPoint < _flWextremumSlip)
        {
            _driftingAxis = _flWextremumSlip / (_flWextremumSlip * handbrakeDriftMultiplier);
        }

        if (_driftingAxis > 1f)
        {
            _driftingAxis = 1f;
        }

        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        if (_driftingAxis < 1f)
        {
            _fLwheelFriction.extremumSlip = _flWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontLeftCollider.sidewaysFriction = _fLwheelFriction;

            _fRwheelFriction.extremumSlip = _frWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontRightCollider.sidewaysFriction = _fRwheelFriction;

            _rLwheelFriction.extremumSlip = _rlWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearLeftCollider.sidewaysFriction = _rLwheelFriction;

            _rRwheelFriction.extremumSlip = _rrWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearRightCollider.sidewaysFriction = _rRwheelFriction;
        }

        isTractionLocked = true;
        DriftCarPS();
    }

    public void DriftCarPS()
    {
        if (useEffects)
        {
            try
            {
                if (isDrifting)
                {
                    RLWParticleSystem.Play();
                    RRWParticleSystem.Play();
                }
                else if (!isDrifting)
                {
                    RLWParticleSystem.Stop();
                    RRWParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((isTractionLocked || Mathf.Abs(_localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
                {
                    RLWTireSkid.emitting = true;
                    RRWTireSkid.emitting = true;
                }
                else
                {
                    RLWTireSkid.emitting = false;
                    RRWTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }

            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }

            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }

            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }
    }

    public void RecoverTraction()
    {
        isTractionLocked = false;
        _driftingAxis = _driftingAxis - (Time.deltaTime / 1.5f);
        if (_driftingAxis < 0f)
        {
            _driftingAxis = 0f;
        }

        if (_fLwheelFriction.extremumSlip > _flWextremumSlip)
        {
            _fLwheelFriction.extremumSlip = _flWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontLeftCollider.sidewaysFriction = _fLwheelFriction;

            _fRwheelFriction.extremumSlip = _frWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontRightCollider.sidewaysFriction = _fRwheelFriction;

            _rLwheelFriction.extremumSlip = _rlWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearLeftCollider.sidewaysFriction = _rLwheelFriction;

            _rRwheelFriction.extremumSlip = _rrWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearRightCollider.sidewaysFriction = _rRwheelFriction;

            Invoke("RecoverTraction", Time.deltaTime);
        }
        else if (_fLwheelFriction.extremumSlip < _flWextremumSlip)
        {
            _fLwheelFriction.extremumSlip = _flWextremumSlip;
            frontLeftCollider.sidewaysFriction = _fLwheelFriction;

            _fRwheelFriction.extremumSlip = _frWextremumSlip;
            frontRightCollider.sidewaysFriction = _fRwheelFriction;

            _rLwheelFriction.extremumSlip = _rlWextremumSlip;
            rearLeftCollider.sidewaysFriction = _rLwheelFriction;

            _rRwheelFriction.extremumSlip = _rrWextremumSlip;
            rearRightCollider.sidewaysFriction = _rRwheelFriction;

            _driftingAxis = 0f;
        }
    }
}