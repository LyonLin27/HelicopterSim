using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{

	Node node;

	void OnEnable()
	{
		node = (Node)target;
	}

    public override void OnInspectorGUI()
    {
		DrawDefaultInspector();
		bool isPlaying = EditorApplication.isPlaying;
		EditorGUI.BeginDisabledGroup(!isPlaying);
		if (GUILayout.Button("Split"))
		{
			node.SplitNode();
		}
		if (GUILayout.Button("Delete"))
		{
			node.DeleteNode();
		}
		EditorGUI.EndDisabledGroup();
		
    }
}
