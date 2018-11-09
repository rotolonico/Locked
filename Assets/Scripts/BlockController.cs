using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{

	private GameObject player;
	private BoxCollider2D upCollider;
	private BoxCollider2D downCollider;
	private BoxCollider2D leftCollider;
	private BoxCollider2D rightCollider;
	private BoxCollider2D blockCollider;
	
	public bool Up = true;
	public bool Down = true;
	public bool Left = true;
	public bool Right = true;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		upCollider = player.transform.GetChild(0).GetComponent<BoxCollider2D>();
		downCollider = player.transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		leftCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		rightCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.name == "blockTopCol")
		{
			other.gameObject.GetComponentInParent<BlockController>().Up = false;
		}
		if (other.gameObject.name == "blockBopCol")
		{
			other.gameObject.GetComponentInParent<BlockController>().Down = false;
		}
		if (other.gameObject.name == "blockLeftCol")
		{
			other.gameObject.GetComponentInParent<BlockController>().Left = false;
		}
		if (other.gameObject.name == "blockRightCol")
		{
			other.gameObject.GetComponentInParent<BlockController>().Right = false;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other == upCollider)
		{
			PlayerController.Up = false;
			if (Swipe.SwipeUp)
			{
				if (Up)
				{
					transform.position += Vector3.up;
					player.transform.position += Vector3.up;
					PlayerController.Up = PlayerController.Down = PlayerController.Left = PlayerController.Right = true;
					Up = Down = Left = Right = true;
				}
			}
		}

		if (other == downCollider)
		{
			PlayerController.Down = false;
			if (Swipe.SwipeDown)
			{
				if (Down)
				{
					transform.position += Vector3.down;
					player.transform.position += Vector3.down;
					PlayerController.Up = PlayerController.Down = PlayerController.Left = PlayerController.Right = true;
					Up = Down = Left = Right = true;
				}
			}
		}

		if (other == leftCollider)
		{
			PlayerController.Left = false;
			if (Swipe.SwipeLeft)
			{
				if (Left)
				{
					transform.position += Vector3.left;
					player.transform.position += Vector3.left;
					PlayerController.Up = PlayerController.Down = PlayerController.Left = PlayerController.Right = true;
					Up = Down = Left = Right = true;
				}
			}
		}

		if (other == rightCollider)
		{
			PlayerController.Right = false;
			if (Swipe.SwipeRight)
			{
				if (Right)
				{
					transform.position += Vector3.right;
					player.transform.position += Vector3.right;
					PlayerController.Up = PlayerController.Down = PlayerController.Left = PlayerController.Right = true;
					Up = Down = Left = Right = true;
				}
			}
		}
	}
}