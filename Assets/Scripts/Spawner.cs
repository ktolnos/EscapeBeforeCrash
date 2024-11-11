using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class Spawner: MonoBehaviour
{
    public GameObject prefab;
    public Transform parent;
    public Transform origin;
    public float freqency = 1f;
    private float _timer;
    public float xOffset = 25f;
    public float initialOffsetZ = 225f;
    public int initialCars = 50;
    public float initialSpeed = 5f;
    public float speedIncrease = 5f;
    public Player player;
    public float distanceFromCamera;
    
    private bool isFirstUpdate = true;

    public void Awake()
    {
        Car.CarCount = 0;
    }
    
    public void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            for (int i = 0; i < initialCars; i++)
            {
                SpawnCar(Random.Range(0, initialOffsetZ), initialSpeed);
            }
            return;
        }
        _timer += Time.deltaTime;
        if (_timer >= freqency && Car.CarCount <= Car.MaxCars)
        {
            if (SpawnCar(distanceFromCamera,
                    Mathf.Max(player.car.rb.linearVelocity.magnitude, initialSpeed) + speedIncrease))
            {
                _timer = 0;
            }
        }
    }
    
    private bool SpawnCar(float distanceFromCameraLocal, float speed)
    {
        var spawnT = CameraController.Instance.splineT +
                     distanceFromCameraLocal / CameraController.Instance.nativeSpline.GetLength();
        Vector3 spawnPosition = CameraController.Instance.nativeSpline.EvaluatePosition(spawnT);
        spawnPosition = CameraController.Instance.mainSpline.transform.TransformPoint(spawnPosition);
        Vector3 tangent = CameraController.Instance.nativeSpline.EvaluateTangent(spawnT);
        tangent = tangent.normalized;
        
        Quaternion rotation = Quaternion.LookRotation(tangent);
        var randomOffset = Random.Range(-xOffset, xOffset);
        spawnPosition += Quaternion.Euler(0, 90, 0) * tangent * randomOffset;
        if (Physics.Raycast(spawnPosition, tangent , out var hit, 
                30f, LayerMask.GetMask("CarSelect", "Car", "Obstacles")))
        {
            return false;
        }
        var spawned = Instantiate(prefab, spawnPosition, rotation, parent);
        spawned.GetComponent<Rigidbody>().linearVelocity =
            spawned.transform.forward * speed;
        spawned.GetComponent<Car>().splineT = spawnT;
        return true;
    }
}