using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericLevelController : MonoBehaviour
{
	private EditorHandler editorHandler;

	void Start ()
	{
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
		Initialize();
	}

	public void Initialize()
	{
		editorHandler.ClearEditor();
		editorHandler.ChangeSize(true, EditorHandler.objectSavedLevel.levelRows, EditorHandler.objectSavedLevel.levelColumns);
		editorHandler.LoadLevel(EditorHandler.objectSavedLevel);
		EditorHandler.GameOver = false;
	}
}
