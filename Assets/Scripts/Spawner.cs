using System;
using Unity.Mathematics;
using UnityEngine;
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

    public void Awake()
    {
        Car.CarCount = 0;
    }

    private void Start()
    {
        // Time.timeScale = 0.5f;
        for (int i = 0; i < initialCars; i++)
        {
            var spawnPosition = origin.position;
            spawnPosition.x += Random.Range(-xOffset, xOffset);
            spawnPosition.z += Random.Range(0, initialOffsetZ);
            var spawned = Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
            spawned.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, initialSpeed);
        }
    }
    
    public void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= freqency && Car.CarCount < Car.MaxCars)
        {
            _timer = 0;
            var spawnT = CameraController.Instance.splineT +
                         distanceFromCamera / CameraController.Instance.mainSpline.CalculateLength();
            Vector3 spawnPosition = CameraController.Instance.mainSpline.EvaluatePosition(spawnT);
            var tangent = CameraController.Instance.mainSpline.EvaluateTangent(spawnT);
            Quaternion rotation = Quaternion.LookRotation(tangent);
            var randomOffset = Random.Range(-xOffset, xOffset);
            if(Mathf.Abs(randomOffset)<3){
                return;
            }

            spawnPosition += CameraController.Instance.transform.right * randomOffset;
            var spawned = Instantiate(prefab, spawnPosition, rotation, parent);
            spawned.GetComponent<Rigidbody>().linearVelocity =
                spawned.transform.forward * (player.car.rb.linearVelocity.magnitude + speedIncrease);
            spawned.GetComponent<Car>().splineT = spawnT;
            
        }
    }
}