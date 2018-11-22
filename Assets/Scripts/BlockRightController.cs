using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRightController : MonoBehaviour
{
	private readonly String[] unpassableBlocksTags = {"Wall", "Lock", "block", "UpOnly", "DownOnly", "LeftOnly"};
	public bool movable = true;
	private bool blocked;
	private bool playerTouch;
	private GameObject block;
	private GameObject player;
	private Transform antiBlock;
	private LeftController playerMovement;
	
	public AudioSource HitWall;
	public AudioSource MoveSound;

	void Start()
	{
		block = transform.parent.gameObject;
		antiBlock = transform.parent.GetChild(2);
		player = GameObject.FindGameObjectWithTag("Player");
		playerMovement = player.transform.GetChild(3).GetComponent<LeftController>();
	}

	void Update ()
	{
		if (Swipe.SwipeLeft && movable && playerTouch && playerMovement.Movable)
		{
			block.transform.position += Vector3.left;
			player.transform.position += Vector3.left;
			MoveSound.Play();
			blocked = false;
			Swipe.SwipeLeft = false;
		}
		else if (Swipe.SwipeLeft && !movable && playerTouch)
		{
			HitWall.Play();
		}

		antiBlock.GetComponent<BlockLeftController>().movable = !blocked;
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