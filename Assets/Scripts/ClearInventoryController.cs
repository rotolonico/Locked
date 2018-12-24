using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearInventoryController : MonoBehaviour
{

	public AudioSource NoSound;
	private PlayerController playerController;
	
	void Start ()
	{
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Transform keyInventory = GameObject.Find("KeyListContent").transform;
			if (keyInventory.childCount > 0)
			{
				playerController.ClearKeys();
				NoSound.Play();
			}
		}
	}
}
