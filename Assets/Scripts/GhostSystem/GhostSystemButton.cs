using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GhostSystem
{
    // This is an example class to handle the start of recording as well as the subsequent replay
    // In actual Gameplay, recording will happen as soon as player 1 race starts, and replay when player 2 race starts
    public class GhostDebugController : MonoBehaviour
    {
        [SerializeField] private Ghost ghost;
        [SerializeField] private GhostPlayer ghostPlayer;
        [SerializeField] private Button recordButton;

        private TextMeshProUGUI _recordButtonText;

        private void Start()
        {
            recordButton.onClick.AddListener(ToggleRecording);
            _recordButtonText = recordButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void ToggleRecording()
        {
            switch (ghost.currentState)
            {
                case GhostState.Idle:
                    StartRecording();
                    break;
                case GhostState.Recording:
                    StopRecording();
                    break;
                case GhostState.Replaying:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartRecording()
        {
            ghost.currentState = GhostState.Recording;

            _recordButtonText.text = "Stop Recording";
            recordButton.interactable = true;
        }

        private void StopRecording()
        {
            _recordButtonText.text = "Replaying...";
            recordButton.interactable = false;

            ghostPlayer.OnReplayComplete += HandleReplayComplete;
            ghost.currentState = GhostState.Replaying;
        }

        private void HandleReplayComplete()
        {
            ghost.currentState = GhostState.Idle;

            _recordButtonText.text = "Start Recording";
            recordButton.interactable = true;

            ghostPlayer.OnReplayComplete -= HandleReplayComplete;
        }
    }
}
