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

    private PlayerState currentState = PlayerState.Idle;

    private float steer = 0.0f;
    private float forwardVelocity = 0.0f;

    private bool acceleratePressed;
    private bool reversePressed;

    private Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();

        GameManager.Instance.OnLoadNextCourse += ResetPlayer;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLoadNextCourse -= ResetPlayer;
    }

    private void Update()
    {
        forwardVelocity = Vector3.Dot(rb.velocity, transform.forward);

        switch (currentState)
        {
            case PlayerState.Idle:
                if(forwardVelocity > 0)
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

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        if (rb.velocity.magnitude < 0.1f)
            rb.velocity = Vector3.zero;

        if (steer != 0)
            Steer();
    }

    private void UpdatePlayerState()
    {
        if (acceleratePressed && reversePressed)
        {
            currentState = PlayerState.Idle;
        }
        else if (acceleratePressed)
        {
            currentState = PlayerState.Accelerating;
        }
        else if (reversePressed)
        {
            currentState = PlayerState.Reversing;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void Idle()
    {
        Vector3 decelerationForce = -rb.velocity.normalized * decelerationRate;
        rb.AddForce(decelerationForce, ForceMode.Acceleration);
    }

    private void Accelerate()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
        }
    }

    private void Reverse()
    {
        if (forwardVelocity > 0)
        {
            rb.AddForce(-rb.velocity.normalized * brakeForce, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(-transform.forward * reverseForce, ForceMode.Acceleration);
        }
    }

    private void Steer()
    {
        if (currentState == PlayerState.Idle)
            return;

        float torque = (currentState == PlayerState.Reversing ? -steer : steer) * steerSpeed;

        float currentAngularVelocity = rb.angularVelocity.y;

        float newAngularVelocity = currentAngularVelocity + torque;

        if (Mathf.Abs(newAngularVelocity) <= maxAngularVelocity)
        {
            rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
        }
        else
        {
            if (Mathf.Sign(torque) != Mathf.Sign(currentAngularVelocity))
            {
                rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 bounceDirection = -collision.contacts[0].normal;

        rb.velocity = Vector3.Reflect(rb.velocity, bounceDirection) * bounceFactor;
    }

    private void ResetPlayer(Transform spawnPoint)
    {
        rb.velocity = Vector3.zero;
        steer = 0.0f;
        acceleratePressed = false;
        reversePressed = false;

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            acceleratePressed = true;
        }
        else if (ctx.canceled)
        {
            acceleratePressed = false;
        }

        UpdatePlayerState();
    }

    public void OnSteer(InputAction.CallbackContext ctx)
    {
        steer = ctx.ReadValue<float>();
    }

    public void OnReverse(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            reversePressed = true;
        }
        else if (ctx.canceled)
        {
            reversePressed = false;
        }

        UpdatePlayerState();
    }
}
