using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
	public AudioSource TeleportSound;
	
	private PlayerController playerController;
	private GameObject player;
	private GameObject[] portalsArray;
	private List<GameObject> portals = new List<GameObject>();
	private readonly System.Random rnd = new System.Random();
	private bool teleported;
	private bool isBlocked;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<PlayerController>();
		
	}

	private void OnTriggerStay2D(Collider2D other)
	{
			isBlocked = false;
			if (other.CompareTag("block"))
			{
				isBlocked = true;
			}

			if (other.CompareTag("Player") && !playerController.Teleporting)
			{
				portals.Clear();
				portalsArray = GameObject.FindGameObjectsWithTag(transform.tag);

				foreach (var i in portalsArray)
				{
					if (i != gameObject && !i.GetComponent<TeleporterController>().isBlocked)
					{
						portals.Add(i);
					}
				}

				if (portals.Count != 0)
				{
					int r = rnd.Next(portals.Count);
					Debug.Log(r);
					player.transform.position = portals[r].transform.position;
					TeleportSound.Play();
					playerController.Teleporting = true;
				}

			}
			else if (other.CompareTag("Player") && playerController.Teleporting)
			{
				teleported = true;
			}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (teleported)
		{
			playerController.Teleporting = false;
			teleported = false;
			isBlocked = false;
		}
	}
}
