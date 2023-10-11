using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    private Dictionary<int, int> _playerProgress = new Dictionary<int, int>();

    private int _currentCourseIndex = 0;

    [SerializeField] private PlayerInputManager playerInputManager;

    [SerializeField] private Course[] courses;

    public PlayerInputManager PlayerInputManager => playerInputManager;

    public Canvas victoryCanvas;

    public Action<Transform> OnLoadNextCourse;

    public float victoryMessageDuration = 3f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetPlayerProgress(int playerNumber)
    {
        if (_playerProgress.ContainsKey(playerNumber))
        {
            return _playerProgress[playerNumber];
        }
        return -1;
    }

    public void SetPlayerProgress(int playerNumber, int progress)
    {
        if (_playerProgress.ContainsKey(playerNumber))
        {
            _playerProgress[playerNumber] = progress;
        }
    }

    public void PlayerWon(int playerNumber)
    {
        _playerProgress.Clear();

        DisplayVictoryMessage(playerNumber);
    }

    private void DisplayVictoryMessage(int playerNumber)
    {
        if (victoryCanvas != null)
        {
            victoryCanvas.GetComponentInChildren<TextMeshProUGUI>().text = $"Player {playerNumber + 1} Victory!";
            victoryCanvas.enabled = true;
            StartCoroutine(HideVictoryMessage());
        }
    }

    private IEnumerator HideVictoryMessage()
    {
        yield return new WaitForSeconds(victoryMessageDuration);

        if (victoryCanvas != null)
        {
            victoryCanvas.enabled = false;
        }

        LoadNextCourse();
    }

    private void LoadNextCourse()
    {
        _currentCourseIndex++;

        if (_currentCourseIndex < courses.Length)
        {
            if (_currentCourseIndex > 0)
            {
                courses[_currentCourseIndex - 1].gameObject.SetActive(false);
            }

            courses[_currentCourseIndex].gameObject.SetActive(true);

            OnLoadNextCourse(courses[_currentCourseIndex].SpawnPoint);
        }
    }
}