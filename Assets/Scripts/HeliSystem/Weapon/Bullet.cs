using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PlayerProjBase
{
    public void InitBullet(int dmg)
    {
        InitProjInfo(dmg);
    }

    private void Update() {
        transform.forward = GetComponent<Rigidbody>().velocity.normalized;
        if (Time.time - _startTime > 1.5f) {
            gameObject.SetActive(false);
        }
    }

    public override void DespawnSequence() {
        GameObject ps = WeaponPool.instance.GetBulletPS();
        ps.transform.position = transform.position;
        ps.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
    }
}
