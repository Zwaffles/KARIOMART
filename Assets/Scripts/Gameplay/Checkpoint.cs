using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Checkpoint : MonoBehaviour
{
    public event Action<int, Checkpoint> OnCheckpointReached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int playerNumber = other.GetComponentInParent<PlayerInput>().playerIndex;

            int playerProgress = GameManager.Instance.GetPlayerProgress(playerNumber);

            OnCheckpointReached?.Invoke(playerNumber, this);

            Debug.Log(playerNumber + " entered checkpoint!");
        }
    }
}
