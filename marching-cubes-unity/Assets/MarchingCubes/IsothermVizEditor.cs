using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IsothermViz))]
public class IsothermVizEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IsothermViz myScript = (IsothermViz)target;
        if (GUILayout.Button("Generate Mesh and Save"))
        {
            myScript.initScene();
        }
    }
}
