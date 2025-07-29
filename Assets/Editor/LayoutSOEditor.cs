using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutSO))]
public class LayoutSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LayoutSO myComponent = (LayoutSO)target;

        if (GUILayout.Button("Random LibraryCardDataSOs"))
        {
            myComponent.RandomizeLibraryCardDataSOs();
        }

        if (GUILayout.Button("Random GenerateLayout"))
        {
            myComponent.RandomGenerateLayout();
        }
    }
}
