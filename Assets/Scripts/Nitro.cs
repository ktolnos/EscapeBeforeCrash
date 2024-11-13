using UnityEngine;

public class Nitro : MonoBehaviour
{
    public GameObject effect;
    public float power = 50000f;
    private Rigidbody rb;
    public AudioClip nitroSound;
    private AudioSource audioSource;

    public void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        audioSource = GetComponentInParent<AudioSource>();
    }
    
    public void boost()
    {
        audioSource.PlayOneShot(nitroSound);
        rb.AddForce(transform.forward * power, ForceMode.Impulse);
        Destroy(Instantiate(effect, transform.position, transform.rotation), 100f);
    }
}
