using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsRegistry", menuName = "CarsRegistry")]
public class CarsRegistry: ScriptableObject
{
    public CarProperties[] cars;
    
    [Serializable]
    public class CarProperties
    {
        public Car car;
        public float spawnChance = 1f;
    }
    
    public CarProperties Sample()
    {
        float total = 0;
        foreach (var car in cars)
        {
            total += car.spawnChance;
        }
        float randomPoint = UnityEngine.Random.value * total;
        foreach (var car in cars)
        {
            if (randomPoint < car.spawnChance)
            {
                return car;
            }
            randomPoint -= car.spawnChance;
        }
        return cars[0];
    }
        
}