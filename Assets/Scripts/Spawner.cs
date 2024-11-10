using UnityEngine;

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
        if (_timer >= freqency)
        {
            _timer = 0;
            var spawnPosition = origin.position;
            spawnPosition.x += Random.Range(-xOffset, xOffset);
            var spawned = Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
            spawned.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, player.rb.linearVelocity.z + speedIncrease);
        }
    }
        
}