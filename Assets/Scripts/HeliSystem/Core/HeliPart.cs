using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * pass damage and impulse to heli
 * 
 */
public class HeliPart : MonoBehaviour
{
    public Rigidbody parentRB;

    private void OnCollisionEnter(Collision coll)
    {
        Debug.Log("part hit " + transform.name);
    }
}
