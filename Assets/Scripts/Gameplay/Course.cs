using UnityEngine;
using System.Collections.Generic;

public class Course : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();

    private int totalCheckpoints;
    private int[] playerProgress;
    private GameManager gameManager;

    public Transform SpawnPoint;

    private void Start()
    {
        gameManager = GameManager.Instance;

        checkpoints.AddRange(GetComponentsInChildren<Checkpoint>());
        totalCheckpoints = checkpoints.Count;

        playerProgress = new int[gameManager.PlayerInputManager.playerCount];

        foreach (var checkpoint in checkpoints)
        {
            checkpoint.OnCheckpointReached += HandleCheckpointReached;
        }
    }

    private void HandleCheckpointReached(int playerNumber, Checkpoint checkpoint)
    {
        if (playerProgress[playerNumber] < totalCheckpoints)
        {
            if (checkpoint == checkpoints[playerProgress[playerNumber]])
            {
                playerProgress[playerNumber]++;
            }

            if (playerProgress[playerNumber] == totalCheckpoints)
            {
                gameManager.PlayerWon(playerNumber);
            }
        }
    }
}
