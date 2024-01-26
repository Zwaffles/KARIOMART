using UnityEngine;

namespace GhostSystem
{
    // This is an example class showing how you could subscribe to the replay complete event
    // For example, you want to move on or do something once a replay has finished
    public class ReplayListener : MonoBehaviour
    {
        [SerializeField] private GhostPlayer ghostPlayer;

        private void Start()
        {
            ghostPlayer.OnReplayComplete += HandleReplayComplete;
        }

        private static void HandleReplayComplete()
        {
            Debug.Log("Replay is complete!");
        }

        private void OnDestroy()
        {
            ghostPlayer.OnReplayComplete -= HandleReplayComplete;
        }
    }
}