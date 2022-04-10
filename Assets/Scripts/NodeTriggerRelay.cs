using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTriggerRelay : MonoBehaviour
{
    public Node node;
    private void OnTriggerEnter(Collider other)
    {
        node.OnNodeHit(other);
    }
}
