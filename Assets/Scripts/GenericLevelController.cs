using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericLevelController : MonoBehaviour
{
	private EditorHandler editorHandler;
	private PlayerController playerController;
	private Animator keyHolderAnimator;

	void Start ()
	{
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
		keyHolderAnimator = GameObject.FindGameObjectWithTag("keyInventory").GetComponent<Animator>();
		Initialize();
	}

	public void Initialize()
	{
		editorHandler.ClearEditor();
		editorHandler.LoadLevel(EditorHandler.savedLevel);
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
}
