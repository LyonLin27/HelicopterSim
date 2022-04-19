using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody heliRB;

    public void Fire(Transform target = null) {
        //GameObject bullet = WeaponPool.instance.GetBullet();
        //bullet.SetActive(true);
        GameObject bullet = Instantiate(WeaponPool.instance.MissilePrefab);
        bullet.GetComponent<Missile>().SetTarget(target);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        //bullet.transform.up = transform.forward;
        bullet.GetComponent<Rigidbody>().velocity = heliRB.velocity;
    }
}
