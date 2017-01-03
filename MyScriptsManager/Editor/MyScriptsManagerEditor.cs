using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;


public class MyScriptsManagerEditor : EditorWindow {
	private Hashtable sets = new Hashtable();
	[MenuItem("[Master_Tools]/Scripts Manager")]
	static void Init() {

		GetWindow(typeof(MyScriptsManagerEditor));
		GetWindow(typeof(MyScriptsManagerEditor)).minSize = new Vector2(250,200);
		GetWindow(typeof(MyScriptsManagerEditor)).titleContent = new GUIContent("All Scripts v0.1");
	}
	public MyScriptsManagerEditor()
	{
		
	}

	GUIStyle styleHelpboxInner;
	GUIStyle titleLabel,editorAddedButtonStyle,normalButtonStyle;
	Texture editButtonIcon,saveButtonIcon;
	void InitStyles()
	{
		editButtonIcon = Resources.Load("toolPencil") as Texture;
		styleHelpboxInner = new GUIStyle("HelpBox");
		styleHelpboxInner.padding = new RectOffset(6, 6, 6, 6);

		titleLabel = new GUIStyle();
		titleLabel.fontSize = 10;
		titleLabel.normal.textColor = Color.white;
		titleLabel.alignment = TextAnchor.UpperCenter;
		titleLabel.fixedHeight = 15;

		editorAddedButtonStyle = new GUIStyle(GUI.skin.button);
		editorAddedButtonStyle.alignment = TextAnchor.MiddleLeft;
		editorAddedButtonStyle.normal.textColor = Color.yellow;

		normalButtonStyle = new GUIStyle(GUI.skin.button);
		normalButtonStyle.alignment = TextAnchor.MiddleLeft;

	}

	public void UpdateList()
	{
		Object[] objects;

		sets.Clear();
		GUILayout.BeginHorizontal(styleHelpboxInner);
		GUILayout.FlexibleSpace();
		GUILayout.Label("All scripts used in current scene",titleLabel);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		objects = FindObjectsOfTypeAll( typeof( Component ) );
		foreach( Component component in objects )
		{
			if(component.GetType().BaseType.ToString().Equals("UnityEngine.MonoBehaviour"))
			{
				if( !sets.ContainsKey( component.GetType() ) )
				{
					sets[ component.GetType() ] = new ArrayList();
				}
				( ( ArrayList )sets[ component.GetType() ] ).Add( component.gameObject );
			}
		}
	
	}
	List<string> pathOfScripts;
	void OnGUI() 
	{
		InitStyles();
		styleHelpboxInner = new GUIStyle("HelpBox");
		styleHelpboxInner.padding = new RectOffset(6, 6, 6, 6);
		UpdateList();
		GUILayout.BeginVertical(styleHelpboxInner);
		string[] guids = AssetDatabase.FindAssets ("t:Script");
		pathOfScripts = new List<string>();
		foreach (var itemPath in guids) 
		{
			pathOfScripts.Add(AssetDatabase.GUIDToAssetPath(itemPath));
		}

	

		foreach(System.Type type in sets.Keys)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			string ext = "cs";
			if(!getPathOfFile(type.Name).Equals("NaN"))
			{
				string[] tempArrString = getPathOfFile(type.Name).Split('.');
				ext = tempArrString[1];
			}
			if(GUILayout.Button(type.Name+"."+ext,normalButtonStyle,GUILayout.MinWidth(200),GUILayout.MaxWidth(1000),GUILayout.Height(20)))
			{
				List<Object> arrayOfObjects;
				arrayOfObjects = new List<Object>();
				foreach( GameObject gameObject in ( ArrayList )sets[ type ] )
				{
					arrayOfObjects.Add((Object)gameObject);
				}
				Selection.objects = arrayOfObjects.ToArray();
			}
			if(GUILayout.Button(editButtonIcon,normalButtonStyle,GUILayout.Width(30),GUILayout.Height(20)))
			{
				if(!getPathOfFile(type.Name).Equals("NaN"))
				{
					UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(getPathOfFile(type.Name), 0);
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	
		GUILayout.EndVertical();
	}

	string getPathOfFile(string tempName)
	{
		for (int count = 0; count < pathOfScripts.Count; count++) 
		{
			if(pathOfScripts[count].Contains(tempName))
			{
				return pathOfScripts[count];
			}
		}
		return "NaN";
	}





}
