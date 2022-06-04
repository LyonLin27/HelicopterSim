using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : PlayerProjBase
{
    // visuals
    public List<MeshRenderer> _disableOnColl;
    public ParticleSystem _explosion;
    public ParticleSystem _smoke;

    // ref
    private Rigidbody _rb;
    [HideInInspector]public Transform _target;

    // parameters
    public float _spd = 50f;
    public float _delay = 0.5f;

    void Awake(){
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_target){
            transform.up = Vector3.MoveTowards(transform.up, _target.position - transform.position, Time.deltaTime * 10f);
        }
        _rb.velocity = transform.up * _spd;
        if(Time.time - _startTime > 7f && !_rb.isKinematic){
            SelfDestroy();
        }
    }

    public void InitMissile(int dmg, float spd, float delay, Transform target)
    {
        InitProjInfo(dmg);
        _spd = spd;
        _delay = delay;
        StartCoroutine(SetTargetWithDelay(target));
    }

    IEnumerator SetTargetWithDelay(Transform target){
        yield return new WaitForSeconds(_delay);
        _target = target;
    }

    public override void DespawnSequence()
    {
        foreach (MeshRenderer mr in _disableOnColl){
            mr.enabled = false;
        }
        _rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        _explosion.Play();
        _smoke.Stop();
        StartCoroutine(CleanUp());
    }

    private void SelfDestroy(){
        foreach(MeshRenderer mr in _disableOnColl){
            mr.enabled = false;
        }
        _rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        _smoke.Stop();
        StartCoroutine(CleanUp());
    }

    IEnumerator CleanUp(){
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

}
