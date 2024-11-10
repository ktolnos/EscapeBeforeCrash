using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1000f;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles")){
            other.gameObject.GetComponent<Destroyable>().Destroy();
            Debug.Log("Bullet hit " + other.gameObject.name);
        }
    }
}
