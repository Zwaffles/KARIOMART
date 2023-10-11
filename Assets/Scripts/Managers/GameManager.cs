using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    private Dictionary<int, int> playerProgress = new Dictionary<int, int>();

    private int currentCourseIndex = 0;

    [SerializeField] private PlayerInputManager playerInputManager;

    [SerializeField] private Course[] courses;

    public PlayerInputManager PlayerInputManager => playerInputManager;

    public Canvas victoryCanvas;

    public Action<Transform> OnLoadNextCourse;

    public float victoryMessageDuration = 3f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetPlayerProgress(int playerNumber)
    {
        if (playerProgress.ContainsKey(playerNumber))
        {
            return playerProgress[playerNumber];
        }
        return -1;
    }

    public void SetPlayerProgress(int playerNumber, int progress)
    {
        if (playerProgress.ContainsKey(playerNumber))
        {
            playerProgress[playerNumber] = progress;
        }
    }

    public void PlayerWon(int playerNumber)
    {
        playerProgress.Clear();

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
        currentCourseIndex++;

        if (currentCourseIndex < courses.Length)
        {
            if (currentCourseIndex > 0)
            {
                courses[currentCourseIndex - 1].gameObject.SetActive(false);
            }

            courses[currentCourseIndex].gameObject.SetActive(true);

            OnLoadNextCourse(courses[currentCourseIndex].SpawnPoint);
        }
    }
}