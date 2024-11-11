using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public GameObject deathEffect;
    public void Destroy()
    {
        Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity), 100f);
        Destroy(gameObject);
    }
}
