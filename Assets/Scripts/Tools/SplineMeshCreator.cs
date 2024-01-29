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

    [SerializeField] private Transform collidersContainer;

    [SerializeField] private PhysicMaterial colliderPhysicMaterial;

    [SerializeField] private float width;
    [SerializeField] private float colliderWidth = 1.0f;
    [SerializeField] private float colliderHeight = 1.0f;

    [SerializeField] private int resolution;

    [SerializeField] private bool autoUpdate;

    public bool AutoUpdate => autoUpdate;

    private int _splineIndex;
    
    private float3 _forward;
    private float3 _position;
    private float3 _upVector;

    private List<Vector3> _verticesP1;
    private List<Vector3> _verticesP2;

    /// <summary>
    /// Invoked once the component is loaded or a value has been changed in the inspector
    /// </summary>
    public event Action OnValueChanged;

    private void Start()
    {
        BuildMesh();
    }

    private void CalculateSplineWidth(float time, out Vector3 p1, out Vector3 p2)
    {
        splineContainer.Evaluate(_splineIndex, time, out _position, out _forward, out _upVector);

        float3 right = Vector3.Cross(_forward, _upVector).normalized;
        p1 = _position + right * width;
        p2 = _position + -right * width;
    }

    private void GetVertices()
    {
        _verticesP1 = new List<Vector3>();
        _verticesP2 = new List<Vector3>();

        var step = 1f / resolution;
        for (var i = 0; i < resolution; i++)
        {
            var time = step * i;
            CalculateSplineWidth(time, out var p1, out var p2);
            _verticesP1.Add(p1);
            _verticesP2.Add(p2);
        }
    }

    public void BuildMesh()
    {
        GetVertices();

        var mesh = new Mesh();

        var vertices = new List<Vector3>();
        var tris = new List<int>();

        var length = _verticesP2.Count;

        var isClosed = splineContainer.Spline.Closed;

        // Iterates between vertices and builds faces
        for (var i = 0; i < length - (isClosed ? 0 : 1); i++)
        {
            var p1 = _verticesP1[i];
            var p2 = _verticesP2[i];

            if (!isClosed && i == length - 1)
            {
                // For open splines, skip the last segment
                break;
            }

            var p3 = _verticesP1[(i + 1) % length];
            var p4 = _verticesP2[(i + 1) % length];

            var offset = 4 * i;

            var t1 = offset + 0;
            var t2 = offset + 2;
            var t3 = offset + 3;

            var t4 = offset + 3;
            var t5 = offset + 1;
            var t6 = offset + 0;

            vertices.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();

        meshFilter.sharedMesh = mesh;
    }

    public void GenerateEdgeColliders()
    {
        if (collidersContainer == null)
        {
            Debug.LogError("RoadColliders GameObject not found. Make sure it's assigned in the Inspector.");
            return;
        }

        for (var i = collidersContainer.childCount; i > 0; --i)
            DestroyImmediate(collidersContainer.GetChild(0).gameObject);

        for (var i = 0; i < _verticesP1.Count - 1; i++)
        {
            var p1 = _verticesP1[i];
            var p2 = _verticesP1[i + 1];

            var colliderPosition = (p1 + p2) / 2f;

            var colliderParent = new GameObject("Collider_Inner_" + i);
            colliderParent.transform.SetParent(collidersContainer);
            colliderParent.transform.position = colliderPosition;

            var rotation = Quaternion.LookRotation(p2 - p1, Vector3.up);

            colliderParent.transform.rotation = rotation;

            var boxCollider = colliderParent.AddComponent<BoxCollider>();
            boxCollider.sharedMaterial = colliderPhysicMaterial;
            boxCollider.size = new Vector3(colliderWidth, colliderHeight, Vector3.Distance(p1, p2));
            boxCollider.center = new Vector3(-colliderWidth / 2f, 0, 0);
        }

        if (_verticesP1.Count > 1)
        {
            var lastP1 = _verticesP1[^1];
            var firstP1 = _verticesP1[0];

            var colliderPosition = (lastP1 + firstP1) / 2f;

            var colliderParent = new GameObject("Collider_Inner_Last");
            colliderParent.transform.SetParent(collidersContainer);
            colliderParent.transform.position = colliderPosition;

            var rotation = Quaternion.LookRotation(firstP1 - lastP1, Vector3.up);

            colliderParent.transform.rotation = rotation;

            var boxCollider = colliderParent.AddComponent<BoxCollider>();
            boxCollider.sharedMaterial = colliderPhysicMaterial;
            boxCollider.size = new Vector3(colliderWidth, colliderHeight, Vector3.Distance(lastP1, firstP1));
            boxCollider.center = new Vector3(-colliderWidth / 2f, 0, 0);
        }

        for (var i = 0; i < _verticesP2.Count - 1; i++)
        {
            var p1 = _verticesP2[i];
            var p2 = _verticesP2[i + 1];

            var colliderPosition = (p1 + p2) / 2f;

            var colliderParent = new GameObject("Collider_Outer_" + i);
            colliderParent.transform.SetParent(collidersContainer);
            colliderParent.transform.position = colliderPosition;

            var rotation = Quaternion.LookRotation(p2 - p1, Vector3.up);

            colliderParent.transform.rotation = rotation;

            var boxCollider = colliderParent.AddComponent<BoxCollider>();
            boxCollider.sharedMaterial = colliderPhysicMaterial;
            boxCollider.size = new Vector3(colliderWidth, colliderHeight, Vector3.Distance(p1, p2));
            boxCollider.center = new Vector3(colliderWidth / 2f, 0, 0);
        }

        if (_verticesP2.Count <= 1) return;
        {
            var lastP2 = _verticesP2[^1];
            var firstP2 = _verticesP2[0];

            var colliderPosition = (lastP2 + firstP2) / 2f;

            var colliderParent = new GameObject("Collider_Inner_Last");
            colliderParent.transform.SetParent(collidersContainer);
            colliderParent.transform.position = colliderPosition;

            var rotation = Quaternion.LookRotation(firstP2 - lastP2, Vector3.up);

            colliderParent.transform.rotation = rotation;

            var boxCollider = colliderParent.AddComponent<BoxCollider>();
            boxCollider.sharedMaterial = colliderPhysicMaterial;
            boxCollider.size = new Vector3(colliderWidth, colliderHeight, Vector3.Distance(lastP2, firstP2));
            boxCollider.center = new Vector3(colliderWidth / 2f, 0, 0);
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
#endif
}