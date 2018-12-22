using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySprite : MonoBehaviour {

	private EditorHandler editorHandler;
	public AudioSource fillSound;

	void Start () {
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			editorHandler.RestartLevelFromCp();
		}
		if (other.gameObject.CompareTag("block"))
		{
			Destroy(other.gameObject);
			fillSound.Play();
		}
	}

	
}
