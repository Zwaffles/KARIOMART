using System.Collections.Generic;
using UnityEngine;

namespace GhostSystem
{
    public struct GhostValues
    {
        public readonly float TimeStamp;
        public Vector3 Position;
        public Quaternion Rotation;

        public GhostValues(float timeStamp, Vector3 position, Quaternion rotation)
        {
            this.TimeStamp = timeStamp;
            this.Position = position;
            this.Rotation = rotation;
        }
    }
    
    [CreateAssetMenu(fileName = "GhostObject", menuName = "GhostSystem/Ghost Object")]
    public class Ghost : ScriptableObject
    {
        public readonly List<GhostValues> GhostValues = new List<GhostValues>();

        public bool isRecording;
        public bool isReplaying;
        public float recordFrequency;

        public void ResetData()
        {
            GhostValues.Clear();
        }
    }
}
