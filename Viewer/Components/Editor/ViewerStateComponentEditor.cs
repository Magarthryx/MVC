using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using NVYVE;

namespace NVYVE.MVC
{
    [CustomEditor(typeof(ViewerStateComponent), true)]
    public class ViewerStateComponentEditor : Editor
    {
        ViewerStateComponent _target;

        private static GUIContent SetButtonList = new GUIContent("Screen Button");

        private static GUIContent SetFocus = new GUIContent("Focus");
        private static GUIContent SetActive = new GUIContent("Active");
        private static GUIContent SetInactive = new GUIContent("Inactive");

        // Grab default color
        Color defaultColor;

        private void OnEnable()
        {
            defaultColor = GUI.color;
            _target = (ViewerStateComponent)target; // assign the associated class to this
        }

        public override void OnInspectorGUI()
        {
            // Grab the update function
            serializedObject.Update();

            // Bold font for buttons
            GUIStyle bold = new GUIStyle(EditorStyles.toolbarButton);
            bold.fontStyle = FontStyle.Bold;
            bold.fontSize = 10;

            // Jump down a line
            GUILayout.Label("");

            GUI.color = new Color(1f, 0.5f, 1f);

            GUILayout.Label(SetButtonList);

            GUILayout.Label("");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(SetFocus, bold))
            {
                _target.SetFocus();
            }
            if (GUILayout.Button(SetActive, bold))
            {
                _target.SetActive();
            }
            if (GUILayout.Button(SetInactive, bold))
            {
                _target.SetInactive();
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("____________________________________________\n");

            GUI.color = defaultColor;

            DrawDefaultInspector();
        }
    }
}