using System.Linq;
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

        
        // //Broken function, fix this before shipping
        //  private void OnSceneGUI()
        //  {
        //      if (!_splineMeshCreator.AutoUpdate)
        //          return;
        //      
        //      EditorGUI.BeginChangeCheck();
        //
        //      var knotsList = _splineMeshCreator.Spline.Knots.ToList();
        //
        //      // Display handles for each knot width
        //      for (var i = 0; i < knotsList.Count; i++)
        //      {
        //          var newWidth = Handles.ScaleValueHandle(
        //              _splineMeshCreator.Widths[i], // Current width
        //              knotsList[i].Position,
        //              Quaternion.identity,
        //              HandleUtility.GetHandleSize(knotsList[i].Position),
        //              Handles.CircleHandleCap, // Use DotHandleCap for a spherical handle
        //              1f
        //          );
        //
        //          if (!EditorGUI.EndChangeCheck())
        //              continue;
        //
        //          // If the handle is moved, update the width
        //          _splineMeshCreator.Widths[i] = Mathf.Max(newWidth, Mathf.Epsilon); // Ensure width is positive
        //
        //          // You can also add a small value to prevent getting stuck at very low values
        //          if (_splineMeshCreator.Widths[i] < 0.01f)
        //          {
        //              _splineMeshCreator.Widths[i] = 0.01f;
        //          }
        //
        //          _splineMeshCreator.BuildMesh(); // Rebuild the mesh
        //      }
        //  }
    }
}
