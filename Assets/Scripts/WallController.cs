using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
	
	private GameObject player;
	private BoxCollider2D upCollider;
	private BoxCollider2D downCollider;
	private BoxCollider2D leftCollider;
	private BoxCollider2D rightCollider;

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		upCollider = player.transform.GetChild(0).GetComponent<BoxCollider2D>();
		downCollider = player.transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		leftCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		rightCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other == upCollider)
		{
			PlayerController.Up = false;
		}
		if (other == downCollider)
		{
			PlayerController.Down = false;
		}
		if (other == leftCollider)
		{
			PlayerController.Left = false;
		}

		if (other == rightCollider)
		{
			PlayerController.Right = false;
		}
		
		if (other.gameObject.name == "blockTopCol")
		{
			other.gameObject.GetComponentInParent<BlockController>().Up = false;
		}
		if (other.gameObject.name == "blockBotCol")
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
}
