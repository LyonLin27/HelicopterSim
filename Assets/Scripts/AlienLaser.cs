using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienLaser : EnemyProjectile
{
    public float speed = 300f;
    public ParticleSystem explosion;

    private void FixedUpdate() {
        if(GetComponent<Collider>().enabled)
            transform.position += transform.up * speed * Time.fixedDeltaTime;
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);

        explosion.Play();
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        Invoke("DestroyObject", 4f);
    }

    void DestroyObject() {
        Destroy(this.gameObject);
    }
}
