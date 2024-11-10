using UnityEngine;

public class Booster: MonoBehaviour
{
    public float power = 1000f;
    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Car")){
            var carRB = other.GetComponentInParent<Rigidbody>();
            carRB.AddForce(transform.forward * power);
            Debug.Log("boosted");   
        }
    }
}
