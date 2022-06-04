using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    public float bulletSpd = 100f;
    public int bulletDmg = 20;

    [HideInInspector]
    public Rigidbody heliRB;

    public void Fire() {
        GameObject bullet = WeaponPool.instance.GetBullet();
        bullet.SetActive(true);
        bullet.GetComponent<Bullet>().InitBullet(bulletDmg);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.GetComponent<Rigidbody>().velocity = heliRB.velocity + bullet.transform.forward * bulletSpd;
    }

    public void SetupWeapon(float bulletSpd_in)
    {
        bulletSpd = bulletSpd_in;
    }
}
