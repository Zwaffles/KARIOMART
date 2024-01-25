using System.Collections.Generic;
using UnityEngine;

namespace GhostSystem
{
    public class GhostPlayer : MonoBehaviour
    {
        [SerializeField] private Ghost ghost;

        private float _timeValue;
        private int _index1;
        private int _index2;

        private Transform _playerTransform;
        private List<GhostValues> _ghostValues;

        private void Awake()
        {
            _playerTransform = transform;
            _timeValue = 0;
        }

        private void Update()
        {
            if (!ghost.isReplaying) 
                return;
            
            _timeValue += Time.unscaledDeltaTime;
            
            GetIndex();

            if (_index1 != _index2)
            {
                UpdateTransform();
            }
        }

        private void GetIndex()
        {
            for (var i = 0; i < ghost.GhostValues.Count - 1; i++)
            {
                if (ghost.GhostValues[i].TimeStamp <= _timeValue && _timeValue <= ghost.GhostValues[i + 1].TimeStamp)
                {
                    _index1 = i;
                    _index2 = i + 1;
                    return;
                }
            }

            _index1 = ghost.GhostValues.Count - 1;
            _index2 = ghost.GhostValues.Count - 1;
        }

        private void UpdateTransform()
        {
            var timestamp1 = ghost.GhostValues[_index1].TimeStamp;
            var timestamp2 = ghost.GhostValues[_index2].TimeStamp;

            var interpolationFactor = (_timeValue - timestamp1) / (timestamp2 - timestamp1);

            _playerTransform.position = Vector3.Lerp(ghost.GhostValues[_index1].Position,
                ghost.GhostValues[_index2].Position, interpolationFactor);

            _playerTransform.rotation = Quaternion.Slerp(ghost.GhostValues[_index1].Rotation,
                ghost.GhostValues[_index2].Rotation, interpolationFactor);
        }

    }
}
