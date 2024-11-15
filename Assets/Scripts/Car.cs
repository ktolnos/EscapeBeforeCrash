using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum vehicleType{
    Gun,
    Shahid,
    Boost,
    Jump,
}

public class Car: MonoBehaviour
{
    public Wheel frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider;
    public float torque = 1000f;
    public float randomTorque = 100f;
    public float maxSteerAngle = 30f;
    public Transform seat;
    public vehicleType type;
    private const int NRaycasts = 4;
    private const float RaycastLength = 40;
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
    public float splineT;
    public float behindCameraSpeedup = 5000f;
    private float _boostCooldown = 1f;
    public bool isBehindCamera;
    private static int _carsDestroyedThisFrame = 0;
    public float downforce = 100f;
    private float _timeScinceSpawned;
    
    
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * (minSpeed + 10f);
        _outline = GetComponent<Outline>();
        StartCoroutine(Unhighlight());
        CarCount++;
        torque = UnityEngine.Random.Range(torque - randomTorque, torque + randomTorque);
    }

    public void Update()
    {
        _timeScinceSpawned += Time.deltaTime;
        Utils.GetNearestPointAndT(transform.position + transform.forward * 6, splineT,
            out splineT, out var tangent);
        if ((transform.position - CameraController.Instance.transform.position).magnitude > maxCameraDistance)
        {
            Destroy(gameObject);
            _carsDestroyedThisFrame++;
        }

        isBehindCamera = splineT <
                             CameraController.Instance.splineT -
                             15f / CameraController.Instance.nativeSpline.GetLength();

        if (isBehindCamera &&
            rb.linearVelocity.sqrMagnitude < Player.Instance.car.rb.linearVelocity.sqrMagnitude &&
            CarCount - _carsDestroyedThisFrame >= MaxCars-1)
        {
            Destroy(gameObject);
            _carsDestroyedThisFrame++;
            return;
        }

        if (isBehindCamera && _boostCooldown > 0)
        {
            _boostCooldown -= Time.deltaTime;
        }
        else if (isBehindCamera && _boostCooldown <= 0)
        {
            _boostCooldown = 1f;
            rb.AddForce(transform.forward * behindCameraSpeedup, ForceMode.Impulse);
        }
        
        float trackAngle = Quaternion.FromToRotation(transform.forward, tangent).eulerAngles.y;
        var furthestAngle = trackAngle;
        var furhtestDistance = 0f;
        for (int i = -1; i < NRaycasts; i++)
        {
            // iterate over angles starting from center and going to the sides
            var angle = trackAngle + (i % 2 == 0 ? i * RaycastAngleMax / NRaycasts : -(i-1) * RaycastAngleMax / NRaycasts);
            if (i == -1)
            {
                angle = trackAngle;
            }
            var direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            var totalDistance = RaycastLength * 2f;
            if (Physics.Raycast(frontLeftWheelCollider.transform.position, direction, out var hit, RaycastLength, 
                    LayerMask.GetMask("Obstacles")))
            {
                totalDistance = hit.distance;
                // Debug.DrawRay(frontLeftWheelCollider.transform.position, direction * RaycastLength, Color.red);
            }
            
            if (Physics.Raycast(frontRightWheelCollider.transform.position, direction, out hit, RaycastLength, 
                LayerMask.GetMask("Obstacles")))
            {
                totalDistance = Mathf.Min(totalDistance, hit.distance);
                // Debug.DrawRay(frontRightWheelCollider.transform.position, direction * RaycastLength, Color.red);
            }
            
            if (totalDistance > furhtestDistance)
            {
                furhtestDistance = totalDistance;
                furthestAngle = angle;
                Debug.DrawRay(frontRightWheelCollider.transform.position, direction * RaycastLength, Color.green);
            }
        }
        float steerAngle = furthestAngle;
        steerAngle = (steerAngle + 3600f) % 360f;
        steerAngle = steerAngle > 180f ? steerAngle - 360f : steerAngle;
        steerAngle = Mathf.Clamp(steerAngle, -maxSteerAngle, maxSteerAngle);
        
        frontLeftWheelCollider.wheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.wheelCollider.steerAngle = steerAngle;

        frontLeftWheelCollider.wheelCollider.motorTorque = torque;
        frontRightWheelCollider.wheelCollider.motorTorque = torque;
        rearLeftWheelCollider.wheelCollider.motorTorque = torque;
        rearRightWheelCollider.wheelCollider.motorTorque = torque;
        
        if (rb.linearVelocity.magnitude <= minSpeed && _destroyCarEnumerator == null && _timeScinceSpawned > 5f)
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
        _carsDestroyedThisFrame = 0;
    }
    
    private void FixedUpdate()
    {
        rb.AddForce(-transform.up * downforce);
    }

    private IEnumerator DestroyCar()
    {
        yield return new WaitForSeconds(secondsBeforeDestroy);
        StartCoroutine(AnimateExplosion());
    }
    
    private IEnumerator AnimateExplosion()
    {
        explosion.SetActive(true);
        explosion.transform.parent = null;
        Destroy(explosion.gameObject, 2f);
        if (_player != null)
        {
            _player.gameEnded = true;
        }
        transform.DOScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(1);
        transform.DOKill();
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
    
    public void Highlight(Color color, Outline.Mode mode = Outline.Mode.OutlineAll)
    {
        _outline.enabled = true;
        _outline.OutlineColor = color;
        _outline.OutlineMode = mode;
        _framesSinceHighlighted = 0;
    }
    public void Action(){
        if(actionUsed){
            return;
        }
        if (type == vehicleType.Gun){
           GetComponentInChildren<Gun>().Shoot();
        }
        else if (type == vehicleType.Boost){
            GetComponentInChildren<Nitro>().boost(transform.forward);
        }
        else if (type == vehicleType.Jump){
            GetComponentInChildren<Nitro>().boost(Vector3.up);
        }
        actionUsed = true;
    }
    public void TurnCar(float direction)
    {
        this.frontLeftWheelCollider.wheelCollider.steerAngle = direction * maxSteerAngle;
        this.frontRightWheelCollider.wheelCollider.steerAngle = direction * maxSteerAngle;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish") && _player != null)
        {
            UIManager.Instance.ShowFinishedPanel();
        }   
    }

    public void OnDestroy()
    {
        transform.DOKill();
        CarCount--;
    }
}