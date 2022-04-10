using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobFood : MonoBehaviour
{
    public int nodeContribution; // how many times this object can split a node.

    private int timesSplit = 0;
    private float timer = 0.0f;
    private float absorbTimer = 1.0f; // how long a node has to stay in this trigger to absorb it.
    private GameObject currentNode;
    [HideInInspector] public GameObject currentMinionBlob;
    private float minionTimer = 0.0f;
    private float minionAbsorbTimer = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.AddBlobFood(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentNode != null)
        {
            if(currentNode.GetComponent<Node>().NodeMesh().Nodes().Count < 13)
            {
                timer += Time.deltaTime;
                if (timer > absorbTimer)
                {
                    currentNode.GetComponent<Node>().SplitNode();
                    timesSplit++;
                    if (timesSplit >= nodeContribution)
                    {
                        GameManager.instance.RemoveBlobFood(this);
                        Destroy(gameObject);
                    }
                    timer = 0.0f;
                }
            }
        }
        if(currentMinionBlob != null)
        {
            minionTimer += Time.deltaTime;
            if(minionTimer > minionAbsorbTimer)
            {
                currentMinionBlob.GetComponent<BlobController>().SetScale(currentMinionBlob.transform.localScale.x + 1);
                timesSplit++;
                if (timesSplit >= nodeContribution)
                {
                    GameManager.instance.RemoveBlobFood(this);
                    currentMinionBlob.GetComponent<BlobController>().GoToClosestFeature();
                    Destroy(gameObject);
                    
                }
                timer = 0.0f;
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(currentNode == null)
        {
            if(other.gameObject.layer == 11)
            {
                currentNode = other.gameObject.GetComponent<NodeTriggerRelay>().node.gameObject;
            }
        }
        if(currentMinionBlob == null)
        {
            if(other.gameObject.layer == 12)
            {
                if(other.gameObject.transform.localScale.x <= 15)
                {
                    currentMinionBlob = other.gameObject;
                }
                
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(currentNode != null)
        {
            if(other.gameObject.layer == 11)
            {
                if(other.gameObject.GetComponent<NodeTriggerRelay>().node.gameObject == currentNode)
                {
                    currentNode = null;
                }
            }
        }
        if (currentMinionBlob != null)
        {
            if (other.gameObject.layer == 12)
            {
                if (other.gameObject == currentMinionBlob)
                {
                    currentMinionBlob = null;
                }
            }
        }
    }

    public void EnableCollider()
    {
        GetComponent<Collider>().enabled = true;
    }
}
