using System;
using TMPro;
using UnityEngine;

public class Player: MonoBehaviour
{
    public Car car;
    public float radius = 20f;
    public TextMeshProUGUI speedText;
    public bool gameEnded;
    public static Player Instance;
    public float speed;
    
    public void Awake()
    {
        Instance = this;
    }
    
    public void Start()
    {
        car = GetComponentInParent<Car>();
        car.Sit(this);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            UIManager.Instance.ShowFinishedPanel();
        }   
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
        if (Input.GetButton("Horizontal"))
        {
            this.car.TurnCar(Input.GetAxis("Horizontal"));
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if ((Physics.Raycast(ray, out var hit, 200f, LayerMask.GetMask("Car"))
              || Physics.Raycast(ray, out hit, 200f, LayerMask.GetMask("CarSelect")))
            && hit.rigidbody != null && hit.rigidbody.TryGetComponent<Car>(out var car)
            && Vector3.Distance(car.transform.position, transform.position) < radius)
        {
            car.Highlight(Color.white);
            if (Input.GetMouseButtonDown(0))
            {
                this.car.Leave();
                this.car = car;
                car.Sit(this);
            }
            
        }

        speed = this.car.rb.linearVelocity.magnitude;
        speedText.text = $"{speed * 5:0} km/h";
        if (speed <= this.car.minSpeed)
        {
           speedText.color = Color.red;   
        }
        else
        {
            speedText.color = Color.white;
        }
        this.car.Highlight(Color.green, Outline.Mode.OutlineHidden);
    }
}