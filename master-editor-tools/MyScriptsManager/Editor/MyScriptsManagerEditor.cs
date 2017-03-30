using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace myEditorScripts
{
	public class MyScriptsManagerEditor : EditorWindow {
	private Hashtable sets = new Hashtable();

	[MenuItem("[Master_Tools]/Scripts Manager")]
	static void Init() {

		GetWindow(typeof(MyScriptsManagerEditor));
		GetWindow(typeof(MyScriptsManagerEditor)).minSize = new Vector2(300,200);
		GetWindow(typeof(MyScriptsManagerEditor)).titleContent = new GUIContent("All Scripts v0.3");
		}



	static Texture2D csScriptIcon,jsScriptIcon;
	GUIStyle styleHelpboxInner;
	GUIStyle titleLabel,normalButtonStyle,helpButtonStyle;
	Texture editButtonIcon,saveButtonIcon;
	void InitStyles()
	{
		editButtonIcon = Resources.Load("toolPencil") as Texture;
		styleHelpboxInner = new GUIStyle("HelpBox");
		styleHelpboxInner.padding = new RectOffset(6, 6, 6, 6);

		titleLabel = new GUIStyle();
		titleLabel.fontSize = 10;
			titleLabel.fontStyle = FontStyle.Bold;
		titleLabel.normal.textColor = Color.white;
		titleLabel.alignment = TextAnchor.UpperCenter;
		titleLabel.fixedHeight = 15;


			helpButtonStyle = new GUIStyle(GUI.skin.button);
			helpButtonStyle.fontSize = 10;
			helpButtonStyle.fontStyle = FontStyle.Bold;
			helpButtonStyle.normal.textColor = Color.white;
			helpButtonStyle.alignment = TextAnchor.MiddleCenter;


		normalButtonStyle = new GUIStyle(GUI.skin.button);
		normalButtonStyle.alignment = TextAnchor.MiddleLeft;

	}

	public void UpdateList()
	{
		Object[] objects;

		sets.Clear();
			objects = FindObjectsOfType( typeof( Component ) );

		foreach( Component component in objects )
		{
			if(component.GetType().BaseType.ToString().Equals("UnityEngine.MonoBehaviour") &&  IsInsideProject(component.GetType().Name))
			{
				if( !sets.ContainsKey( component.GetType() ))
				{
					sets[ component.GetType() ] = new ArrayList();
				}
				( ( ArrayList )sets[ component.GetType() ] ).Add( component.gameObject );
			}
		}
		if(sets.Count>0)
		{
			hasAnyScript = true;
		}
		else
		{
			hasAnyScript = false;
		}
	}

	bool IsInsideProject(string fileName)
	{
		List<string> tempPaths = new List<string>();
		string[] guids = AssetDatabase.FindAssets ("t:Script");
		tempPaths = new List<string>();
		foreach (var itemPath in guids) 
		{
			tempPaths.Add(AssetDatabase.GUIDToAssetPath(itemPath));
		}

		for (int count = 0; count < tempPaths.Count; count++) {
			if(tempPaths[count].Contains(fileName))
				return true;
		}
		return false;
	}
	bool hasAnyScript = false;
	List<string> pathOfScripts;
	void OnGUI() 
	{
		InitStyles();
		styleHelpboxInner = new GUIStyle("HelpBox");
		styleHelpboxInner.padding = new RectOffset(6, 6, 6, 6);
		GUILayout.BeginVertical(styleHelpboxInner);
		GUILayout.BeginHorizontal(styleHelpboxInner);
		GUILayout.Label("All scripts used in current scene",titleLabel);
			if(GUILayout.Button(new GUIContent("?", "Shoot me an email about the issue or suggestions"),helpButtonStyle,GUILayout.MaxWidth(20),GUILayout.MaxHeight(20)))
			{
				Application.OpenURL("https://github.com/prashant-singh");
			}
		GUILayout.EndHorizontal();
		GUILayout.Space(5);
		if(!hasAnyScript)
		{
			GUILayout.BeginHorizontal(styleHelpboxInner);
			GUILayout.Label("No scripts found in this scene",titleLabel);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			return;
		}
		
		
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

			if(ext.Equals("cs"))
			{
					GUILayout.Box(new GUIContent(csScriptIcon, "CSharp Script"),GUILayout.MinWidth(20),GUILayout.MinHeight(20));
			}
			else
			{
					GUILayout.Box(new GUIContent(jsScriptIcon, "JavaScript"),GUILayout.MinWidth(20),GUILayout.MinHeight(20));
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
				if(GUILayout.Button(new GUIContent(editButtonIcon, "Edit this script"),normalButtonStyle,GUILayout.Width(30),GUILayout.Height(20)))
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

	void OnFocus()
	{
		csScriptIcon = EditorGUIUtility.Load("icons/generated/cs script icon.asset") as Texture2D;
		jsScriptIcon = EditorGUIUtility.Load("icons/generated/js script icon.asset") as Texture2D;
		UpdateList();
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
}

