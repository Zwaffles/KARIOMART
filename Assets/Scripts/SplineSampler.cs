using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSampler : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;

    [SerializeField] private int splineIndex;

    [SerializeField] private float width;

    private float3 _position;
    private float3 _forward;
    private float3 _upVector;

    public void SampleSplineWidth(float time, out Vector3 p1, out Vector3 p2)
    {
        splineContainer.Evaluate(splineIndex, time, out _position, out _forward, out _upVector);

        float3 right = Vector3.Cross(_forward, _upVector).normalized;
        p1 = _position + (right * width);
        p2 = _position + (-right * width);
    }
}
