using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliPart : MonoBehaviour
{
    public Rigidbody parentRB;
    public float maxImpulse = 100f;

    private void OnCollisionEnter(Collision coll){
        float impulse = coll.impulse.magnitude - maxImpulse;
        print("impulse: " + impulse);
        if(impulse > 0){
            parentRB.GetComponent<PlayerController>().TakeDamage((int)impulse);
        }
    }
}
