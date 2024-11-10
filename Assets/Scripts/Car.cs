using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Car: MonoBehaviour
{
    public Wheel frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider;
    public float torque = 1000f;
    public float randomTorque = 100f;
    public Transform seat;
    private const int NRaycasts = 10;
    private const float RaycastAngleMax = 30f;
    public Rigidbody rb;
    private IEnumerator _destroyCarEnumerator;
    public float minSpeed = 5f;
    private Player _player;
    private Outline _outline;
    private int _framesSinceHighlighted;
    public float secondsBeforeDestroy = 2f;
    public GameObject fire;
    public GameObject explosion;
    
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(0, 0, Mathf.Max(rb.linearVelocity.z, minSpeed));
        _outline = GetComponent<Outline>();
        StartCoroutine(Unhighlight());
    }

    public void Update()
    {
        torque += UnityEngine.Random.Range(-randomTorque, randomTorque) * Time.deltaTime;
        torque = Mathf.Clamp(torque, torque - randomTorque, torque + randomTorque);
        
        float steerAngle = 0;
      
        var furthestAngle = 0f;
        var furhtestDistance = 0f;
        for (int i = -1; i < NRaycasts; i++)
        {
            // iterate over angles starting from center and going to the sides
            var angle = i % 2 == 0 ? i * RaycastAngleMax / NRaycasts / 2 : -i * RaycastAngleMax / NRaycasts / 2;
            if (i == -1)
            {
                angle = 0;
            }
            var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
          
            if (Physics.Raycast(transform.position, direction, out var hit, 20f, 
                    LayerMask.GetMask("Default")))
            {
                if (hit.distance > furhtestDistance)
                {
                    furhtestDistance = hit.distance;
                    furthestAngle = angle;
                }
            }
            else if (!float.IsPositiveInfinity(furhtestDistance))
            {
                furhtestDistance = float.PositiveInfinity;
                furthestAngle = angle;
            }
        }
        steerAngle = furthestAngle * 1.1f;
        
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
        player.rb = rb;
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
                    _player.transform.position, seat.position, (20f
                                                                + rb.linearVelocity.magnitude) * Time.deltaTime);
                if (_player.transform.position == seat.position)
                {
                    _player.transform.parent = seat;
                    yield break;
                }
            }
        }
    }
}