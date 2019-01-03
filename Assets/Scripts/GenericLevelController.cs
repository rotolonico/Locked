using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		editorHandler.LoadLevel(EditorHandler.objectSavedLevel);
	}
}
