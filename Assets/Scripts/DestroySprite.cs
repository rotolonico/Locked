using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySprite : MonoBehaviour {

	private EditorHandler editorHandler;
	public AudioSource fillSound;

	private GameObject player;
	private TopController topController;
	private RightController rightController;
	private LeftController leftController;
	private BotController botController;
	
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		topController = player.transform.GetChild(1).GetComponent<TopController>();
		rightController = player.transform.GetChild(0).GetComponent<RightController>();
		leftController = player.transform.GetChild(3).GetComponent<LeftController>();
		botController = player.transform.GetChild(2).GetComponent<BotController>();
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
			topController.colliders.Clear();
			rightController.colliders.Clear();
			botController.colliders.Clear();
			leftController.colliders.Clear();
		}
	}

	
}
