using UnityEngine;

public class Booster: MonoBehaviour
{
    public float power = 1000f;
    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Car") && other.attachedRigidbody != null)
        {
            other.attachedRigidbody.AddForce(transform.forward * power, ForceMode.Impulse);
        }
    }
}
