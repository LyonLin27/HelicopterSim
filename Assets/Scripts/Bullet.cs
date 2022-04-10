using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    private float timer = 0;
    private void OnEnable() {
        timer = Time.time;
    }

    private void Update() {
        transform.forward = GetComponent<Rigidbody>().velocity.normalized;
        if (Time.time - timer > 1.5f) {
            gameObject.SetActive(false);
        }
    }

    public void OnCollisionEnter(Collision collision) {
        GameObject ps = WeaponPool.instance.GetBulletPS();
        ps.transform.position = transform.position;
        ps.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
    }
}
