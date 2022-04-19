using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public int damage = 50;
    public List<MeshRenderer> disableOnColl;
    public ParticleSystem explosion;
    public ParticleSystem smoke;
    public Transform target;
    public float spd = 50f;
    public float delay = 0.5f;
    private Rigidbody rb;
    private float startTime;

    void Awake(){
        rb = GetComponent<Rigidbody>(); 
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(target){
            transform.up = Vector3.MoveTowards(transform.up, target.position - transform.position, Time.deltaTime * 10f);
        }
        rb.velocity = transform.up * spd;
        if(Time.time - startTime > 7f && !rb.isKinematic){
            SelfDestroy();
        }
    }

    public void SetTarget(Transform _target){
        StartCoroutine(SetTargetWithDelay(_target));
    }

    IEnumerator SetTargetWithDelay(Transform _target){
        yield return new WaitForSeconds(delay);
        target = _target;
    }

    public void OnCollisionEnter(Collision collision) {
        foreach(MeshRenderer mr in disableOnColl){
            mr.enabled = false;
        }
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        explosion.Play();
        smoke.Stop();
        StartCoroutine(CleanUp());
    }

    private void SelfDestroy(){
        foreach(MeshRenderer mr in disableOnColl){
            mr.enabled = false;
        }
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        smoke.Stop();
        StartCoroutine(CleanUp());
    }

    IEnumerator CleanUp(){
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

}
