using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif



[Serializable]
public class UnityPathAudio : UnityPathBase<AudioClip>
{
	public UnityPathAudio(stringType type) : base(type)
	{
		myType = type;
	}

#if UNITY_EDITOR
	public UnityPathAudio(AudioClip obj) : base(obj)
	{
		ObjectEditorOnly = obj;
		FillPath();
	}
#endif
}

[Serializable]
public class UnityPathTextAsset : UnityPathBase<TextAsset>
{
	public UnityPathTextAsset(stringType type) : base(type)
	{
		myType = type;
	}

#if UNITY_EDITOR
	public UnityPathTextAsset(TextAsset obj) : base(obj)
	{
		ObjectEditorOnly = obj;
		FillPath();
	}
#endif
}

[Serializable]
public class UnityPathPrefab : UnityPathBase<GameObject>
{
	public UnityPathPrefab(stringType type) : base(type)
	{
		myType = type;
	}
#if UNITY_EDITOR
	public UnityPathPrefab(GameObject obj) : base(obj)
	{
		ObjectEditorOnly = obj;
		FillPath();
	}
#endif
}

[Serializable]
public class UnityPathScene : UnityPathBase<
#if UNITY_EDITOR
	SceneAsset
#else
		Object
#endif
>
{
	public UnityPathScene(stringType type) : base(type)
	{
		myType = type;
	}
#if UNITY_EDITOR
	public UnityPathScene(SceneAsset obj) : base(obj)
	{
		ObjectEditorOnly = obj;
		FillPath();
	}
#endif
}

[Serializable]
public class UnityPath : UnityPathBase<
#if UNITY_EDITOR
	DefaultAsset
	#else
		Object
#endif
>
{
	public UnityPath(stringType type) : base(type)
	{
		myType = type;
	}
#if UNITY_EDITOR
	public UnityPath(DefaultAsset obj) : base(obj)
	{
		ObjectEditorOnly = obj;
		FillPath();
	}
#endif
}

[Serializable]
public class UnityPathBase<T> 
#if UNITY_EDITOR
	: ISerializationCallbackReceiver
#endif
	where T : Object
{
	public enum stringType
	{
		ResourcePath = 0,
		FileName = 1,
	}
	
	public string value;
	public stringType myType;
	
	public override string ToString()
	{
		return value;
	}

	public bool Exist;
	
	
	public UnityPathBase(stringType type)
	{
		myType = type;
	}
#if UNITY_EDITOR
	public T ObjectEditorOnly;
	public string fullPathEditorOnly;

	public void FillPath()
	{
		if (Exist = ObjectEditorOnly) 
		{
			switch (myType)
			{
				case stringType.ResourcePath:
					fullPathEditorOnly = AssetDatabase.GetAssetPath(ObjectEditorOnly);
					var splitted = fullPathEditorOnly.Split(new[] {"/Resources/"},StringSplitOptions.None);
					value = splitted[splitted.Length - 1].Split('.')[0];
					break;
				case stringType.FileName:
					value = ObjectEditorOnly.name;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	
	public UnityPathBase(T obj)
	{
		ObjectEditorOnly = obj;
		FillPath();
	}
	
	public void OnBeforeSerialize()
	{
		FillPath();
	}

	public void OnAfterDeserialize()
	{

	}
	
#else
	public T Obj;
	public string fullPath;
#endif
}


#region UnityPathDrawer

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UnityPathScene), true)]
public class UnityPathSceneDrawer : UnityPathDrawer
{
	
}

[CustomPropertyDrawer(typeof(UnityPathPrefab), true)]
public class UnityPathPrefabDrawer : UnityPathDrawer
{
	
}

[CustomPropertyDrawer(typeof(UnityPathAudio), true)]
public class UnityPathAudioDrawer : UnityPathDrawer
{
	
}
[CustomPropertyDrawer(typeof(UnityPathTextAsset), true)]
public class UnityPathTextAssetDrawer : UnityPathDrawer
{
	
}

[CustomPropertyDrawer(typeof(UnityPath),true)]
public class UnityPathDrawer : PropertyDrawer
{
	private bool inited;
	private string lastString;

	void Init(SerializedProperty property)
	{
		var newstring = FillPath(property.FindPropertyRelative("ObjectEditorOnly").objectReferenceValue);
		inited = newstring.Equals(lastString);

		if (inited) return;

		lastString = property.FindPropertyRelative("value").stringValue = newstring;
	}

	public string FillPath(Object Folder)
	{
		if (Folder) return AssetDatabase.GetAssetPath(Folder);
		return "";
	}

	private readonly GUIContent emptyGUIContent = new GUIContent();


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var size = position.size;
		var pos = position.position;

		size.x = size.x / 1.5f;
		position.size = size;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("ObjectEditorOnly"), label);

		pos.x += size.x;
		position.position = pos;
		size.x = size.x * .5f;
		position.size = size;

		EditorGUI.PropertyField(position, property.FindPropertyRelative("myType"), emptyGUIContent);

		Init(property);
		//base.OnGUI(position, property, label);
	}
}

#endif
#endregion