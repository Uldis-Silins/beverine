using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(RandomRotateChilds))]
public class RandomRotateChildsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Rotate"))
        {
            RandomRotateChilds rand = (RandomRotateChilds)target;
            rand.RandomRotate();
        }
    }
}
