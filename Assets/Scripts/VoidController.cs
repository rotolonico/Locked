using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidController : MonoBehaviour
{
	public Sprite voidFilled;
	public AudioSource fillSound;

	private SpriteRenderer sr;
	private GameObject player;
	private BoxCollider2D upCollider;
	private BoxCollider2D downCollider;
	private BoxCollider2D leftCollider;
	private BoxCollider2D rightCollider;
	private bool filled;
	private bool done;

	void Start ()
	{
		sr = transform.GetComponent<SpriteRenderer>();
		player = GameObject.FindGameObjectWithTag("Player");
		upCollider = player.transform.GetChild(0).GetComponent<BoxCollider2D>();
		downCollider = player.transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		leftCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		rightCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!filled)
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

			if (other.CompareTag("block"))
			{
				sr.sprite = voidFilled;
				Destroy(other);
				other.GetComponent<SpriteRenderer>().sortingOrder = 0;
				filled = true;
				fillSound.Play();
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (filled && !done)
		{
			if (other == upCollider)
			{
				PlayerController.Up = true;
				done = true;
			}

			if (other == downCollider)
			{
				PlayerController.Down = true;
				done = true;
			}

			if (other == leftCollider)
			{
				PlayerController.Left = true;
				done = true;
			}

			if (other == rightCollider)
			{
				PlayerController.Right = true;
				done = true;
			}
		}
	}
}