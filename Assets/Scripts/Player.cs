using System;
using TMPro;
using UnityEngine;

public class Player: MonoBehaviour
{
    public Rigidbody rb;
    public Car car;
    public float radius = 20f;
    public TextMeshProUGUI speedText;
    public bool gameEnded;
    
    public void Start()
    {
        car = GetComponentInParent<Car>();
        car.Sit(this);
    }
    
    public void Update()
    {
        if (gameEnded)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1)){
            this.car.Action();
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if ((Physics.Raycast(ray, out var hit, 100f, LayerMask.GetMask("Car"))
              || Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("CarSelect")))
            && hit.rigidbody != null && hit.rigidbody.TryGetComponent<Car>(out var car)
            && Vector3.Distance(car.transform.position, transform.position) < radius)
        {
            car.Highlight();
            if (Input.GetMouseButtonDown(0))
            {
                this.car.Leave();
                this.car = car;
                car.Sit(this);
            }
            
        }

        var speed = rb.linearVelocity.magnitude;
        speedText.text = $"{speed * 10:0} km/h";
        if (speed <= this.car.minSpeed)
        {
           speedText.color = Color.red;   
        }
        else
        {
            speedText.color = Color.white;
        }
    }
}