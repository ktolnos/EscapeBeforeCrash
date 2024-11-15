using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletSpawn;
    public GameObject bullet;
    public GameObject effect;
    public AudioClip shotSound;
    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void Shoot()
    {
        audioSource.PlayOneShot(shotSound);
        Destroy(Instantiate(effect, bulletSpawn.transform.position, bulletSpawn.transform.rotation), 100f);
        Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation).GetComponent<Rigidbody>().AddForce(bulletSpawn.transform.forward * bullet.GetComponent<Bullet>().speed);
    }
}
