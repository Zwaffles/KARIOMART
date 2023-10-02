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

    private UnityEvent onAccelerate;
    private UnityEvent onBrake;

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                // Implement logic here eventually?
                break;
            case PlayerState.Accelerating:
                Accelerate();
                break;
            case PlayerState.Braking:
                Brake();
                break;
        }

        CheckInput();
    }

    private void CheckInput()
    {
        //if (onAccelerate.GetPersistentEventCount() > 0)
        //{
        //    currentState = PlayerState.Accelerating;
        //}
        //else if (onBrake.GetPersistentEventCount() > 0)
        //{
        //    currentState = PlayerState.Braking;
        //}
        //else
        //{
        //    currentState = PlayerState.Idle;
        //}

        if (rotY == 0)
            return;

        Steer(rotY);
    }


    private void Accelerate()
    {
        Debug.Log("luh acceleratshion");
    }

    private void Brake()
    {
        Debug.Log("luh breking");
    }

    private void Steer(float steer)
    {
        transform.rotation = Quaternion.Euler(0, rotY, 0);
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        onAccelerate.Invoke();
    }

    public void OnSteer(InputAction.CallbackContext ctx)
    {
        rotY += ctx.ReadValue<float>();
    }

    public void OnBrake(InputAction.CallbackContext ctx)
    {
        onBrake.Invoke();
    }
}
