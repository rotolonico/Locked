using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBotController : MonoBehaviour
{
	private readonly String[] unpassableBlocksTags = {"Wall", "Lock", "block", "UpOnly", "RightOnly", "LeftOnly"};
	public bool movable = true;
	private bool blocked;
	private bool playerTouch;
	private GameObject block;
	private GameObject player;
	private Transform antiBlock;
	private TopController playerMovement;
	
	public AudioSource HitWall;
	public AudioSource MoveSound;

	void Start()
	{
		block = transform.parent.gameObject;
		antiBlock = transform.parent.GetChild(0);
		player = GameObject.FindGameObjectWithTag("Player");
		playerMovement = player.transform.GetChild(1).GetComponent<TopController>();
	}

	void Update ()
	{
		if (Swipe.SwipeUp && movable && playerTouch && playerMovement.Movable)
		{
			block.transform.position += Vector3.up;
			player.transform.position += Vector3.up;
			MoveSound.Play();
			blocked = false;
			Swipe.SwipeUp = false;
		}
		else if (Swipe.SwipeUp && !movable && playerTouch)
		{
			HitWall.Play();
		}

		antiBlock.GetComponent<BlockTopController>().movable = !blocked;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerTouch = true;
		}
	}
	
	

	private void OnTriggerStay2D(Collider2D other)
	{
		blocked = false;
		foreach (var i in unpassableBlocksTags)
		{
			if (other.CompareTag(i))
			{
				blocked = true;
			}
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		blocked = false;
		if (other.CompareTag("Player"))
		{
			playerTouch = false;
		}
	}
}