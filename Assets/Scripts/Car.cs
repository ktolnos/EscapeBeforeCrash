using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum vehicleType{
    Gun,
    Shahid
}

public class Car: MonoBehaviour
{
    public Wheel frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider;
    public float torque = 1000f;
    public float randomTorque = 100f;
    public Transform seat;
    public vehicleType type;
    private const int NRaycasts = 10;
    private const float RaycastAngleMax = 30f;
    public Rigidbody rb;
    private IEnumerator _destroyCarEnumerator;
    public float minSpeed = 5f;
    private Player _player;
    private Outline _outline;
    private int _framesSinceHighlighted;
    private bool actionUsed = false;
    public float secondsBeforeDestroy = 2f;
    public GameObject fire;
    public GameObject explosion;
    public Transform debugSphere;
    public static int CarCount = 0;
    public static int MaxCars = 50;
    public float maxCameraDistance = 100f;
    public static float minCarT = 1f;
    public static GameObject lastCar;
    public float splineT;
    
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(0, 0, Mathf.Max(rb.linearVelocity.z, minSpeed));
        _outline = GetComponent<Outline>();
        StartCoroutine(Unhighlight());
        CarCount++;
    }

    public void Update()
    {
        Utils.GetNearestPointAndT(transform.position, splineT,
            out splineT, out var tangent);
        if ((transform.position - CameraController.Instance.transform.position).magnitude > maxCameraDistance)
        {
            Destroy(gameObject);
        }
        //Debug.Log(CarCount + " " + splineT + " " + minCarT + " " + (lastCar == this));

        if (splineT < minCarT && _player == null && splineT < CameraController.Instance.splineT)
        {
            // Debug.Log("Setting last car");
            lastCar = gameObject;
            minCarT = splineT;
        }

        // if (debugSphere != null)
        // {
        //     debugSphere.position = nearestPoint;
        //     debugSphere.forward = tangent;
        // }
        
        float trackAngle = Quaternion.FromToRotation(transform.forward, tangent).eulerAngles.y;
        var furthestAngle = trackAngle;
        // var furhtestDistance = 0f;
        // for (int i = -1; i < NRaycasts; i++)
        // {
        //     // iterate over angles starting from center and going to the sides
        //     var angle = trackAngle + i % 2 == 0 ? i * RaycastAngleMax / NRaycasts / 2 : -i * RaycastAngleMax / NRaycasts / 2;
        //     if (i == -1)
        //     {
        //         angle = 0;
        //     }
        //     var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
        //   
        //     if (Physics.Raycast(transform.position, direction, out var hit, 20f, 
        //             LayerMask.GetMask("Obstacles")))
        //     {
        //         if (hit.distance > furhtestDistance)
        //         {
        //             furhtestDistance = hit.distance;
        //             furthestAngle = angle;
        //         }
        //     }
        //     else if (!float.IsPositiveInfinity(furhtestDistance))
        //     {
        //         furhtestDistance = float.PositiveInfinity;
        //         furthestAngle = angle;
        //     }
        // }
        float steerAngle = furthestAngle;
        
        frontLeftWheelCollider.wheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.wheelCollider.steerAngle = steerAngle;

        frontLeftWheelCollider.wheelCollider.motorTorque = torque;
        frontRightWheelCollider.wheelCollider.motorTorque = torque;
        rearLeftWheelCollider.wheelCollider.motorTorque = torque;
        rearRightWheelCollider.wheelCollider.motorTorque = torque;
        
        if (rb.linearVelocity.magnitude <= minSpeed && _destroyCarEnumerator == null)
        {
            _destroyCarEnumerator = DestroyCar();
            StartCoroutine(_destroyCarEnumerator);
            fire.SetActive(true);
        }
        else if (_destroyCarEnumerator != null && rb.linearVelocity.magnitude > minSpeed)
        {
            StopCoroutine(_destroyCarEnumerator);
            _destroyCarEnumerator = null;
            fire.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        minCarT = 1f;
        if (CarCount > MaxCars && lastCar != null)
        {
            Destroy(lastCar);
            lastCar = null;
        }
    }

    private IEnumerator DestroyCar()
    {
        yield return new WaitForSeconds(secondsBeforeDestroy);
        StartCoroutine(AnimateExplosion());
    }
    
    private IEnumerator AnimateExplosion()
    {
        explosion.SetActive(true);
        if (_player != null)
        {
            _player.gameEnded = true;
        }
        transform.DOScale(Vector3.zero, 1f);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
        if (_player != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    public void Sit(Player player)
    {
        StartCoroutine(MovePlayer());
        _player = player;
    }
    
    public void Leave()
    {
        _player.transform.parent = null;
        _player = null;
        StopCoroutine(nameof(MovePlayer));
    }
    
    public void Highlight()
    {
        _outline.enabled = true;
        _framesSinceHighlighted = 0;
    }
    public void Action(){
        if(actionUsed){
            return;
        }
        if (type == vehicleType.Gun){
           GetComponentInChildren<Gun>().Shoot();
        }
        actionUsed = true;
    }

    
    private IEnumerator Unhighlight()
    {
        while (true)
        {
            yield return 0;
            if (_framesSinceHighlighted > 2)
            {
                _outline.enabled = false;
            }
            _framesSinceHighlighted++;
        }
    }
    
    private IEnumerator MovePlayer()
    {
        while (true)
        {
            yield return 0;
            if (_player != null)
            {
                _player.transform.position = Vector3.MoveTowards(
                    _player.transform.position, seat.position, (30f
                                                                + rb.linearVelocity.magnitude) * Time.deltaTime);
                if (_player.transform.position == seat.position)
                {
                    _player.transform.parent = seat;
                    yield break;
                }
            }
        }
    }
    
    public void OnDestroy()
    {
        CarCount--;
    }
}