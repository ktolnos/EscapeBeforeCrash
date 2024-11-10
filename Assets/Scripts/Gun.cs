using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletSpawn;
    public GameObject bullet;

    public void Shoot()
    {
        Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation).GetComponent<Rigidbody>().AddForce(bulletSpawn.transform.forward * bullet.GetComponent<Bullet>().speed);
    }
}
