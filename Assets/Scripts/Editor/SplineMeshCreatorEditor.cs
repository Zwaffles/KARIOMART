using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

namespace Editor
{
    [CustomEditor(typeof(SplineMeshCreator))]
    public class SplineMeshCreatorEditor : UnityEditor.Editor
    {
        private SplineMeshCreator _splineMeshCreator;
    
        private void Awake()
        {
            _splineMeshCreator = target as SplineMeshCreator;
        }

        private void OnEnable()
        {
            EditorSplineUtility.AfterSplineWasModified += OnSplineChanged;
            _splineMeshCreator.OnValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            EditorSplineUtility.AfterSplineWasModified -= OnSplineChanged;
            _splineMeshCreator.OnValueChanged -= OnValueChanged;
        }
    
        private void OnSplineChanged(Spline spline)
        {
            if (!_splineMeshCreator.AutoUpdate)
                return;
            
            Debug.Log("Spline form has been changed");
            
            _splineMeshCreator.BuildMesh();
        }

        private void OnValueChanged()
        {
            if (!_splineMeshCreator.AutoUpdate)
                return;
    
            Debug.Log("Spline values have been changed");
            
            _splineMeshCreator.BuildMesh();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!GUILayout.Button("Spawn Edge Objects")) 
                return;
            
            var splineMeshCreator = (SplineMeshCreator)target;
            splineMeshCreator.SpawnEdgePrefabs();
        }
    }
}
