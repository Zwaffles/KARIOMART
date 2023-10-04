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
        Braking
    }

    private PlayerState currentState = PlayerState.Idle;

    private float rotY = 0.0f;
    private float steer = 0.0f;

    private bool acceleratePressed;
    private bool brakePressed;

    private Rigidbody rb;

    [SerializeField] private float steerSpeed = 30f;
    [SerializeField] private float accelerationForce = 10f;
    [SerializeField] private float brakingForce = 10f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float decelerationRate = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                Idle();
                break;
            case PlayerState.Accelerating:
                Accelerate();
                break;
            case PlayerState.Braking:
                Brake();
                break;
        }

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        if (steer != 0)
            Steer();
    }

    private void UpdatePlayerState()
    {
        if (acceleratePressed && brakePressed)
        {
            currentState = PlayerState.Idle;
        }
        else if (acceleratePressed)
        {
            currentState = PlayerState.Accelerating;
        }
        else if (brakePressed)
        {
            currentState = PlayerState.Braking;
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

    private void Brake()
    {
        Vector3 reverseForce = -rb.velocity.normalized * brakingForce;
        rb.AddForce(reverseForce, ForceMode.Acceleration);
    }

    private void Steer()
    {
        if (currentState == PlayerState.Idle)
            return;

        rotY += steer * steerSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rotY, 0);
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

    public void OnBrake(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            brakePressed = true;
        }
        else if (ctx.canceled)
        {
            brakePressed = false;
        }

        UpdatePlayerState();
    }
}
