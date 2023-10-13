using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

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

    private GameObject _nextCoursePrefab = null;

    private int _currentCourseIndex = 0;

    [SerializeField] private PlayerInputManager playerInputManager;

    public PlayerInputManager PlayerInputManager => playerInputManager;

    [SerializeField] private Course[] courses;

    [SerializeField] private PlayerInput[] players;

    [SerializeField] private Canvas _victoryCanvas;

    [SerializeField] private float player2Offset = 3f;

    public float Player2Offset => player2Offset;

    [SerializeField] private float victoryMessageDuration = 3f;

    public Action<Course> OnLoadNextCourse;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
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

    public void HandlePlayerWinning(int playerNumber)
    {
        _playerProgress.Clear();

        DisplayVictoryMessage(playerNumber);
    }

    private void DisplayVictoryMessage(int playerNumber)
    {
        if (_victoryCanvas != null)
        {
            _victoryCanvas.GetComponentInChildren<TextMeshProUGUI>().text = $"Player {playerNumber + 1} Victory!";
            _victoryCanvas.enabled = true;
        }

        StartCoroutine(HideVictoryMessage());
    }

    private IEnumerator HideVictoryMessage()
    {
        yield return new WaitForSeconds(victoryMessageDuration);

        if (_victoryCanvas != null)
        {
            _victoryCanvas.enabled = false;
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
                if (_nextCoursePrefab != null)
                    Destroy(_nextCoursePrefab);
            }

            _nextCoursePrefab = Instantiate(courses[_currentCourseIndex].gameObject, Vector3.zero, Quaternion.identity);

            OnLoadNextCourse?.Invoke(courses[_currentCourseIndex]);
        }
    }

    public void InitializeSolo()
    {
        StartCoroutine(InitializeSoloCoroutine());
    }

    private IEnumerator InitializeSoloCoroutine()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        while (SceneManager.GetActiveScene().buildIndex != 1)
        {
            Debug.Log("Loading...");
            yield return null;
        }

        var player1 = Instantiate(players[0].gameObject, Vector3.zero, Quaternion.identity).GetComponent<PlayerInput>();

        player1.user.UnpairDevices();

        InputUser.PerformPairingWithDevice(Keyboard.current, user: player1.user);

        player1.user.ActivateControlScheme("Player1");

        InitializeCourse();
    }

    public void InitializeVersus()
    {
        StartCoroutine(InitializeVersusCoroutine());
    }

    public IEnumerator InitializeVersusCoroutine()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        while (SceneManager.GetActiveScene().buildIndex != 1)
        {
            Debug.Log("Loading...");
            yield return null;
        }

        var player1 = Instantiate(players[0].gameObject, Vector3.zero, Quaternion.identity).GetComponent<PlayerInput>();
        var player2 = Instantiate(players[1].gameObject, Vector3.zero, Quaternion.identity).GetComponent<PlayerInput>();

        player1.user.UnpairDevices();
        player2.user.UnpairDevices();

        InputUser.PerformPairingWithDevice(Keyboard.current, user: player1.user);
        InputUser.PerformPairingWithDevice(Keyboard.current, user: player2.user);

        player1.user.ActivateControlScheme("Player1");
        player2.user.ActivateControlScheme("Player2");

        InitializeCourse();
    }

    private void InitializeCourse()
    {
        _nextCoursePrefab = Instantiate(courses[_currentCourseIndex].gameObject, Vector3.zero, Quaternion.identity);

        OnLoadNextCourse(courses[_currentCourseIndex]);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}