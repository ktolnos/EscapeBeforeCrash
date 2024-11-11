using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Utils
{
    public static void GetNearestPointAndT(Vector3 point, float lastT, out float t, out Vector3 tangent)
    {
        Vector3 localSplinePoint = CameraController.Instance.mainSpline.transform.InverseTransformPoint(point);
        GetNearestPoint(CameraController.Instance.nativeSpline, localSplinePoint, out float3 nearestPoint3, out t, lastT);
        // nearestPoint = CameraController.Instance.mainSpline.transform.TransformPoint(nearestPoint3);
        tangent = SplineUtility.EvaluateTangent(CameraController.Instance.nativeSpline, t);
    }
    
    public static float GetNearestPoint<T>(T spline,
        float3 point,
        out float3 nearest,
        out float t,
        float lastT,
        int resolution = SplineUtility.PickResolutionDefault,
        int iterations = 2) where T : ISpline
    {
        float distance = float.PositiveInfinity;
        nearest = float.PositiveInfinity;
        Segment segment = new Segment(lastT, 0.001f);
        t = 0f;

        for (int i = 0, c = iterations; i < c; i++)
        {
            int segments = 5;
            segment = GetNearestPoint(spline, point, segment, out distance, out nearest, out t, segments);
        }

        return distance;
    }
    
    static Segment GetNearestPoint<T>(T spline,
        float3 point,
        Segment range,
        out float distance, out float3 nearest, out float time,
        int segments) where T : ISpline
    {
        distance = float.PositiveInfinity;
        nearest = float.PositiveInfinity;
        time = float.PositiveInfinity;
        Segment segment = new Segment(-1f, 0f);

        float t0 = range.start;
        float3 a = SplineUtility.EvaluatePosition(spline, t0);


        for (int i = 1; i < segments; i++)
        {
            float t1 = range.start + (range.length * (i / (segments - 1f)));
            float3 b = SplineUtility.EvaluatePosition(spline, t1);
            var p = SplineMath.PointLineNearestPoint(point, a, b, out var lineParam);
            float dsqr = math.distancesq(p, point);

            if (dsqr < distance)
            {
                segment.start = t0;
                segment.length = t1 - t0;
                time = segment.start + segment.length * lineParam;
                distance = dsqr;

                nearest = p;
            }

            t0 = t1;
            a = b;
        }

        distance = math.sqrt(distance);
        return segment;
    }
    
    struct Segment
    {
        public float start, length;

        public Segment(float start, float length)
        {
            this.start = start;
            this.length = length;
        }
    }

        
}