using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;

public class CameraController : MonoBehaviour
{
    private Vector3 _offset;
    public SplineContainer mainSpline;
    public static CameraController Instance;
    public float splineT;
    public NativeSpline nativeSpline;
    
    void OnEnable()
    {
        nativeSpline = new NativeSpline(mainSpline.Spline, Unity.Collections.Allocator.Persistent);
        Vector3 localSplinePoint = mainSpline.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(nativeSpline, localSplinePoint, out float3 nearestPoint3, out splineT);
    }
    void OnDisable()
    {
        nativeSpline.Dispose();
    }
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //_offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform.position = Player.Instance.transform.position + _offset;
        Utils.GetNearestPointAndT(transform.position,  splineT, out splineT, out var tangent);
        transform.forward = tangent;
    }
}
