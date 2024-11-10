using UnityEngine;

public class Wheel : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelTransform;
    private Vector3 _position;
    private Quaternion _rotation;
    private void Update()
    {
        wheelCollider.GetWorldPose(out _position, out _rotation);
        wheelTransform.position = _position;
        wheelTransform.rotation = _rotation;
        wheelCollider.motorTorque = 1000f;
    }
    
    
}