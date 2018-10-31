using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using NVYVE;
using NVYVE.MVC;

namespace NVYVE.Architecture
{
    [CustomEditor(typeof(ViewerButton), true)]
    public class ViewerButtonEditor : Editor
    {
        ViewerButton _target;
        
        private static GUIContent SetButtonList = new GUIContent("Screen Button");

        private static GUIContent SetFocusList = new GUIContent("Focus");
        private static GUIContent SetUnfocusList = new GUIContent("Unfocus");
        private static GUIContent SetActiveList = new GUIContent("Active");
        private static GUIContent SetInactiveList = new GUIContent("Inactive");
        
        private void OnEnable()
        {
            _target = (ViewerButton)target; // assign the associated class to this
        }

        public override void OnInspectorGUI()
        {
            Color defaultColor = GUI.color;

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
            if (GUILayout.Button(SetFocusList, bold))
            {
                _target.SetFocus();
            }
            if (GUILayout.Button(SetUnfocusList, bold))
            {
                _target.SetUnfocus();
            }
            if (GUILayout.Button(SetActiveList, bold))
            {
                _target.SetActive();
            }
            if (GUILayout.Button(SetInactiveList, bold))
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