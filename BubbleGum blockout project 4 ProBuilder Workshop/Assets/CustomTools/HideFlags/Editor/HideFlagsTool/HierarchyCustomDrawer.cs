using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyCustomDrawer
{
	static HierarchyCustomDrawer()
	{
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
	}

	[MenuItem( "GameObject/HideFlags/Default", false, 0 )]
	private static void SetHideFlagsDefault()
	{
		SetHideFlags( HideFlags.None, true );
	}

	[MenuItem( "GameObject/HideFlags/Not Built", false, 0 )]
	private static void ToggleHideFlagsNotBuilt()
	{
		SetHideFlags( HideFlags.DontSaveInBuild );
	}

	[MenuItem( "GameObject/HideFlags/Not Visible", false, 0 )]
	private static void ToggleHideFlagsNotVisible()
	{
		if( !EditorUtility.DisplayDialog( "Confirmation", "You are about to set the selected objects invisible! This means you will no longer be able to see them in the hierarchy. Are you sure you really want to do this?", "No!", "Yes" ) )
		{
			SetHideFlags( HideFlags.HideInHierarchy );
		}
	}

	[MenuItem( "GameObject/HideFlags/Hide Inspector", false, 0 )]
	private static void ToggleHideFlagsHideInspector()
	{
		SetHideFlags( HideFlags.HideInInspector );
	}

	[MenuItem( "GameObject/HideFlags/Not Saved", false, 0 )]
	private static void ToggleHideFlagsDontSaveInEditor()
	{
		SetHideFlags( HideFlags.DontSaveInEditor );
	}

	[MenuItem( "GameObject/HideFlags/Not Editable", false, 0 )]
	private static void ToggleHideFlagsNotEditable()
	{
		SetHideFlags( HideFlags.NotEditable );
	}

	private static void SetHideFlags( HideFlags _hideFlags, bool _override = false )
	{
		for( int i = 0; i < Selection.gameObjects.Length; i++ )
		{
			if( Selection.gameObjects[i].scene.path != null )
			{
				Undo.RegisterCompleteObjectUndo( Selection.gameObjects[i], "Changed HideFlag" );
				if( _override )
				{
					Selection.gameObjects[i].hideFlags = _hideFlags;
				}
				else
				{
					Selection.gameObjects[i].hideFlags = (HideFlags)Bitwise.Toggle( (int)Selection.gameObjects[i].hideFlags, (int)_hideFlags );
				}
				EditorUtility.SetDirty( Selection.gameObjects[i] );
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty( Selection.gameObjects[i].scene );
			}
		}
	}

	private static void HierarchyWindowItemOnGUI( int _instanceId, Rect _rect )
	{
		Object _object = EditorUtility.InstanceIDToObject( _instanceId );
		if( _object is GameObject )
		{
			GameObject _gameObject = (GameObject)_object;

			Rect _flagTexRect = new Rect( _rect );
			_flagTexRect.x = ( _flagTexRect.x + _flagTexRect.width ) - _flagTexRect.height;
			_flagTexRect.y += 1;
			_flagTexRect.width = _flagTexRect.height;

			GUIContent _content;

			if( HasNotBuiltFlag( _gameObject ) )
			{
				_content = new GUIContent( (Texture)EditorGUIUtility.Load( "sv_icon_dot6_pix16_gizmo" ), "Won't be built" );
				GUI.Label( _flagTexRect, _content );
			}

			_flagTexRect.x -= _flagTexRect.height;
			if( HasDontSaveInEditorFlag( _gameObject ) )
			{
				_content = new GUIContent( (Texture)EditorGUIUtility.Load( "sv_icon_dot5_pix16_gizmo" ), "Won't be saved" );
				GUI.Label( _flagTexRect, _content );
			}

			_flagTexRect.x -= _flagTexRect.height;
			if( HasNotEditableFlag( _gameObject ) )
			{
				_content = new GUIContent( (Texture)EditorGUIUtility.Load( "sv_icon_dot4_pix16_gizmo" ), "Not editable" );
				GUI.Label( _flagTexRect, _content );
			}
			
			_flagTexRect.x -= _flagTexRect.height;
			if( HasHideInspectorFlag( _gameObject ) )
			{
				_content = new GUIContent( (Texture)EditorGUIUtility.Load( "sv_icon_dot2_pix16_gizmo" ), "No inspector" );
				GUI.Label( _flagTexRect, _content );
			}
		}
	}

	private static bool HasNotBuiltFlag( GameObject _gameObject )
	{
		return Bitwise.Contains( (int)_gameObject.hideFlags, (int)HideFlags.DontSaveInBuild ) || ( _gameObject.transform.parent != null && HasNotBuiltFlag( _gameObject.transform.parent.gameObject ) );
	}
	private static bool HasHideInspectorFlag( GameObject _gameObject )
	{
		return Bitwise.Contains( (int)_gameObject.hideFlags, (int)HideFlags.HideInInspector );
	}
	private static bool HasDontSaveInEditorFlag( GameObject _gameObject )
	{
		return Bitwise.Contains( (int)_gameObject.hideFlags, (int)HideFlags.DontSaveInEditor ) || ( _gameObject.transform.parent != null && HasDontSaveInEditorFlag( _gameObject.transform.parent.gameObject ) );
	}
	private static bool HasNotEditableFlag( GameObject _gameObject )
	{
		return Bitwise.Contains( (int)_gameObject.hideFlags, (int)HideFlags.NotEditable );
	}
}