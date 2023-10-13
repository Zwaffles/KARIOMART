using UnityEngine;
using System.Collections.Generic;

public class Course : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();

    private int _totalCheckpoints;

    private int[] _playerProgress;

    private GameManager _gameManager;

    public Transform SpawnPoint;

    private void OnEnable()
    {
        _gameManager = GameManager.Instance;

        _totalCheckpoints = checkpoints.Count;

        _playerProgress = new int[_gameManager.PlayerInputManager.playerCount];

        foreach (var checkpoint in checkpoints)
        {
            checkpoint.OnCheckpointReached += HandleCheckpointReached;
        }
    }

    private void OnDisable()
    {
        foreach (var checkpoint in checkpoints)
        {
            checkpoint.OnCheckpointReached -= HandleCheckpointReached;
        }
    }

    private void HandleCheckpointReached(int playerNumber, Checkpoint checkpoint)
    {
        if (_playerProgress[playerNumber] < _totalCheckpoints)
        {
            if (checkpoint == checkpoints[_playerProgress[playerNumber]])
            {
                Debug.Log(playerNumber + " entered checkpoint!");
                _playerProgress[playerNumber]++;
            }

            if (_playerProgress[playerNumber] == _totalCheckpoints)
            {
                Debug.Log(playerNumber + " won! calling " +_gameManager.name);
                _gameManager.HandlePlayerWinning(playerNumber);
            }
        }
    }
}
