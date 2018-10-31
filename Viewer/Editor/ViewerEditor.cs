using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using NVYVE;
using NVYVE.MVC;

namespace NVYVE.Architecture
{
    [CustomEditor ( typeof( Viewer ), true )]
    public class ViewerEditor : Editor {
        Viewer _target;

        private static GUIContent PopulateHomeList = new GUIContent("Push Camera Data");
        private static GUIContent SetStateList = new GUIContent("Push Camera Data");
        private static GUIContent SetStateInList = new GUIContent("In");
        private static GUIContent SetStateOutList = new GUIContent("Out");

        public void OnEnable()
        {
            _target = (Viewer)target; // assign the associated class to this
        }
        
        public override void OnInspectorGUI ()
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

            GUI.color = new Color (0f, 0.95f, 0.85f);
            // If clicked, add the (HomeProperties)children of HomeParent to a list.
            if (GUILayout.Button(PopulateHomeList, bold))
            {
                //TODO NEW CAM
                //_target.cameraData.Load();
            }

            // Show Default Variables Warning
            GUILayout.Label ("____________________________________________\n");

            GUI.color = new Color(1f, 0.5f, 1f);

            GUILayout.Label(SetStateList);

            GUILayout.Label("");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(SetStateInList, bold))
            {
                _target.SetElements(true);
            }
            if (GUILayout.Button(SetStateOutList, bold))
            {
                _target.SetElements(false);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Label("____________________________________________\n");

            GUI.color = defaultColor;
            
            DrawDefaultInspector();
        }    	
    }
}
