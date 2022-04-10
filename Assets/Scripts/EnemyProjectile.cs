using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10;
    public float impactForce = 100f;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Building>())
        {
            other.GetComponent<Building>().TakeDamage(damage);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
            other.GetComponent<PlayerController>().TakeImpact(transform.position, impactForce * transform.up);
        }
    }
}
