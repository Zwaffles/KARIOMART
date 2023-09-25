using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRoadCreator : MonoBehaviour
{
    [SerializeField]
    private SplineContainer splineContainer;

    [SerializeField]
    private int splineIndex;

    [SerializeField]
    private float width;

    private float3 position;
    private float3 forward;
    private float3 upVector;

    private readonly float time;

    void Update()
    {
        splineContainer.Evaluate(splineIndex, time, out position, out forward, out upVector);

        float3 right = Vector3.Cross(forward, upVector).normalized;
        var p1 = position + (right * width);
        var p2 = position + (-right * width);
    }
}
