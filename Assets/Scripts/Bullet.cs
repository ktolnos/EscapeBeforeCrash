using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1000f;
    public GameObject bulletHitEffect;
    public float hitRadius = 5f;
    public float impactForce = 1000000000f;
    private void OnCollisionEnter()
    {
        foreach (var hit in Physics.SphereCastAll(transform.position, hitRadius, Vector3.up, hitRadius))
        {
            if (hit.collider.attachedRigidbody != null)
            {
                if (hit.transform.GetComponentInParent<Destroyable>() != null)
                {
                    hit.transform.GetComponent<Destroyable>().Destroy();
                }
                hit.collider.attachedRigidbody.AddForce((transform.position - hit.transform.position).normalized * impactForce, ForceMode.Impulse);
            }
        }

        Destroy(Instantiate(bulletHitEffect, transform.position, quaternion.identity), 100f);
        Destroy(gameObject);
    }
}
