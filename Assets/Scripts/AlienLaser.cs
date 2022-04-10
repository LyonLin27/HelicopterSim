using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienLaser : MonoBehaviour
{
    public ParticleSystem explosion;
    public float speed = 300f;
    public int damage = 10;
    private void FixedUpdate() {
        if(GetComponent<Collider>().enabled)
            transform.position += transform.up * speed * Time.fixedDeltaTime;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Building>()) {
            other.GetComponent<Building>().TakeLaserDamage(damage);
        }
        explosion.Play();
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        Invoke("DestroyObject", 4f);
    }

    void DestroyObject() {
        Destroy(this.gameObject);
    }
}
