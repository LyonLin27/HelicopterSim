using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPool : MonoBehaviour
{
    static public WeaponPool instance;
    public GameObject BulletPrefab;
    public GameObject MissilePrefab;
    public int bulletCount = 256;
    public GameObject BulletPS;
    public int bulletPSCount = 8;

    private GameObject[] bullets;
    private int bulletIndex = 0;

    private GameObject[] bulletPSs;
    private int bulletPSIndex = 0;

    private void Awake() {
        if (instance) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }

        bullets = new GameObject[bulletCount];
        for (int i = 0; i < bulletCount; i++) {
            GameObject b = Instantiate(BulletPrefab, transform);
            b.SetActive(false);
            bullets[i] = b;
        }

        bulletPSs = new GameObject[bulletPSCount];
        for (int i = 0; i < bulletPSCount; i++) {
            GameObject b = Instantiate(BulletPS, transform);
            bulletPSs[i] = b;
        }
    }

    public GameObject GetBullet() {
        GameObject bullet = bullets[bulletIndex];
        bulletIndex++;
        if (bulletIndex >= bulletCount) {
            bulletIndex = 0;
        }

        return bullet;
    }

    public GameObject GetBulletPS() {
        GameObject ps = bulletPSs[bulletPSIndex];
        bulletPSIndex++;

        if (bulletPSIndex >= bulletPSCount) {
            bulletPSIndex = 0;
        }

        return ps;
    }
}
