using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace NVYVE.WWW
{
	[InitializeOnLoad]
	[CustomEditor(typeof(WWWHandler))]
	public class WWWHandler_Editor: Editor
	{
		#region /- Variables -----------------------------------------------------------------------------------------------
		const string CLASSNAME						= "WWWHandler";
		WWWHandler _target;
		static MonoScript _target_script            = null;
		static MonoScript _editor_script            = null;


		#region /- GUI Styles ----------------------------------------------------------------------------------------------
		private bool isGUIInitialized                       = false;
		/*
		private static float miniButtonWidth                = 30f;
		private static GUIStyle miniButtonStyle;
		private static GUIStyle miniButtonLeftStyle;
		private static GUIStyle miniButtonMidStyle;
		private static GUIStyle miniButtonRightStyle;
		private static GUIContent moveDownButtonContent;
		private static GUIContent moveUpButtonContent;
		//private static GUIContent duplicateButtonContent;
		private static GUIContent addButtonContent;
		private static GUIContent deleteButtonContent;
		private static GUIContent expandAllButtonContent;
		private static GUIContent collapseAllButtonContent;
		private static GUIContent playButtonContent;
		*/
		#endregion
		#endregion

		#region /- Help Menu -----------------------------------------------------------------------------------------------
		[MenuItem("CONTEXT/" + CLASSNAME + "/Edit " + CLASSNAME + "_Editor Script")]
		static void Menu_EditScript(MenuCommand command) { AssetDatabase.OpenAsset(_editor_script); }
		[MenuItem("CONTEXT/" + CLASSNAME + "/Find " + CLASSNAME + " Script")]
		static void Menu_PingScript(MenuCommand command) { EditorGUIUtility.PingObject(_target_script); }
		[MenuItem("CONTEXT/" + CLASSNAME + "/Find " + CLASSNAME + " Editor Script")]
		static void Menu_PingEditorScript(MenuCommand command) { EditorGUIUtility.PingObject(_editor_script); }
		#endregion

		#region /- Constructor ---------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the <see cref="GameController_Editor"/> class on creation. It will register and run OnInitialize() 
		/// on the first Editor update. This is done as we want to defer execution until the rest has compiled and loaded,
		/// so we can get access to functions like FindObjectsOfType().
		/// </summary>
		static WWWHandler_Editor()
		{
			// Register OnInitialize() to run when the EditorApp updates.
			EditorApplication.update += OnInitialize;
		} // GameController_Editor()

		/// <summary>
		/// Runs the initialization event. This runs once whenever this class is loaded. Useful for automatically loading 
		/// properties onto this editor class on start, such as automatically setting build versions.
		/// </summary>
		static void OnInitialize()
		{
			// Do other important initializations here before the editor runs.

			// Deregister this function after it has run, so it only runs once on load.
			EditorApplication.update -= OnInitialize;
		} // OnInitialize()
		#endregion

		#region /- Initialization / Events ---------------------------------------------------------------------------------
		void OnEnable()
		{
			_target = (WWWHandler) target;
			_target_script = MonoScript.FromMonoBehaviour(_target);
			_editor_script = MonoScript.FromScriptableObject(this);

			isGUIInitialized = false;

            // JAMES SAVES
			//if (GameController.Instance != null)
			//{
			//	if (string.IsNullOrEmpty(GameController.properties.xml_savefile))
			//	{
			//		GameController.properties.SetSaveFilename(GameController.properties.GetSaveFilename(_target.gameObject));
			//	}
			//}
		} // OnEnable()
		#endregion

		#region /- OnInspectorGUI ------------------------------------------------------------------------------------------
		void InitializeGUI()
		{
			/*
			// GUIStyle/GUIContent assignments on Inspector panel open, can't run GUIStyles outside of an OnGUI() call
			// Button GUIStyles 
			miniButtonStyle = new GUIStyle(EditorStyles.miniButton);
			miniButtonLeftStyle			= new GUIStyle(EditorStyles.miniButtonLeft);
			miniButtonMidStyle			= new GUIStyle(EditorStyles.miniButtonMid);
			miniButtonRightStyle		= new GUIStyle(EditorStyles.miniButtonRight);

			// GUIContent buttons
			moveDownButtonContent		= new GUIContent("\u25bC", "Move Item Down");
			moveUpButtonContent			= new GUIContent("\u25b2", "Move Item Up");
			//duplicateButtonContent		= new GUIContent((Texture2D)EditorGUIUtility.Load("GUI/icons/icon_copy.png"), "Duplicate Item");
			addButtonContent			= new GUIContent("+", "Delete Item");
			deleteButtonContent			= new GUIContent("-", "Delete Item");
			//expandAllButtonContent		= new GUIContent("\u25bC", "Expand All Items");
			//collapseAllButtonContent	= new GUIContent("\u25b2", "Collapse All Items");
			playButtonContent			= new GUIContent(EditorGUIUtility.FindTexture("d_PlayButton"), "Set Item");
			*/
			// Other GUI Initializations here
			isGUIInitialized = true;
		} // InitializeGUI()

		public override void OnInspectorGUI()
		{
			if (!isGUIInitialized) InitializeGUI(); // init the GUIStyles

			// Update the object's properties after setting them the previous frame.
			serializedObject.Update();
			serializedObject.UpdateIfDirtyOrScript();

			NVYVE.InspectorLabelProperty_Drawer.DrawSeparator("Default Inspector", Color.white);
			DrawDefaultInspector();

			// Custom Inspector


			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			} // GUI.changed
		} // OnInspectorGUI()
		#endregion

	} // WWWHandler_Editor()

} // NVYVE