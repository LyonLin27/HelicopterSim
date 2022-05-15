using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody heliRB;
    private float bulletSpd = 100f;

    public void Fire() {
        GameObject bullet = WeaponPool.instance.GetBullet();
        bullet.SetActive(true);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.GetComponent<Rigidbody>().velocity = heliRB.velocity + bullet.transform.forward * bulletSpd;
    }

    public void SetupWeapon(float bulletSpd_in)
    {
        bulletSpd = bulletSpd_in;
    }
}
