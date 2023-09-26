using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMeshCreator : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;

    [SerializeField] private MeshFilter meshFilter;

    [SerializeField] private float width;

    [SerializeField] private int resolution;

    [SerializeField] private bool autoUpdate;

    public bool AutoUpdate => autoUpdate;

    private int _splineIndex;

    private bool _subscribedToEvent;
    
    private float3 _forward;
    private float3 _position;
    private float3 _upVector;

    private List<Vector3> _vertsP1;
    private List<Vector3> _vertsP2;

    /// <summary>
    /// Invoked once the component is loaded or a value has been changed in the inspector
    /// </summary>
    public event Action OnValueChanged;
    
    private void CalculateSplineWidth(float time, out Vector3 p1, out Vector3 p2)
    {
        splineContainer.Evaluate(_splineIndex, time, out _position, out _forward, out _upVector);

        float3 right = Vector3.Cross(_forward, _upVector).normalized;
        p1 = _position + right * width;
        p2 = _position + -right * width;
    }

    private void GetVerts()
    {
        _vertsP1 = new List<Vector3>();
        _vertsP2 = new List<Vector3>();

        var step = 1f / resolution;
        for (var i = 0; i < resolution; i++)
        {
            var time = step * i;
            CalculateSplineWidth(time, out var p1, out var p2);
            _vertsP1.Add(p1);
            _vertsP2.Add(p2);
        }
    }

    public void BuildMesh()
    {
        GetVerts();

        var mesh = new Mesh();

        var verts = new List<Vector3>();
        var tris = new List<int>();

        var length = _vertsP2.Count;

        // Iterates between verts and builds faces
        for (var i = 1; i <= length; i++)
        {
            var p1 = _vertsP1[i - 1];
            var p2 = _vertsP2[i - 1];
            Vector3 p3;
            Vector3 p4;

            if (i == length)
            {
                p3 = _vertsP1[0];
                p4 = _vertsP2[0];
            }
            else
            {
                p3 = _vertsP1[i];
                p4 = _vertsP2[i];
            }

            var offset = 4 * (i - 1);

            var t1 = offset + 0;
            var t2 = offset + 2;
            var t3 = offset + 3;

            var t4 = offset + 3;
            var t5 = offset + 1;
            var t6 = offset + 0;

            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();

            meshFilter.sharedMesh = mesh;
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.delayCall += _OnValidate;
    }

    private void _OnValidate()
    {
        OnValueChanged?.Invoke();
    }
}
#endif