using UnityEngine;
using UnityEditor;
using System.Collections;

namespace NVYVE.MVC
{
    [CustomEditor(typeof(ContentPanelFitter))]
    public class ContentPanelFitterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
