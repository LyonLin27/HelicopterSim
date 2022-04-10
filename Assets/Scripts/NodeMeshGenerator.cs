using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
/// <summary>
/// Creates a mesh based on an input list of nodes.
/// </summary>
public class NodeMeshGenerator : MonoBehaviour
{
    [Header("Settings")]
    public float height = 2.0f;

    [Tooltip("Divide meshes in submeshes to generate more triangles")]
    [Range(0, 1)]
    public int subdivisionLevel;

    [Tooltip("Repeat the process this many times")]
    [Range(0, 2)] // dont EVER put this above 2
    public int timesToSubdivide;

    [Tooltip("Smoothing Iterations")]
    [Range(0, 2)] 
    public int smoothingIterations;

    [Header("References")]
    public Transform centerPoint;
    public List<GameObject> nodes = new List<GameObject>();

    private int[] subdivision = new int[] { 0, 2, 3, 4, 6, 8, 9, 12, 16, 18, 24 };
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private BossBlobController controller;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        controller = GetComponent<BossBlobController>();
        DefineNodesList();
    }

    void Start()
    {

    }


    void Update()
    {
        CalculateCenterPoint();
        CreateNodeMesh();
    }

    public void DefineNodesList()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Node node = transform.GetChild(i).GetComponent<Node>();
            if(node != null)
            {
                nodes.Add(node.gameObject);
                node.SetIndex(i - 1);
            }
        }
    }

    public void RemoveNode(GameObject node)
    {
        nodes.Remove(node);
        controller.RemoveNodeFromList(node);
        Destroy(node);
        ReassignIndexValues();
    }
    public void SplitNode(int index)
    {
        GameObject originalNode = nodes[index];
        int neighbor1 = 0;
        if(index + 1 >= nodes.Count)
        {
            neighbor1 = 0;
        }
        else
        {
            neighbor1 = index + 1;
        }
        int neighbor2 = 0;
        if(index - 1 < 0)
        {
            neighbor2 = nodes.Count - 1;
        }
        else
        {
            neighbor2 = index - 1;
        }
        Vector3 movedNodePosition = Vector3.Lerp(originalNode.transform.position, nodes[neighbor1].transform.position, 0.33f);
        Vector3 newNodePosition = Vector3.Lerp(originalNode.transform.position, nodes[neighbor2].transform.position, 0.33f);
        originalNode.GetComponent<NavMeshAgent>().Warp(movedNodePosition);
        GameObject duplicatedNode = Instantiate(originalNode, transform.position, Quaternion.identity, transform);
        duplicatedNode.GetComponent<NavMeshAgent>().Warp(newNodePosition);
        duplicatedNode.transform.SetParent(transform);
        duplicatedNode.transform.SetSiblingIndex(originalNode.transform.GetSiblingIndex());
        nodes.Insert(index, duplicatedNode);
        duplicatedNode.transform.position = newNodePosition;

        ReassignIndexValues();
        controller.InitializeSprings();
        controller.AssertAgentsList();
        controller.SetDestination(controller.GetCurrentDestination(), 5.0f);
    }
    public void ReassignIndexValues()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Node node = transform.GetChild(i).GetComponent<Node>();
            if (node != null)
            {
                node.SetIndex(i - 1);
            }
        }
    }

    public void CalculateCenterPoint()
    {
        Vector3 point = new Vector3(0, 0, 0);
        foreach(GameObject node in nodes)
        {
            point += node.transform.position;
        }
        point /= nodes.Count;
        point += new Vector3(0, height, 0);
        centerPoint.transform.position = point;
        centerPoint.GetChild(0).transform.localPosition = new Vector3(0, -height, 0);
    }

    void CreateNodeMesh()
    {
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
        List<Vector3> verts = new List<Vector3>();
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject nextNode = null;
            if (i + 1 < nodes.Count)
            {
                nextNode = nodes[i + 1];
            }
            else if (i + 1 >= nodes.Count)
            {
                nextNode = nodes[0];
            }

            // this node verts
            Vector3 node1_vert1 = nodes[i].transform.localPosition;
            Vector3 node1_vert2 = nodes[i].transform.localPosition + new Vector3(0, height / 2, 0);
            // next node verts

            Vector3 node2_vert1 = nextNode.transform.localPosition;
            Vector3 node2_vert2 = nextNode.transform.localPosition + new Vector3(0, height / 2, 0);

            verts.Add(node1_vert1);
            verts.Add(node1_vert2);
            verts.Add(node2_vert1);
            verts.Add(node2_vert2);

        }

        

        List<int> triangles = new List<int>();
        for (int nodeNum = 0; nodeNum < nodes.Count; nodeNum++) // calculate the two faces that each node provides.
        {
            int offset = nodeNum * 4;

            triangles.Add(offset + 2);
            triangles.Add(offset + 1);
            triangles.Add(offset);

            triangles.Add(offset + 1);
            triangles.Add(offset + 2);
            triangles.Add(offset + 3);
        }

        //add top vertex
        verts.Add(centerPoint.transform.localPosition);

        for (int nodeNum = 0; nodeNum < nodes.Count; nodeNum++) // calculate the two faces that each node provides.
        {
            int offset = nodeNum * 4;

            triangles.Add(verts.Count - 1);
            triangles.Add(offset + 1);
            triangles.Add(offset + 3);
        }




        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < verts.Count; i++)
        {
            uvs.Add(new Vector2(verts[i].x, verts[i].z));
        }

        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        SubdivideMesh();
        SmoothMesh();

        Mesh smoothedMesh = meshFilter.mesh;
        smoothedMesh.RecalculateTangents();
        smoothedMesh.RecalculateNormals(180);
        
        smoothedMesh.RecalculateBounds();
    }

    public void SubdivideMesh()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < timesToSubdivide; i++)
        {
            MeshHelper.Subdivide(mesh, subdivision[subdivisionLevel]);
        }
        meshFilter.mesh = mesh;
    }

    public void SmoothMesh()
    {
        // Clone the cloth mesh to work on
        Mesh sourceMesh = new Mesh();
        // Get the sourceMesh from the originalSkinnedMesh
        sourceMesh = meshFilter.mesh;
        // Clone the sourceMesh 
        Mesh workingMesh = CloneMesh(sourceMesh);
        // Reference workingMesh to see deformations
        meshFilter.mesh = workingMesh;


        // Apply Laplacian Smoothing Filter to Mesh
        for (int i = 0; i < smoothingIterations; i++)
            workingMesh.vertices = SmoothFilter.hcFilter(sourceMesh.vertices, workingMesh.vertices, workingMesh.triangles, 0.0f, 0.5f);
    }

    private static Mesh CloneMesh(Mesh mesh)
    {
        Mesh clone = new Mesh();
        clone.vertices = mesh.vertices;
        clone.normals = mesh.normals;
        clone.tangents = mesh.tangents;
        clone.triangles = mesh.triangles;
        clone.uv = mesh.uv;
        clone.uv2 = mesh.uv2;
        clone.uv3 = mesh.uv3;
        clone.bindposes = mesh.bindposes;
        clone.boneWeights = mesh.boneWeights;
        clone.bounds = mesh.bounds;
        clone.colors = mesh.colors;
        clone.name = mesh.name;
        
        return clone;
    }

    public List<GameObject> Nodes()
    {
        return nodes;
    }
}