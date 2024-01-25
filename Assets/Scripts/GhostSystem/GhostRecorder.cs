using System;
using UnityEngine;

namespace GhostSystem
{
    public class GhostRecorder : MonoBehaviour
    {
        [SerializeField] private Ghost ghost;
        private float _timer;
        private float _timeValue;

        private Transform _recorderTransform;

        private void Awake()
        {
            _recorderTransform = transform;

            if (!ghost.isRecording)
                return;

            ghost.ResetData();
            _timeValue = 0;
        }

        private void Update()
        {
            _timer += Time.unscaledDeltaTime;
            _timeValue += Time.unscaledDeltaTime;

            if (!ghost.isRecording || !(_timer >= 1 / ghost.recordFrequency)) 
                return;
            
            var ghostValues = new GhostValues(_timeValue, _recorderTransform.position, _recorderTransform.rotation);
            ghost.GhostValues.Add(ghostValues);

            _timer = 0;
        }
    }
}
