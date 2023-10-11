using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Accelerating,
        Reversing
    }

    private PlayerState _currentState = PlayerState.Idle;

    private float _steer = 0.0f;
    private float _forwardVelocity = 0.0f;

    private bool _acceleratePressed;
    private bool _reversePressed;

    private Rigidbody _rb;

    [SerializeField] private float steerSpeed = 10f;
    [SerializeField] private float accelerationForce = 10f;
    [SerializeField] private float brakeForce = 10f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float decelerationRate = 1f;
    [SerializeField] private float reverseForce = 10f;
    [SerializeField] private float maxAngularVelocity = 10f;
    [SerializeField] private float bounceFactor = 0.5f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        GameManager.Instance.OnLoadNextCourse += ResetPlayer;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLoadNextCourse -= ResetPlayer;
    }

    private void Update()
    {
        _forwardVelocity = Vector3.Dot(_rb.velocity, transform.forward);

        switch (_currentState)
        {
            case PlayerState.Idle:
                if(_forwardVelocity > 0)
                    Idle();
                else
                    Debug.Log("Car is idling");
                break;
            case PlayerState.Accelerating:
                Debug.Log("Car is accelerating");
                Accelerate();
                break;
            case PlayerState.Reversing:
                Debug.Log("Car is reversing");
                Reverse();
                break;
        }

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed);

        if (_rb.velocity.magnitude < 0.1f)
            _rb.velocity = Vector3.zero;

        if (_steer != 0)
            Steer();
    }

    private void UpdatePlayerState()
    {
        if (_acceleratePressed && _reversePressed)
        {
            _currentState = PlayerState.Idle;
        }
        else if (_acceleratePressed)
        {
            _currentState = PlayerState.Accelerating;
        }
        else if (_reversePressed)
        {
            _currentState = PlayerState.Reversing;
        }
        else
        {
            _currentState = PlayerState.Idle;
        }
    }

    private void Idle()
    {
        Vector3 decelerationForce = -_rb.velocity.normalized * decelerationRate;
        _rb.AddForce(decelerationForce, ForceMode.Acceleration);
    }

    private void Accelerate()
    {
        if (_rb.velocity.magnitude < maxSpeed)
        {
            _rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
        }
    }

    private void Reverse()
    {
        if (_forwardVelocity > 0)
        {
            _rb.AddForce(-_rb.velocity.normalized * brakeForce, ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(-transform.forward * reverseForce, ForceMode.Acceleration);
        }
    }

    private void Steer()
    {
        if (_currentState == PlayerState.Idle)
            return;

        float torque = (_currentState == PlayerState.Reversing ? -_steer : _steer) * steerSpeed;

        float currentAngularVelocity = _rb.angularVelocity.y;

        float newAngularVelocity = currentAngularVelocity + torque;

        if (Mathf.Abs(newAngularVelocity) <= maxAngularVelocity)
        {
            _rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
        }
        else
        {
            if (Mathf.Sign(torque) != Mathf.Sign(currentAngularVelocity))
            {
                _rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 bounceDirection = -collision.contacts[0].normal;

        _rb.velocity = Vector3.Reflect(_rb.velocity, bounceDirection) * bounceFactor;
    }

    private void ResetPlayer(Transform spawnPoint)
    {
        _rb.velocity = Vector3.zero;
        _steer = 0.0f;
        _acceleratePressed = false;
        _reversePressed = false;

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _acceleratePressed = true;
        }
        else if (ctx.canceled)
        {
            _acceleratePressed = false;
        }

        UpdatePlayerState();
    }

    public void OnSteer(InputAction.CallbackContext ctx)
    {
        _steer = ctx.ReadValue<float>();
    }

    public void OnReverse(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _reversePressed = true;
        }
        else if (ctx.canceled)
        {
            _reversePressed = false;
        }

        UpdatePlayerState();
    }
}
