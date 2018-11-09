using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLockController : MonoBehaviour
{
	public Sprite LockKey;
	public Sprite PlayerNormal;
	public Sprite PlayerLock;

	public AudioSource UnlockSound;
	
	private GameObject player;
	private BoxCollider2D upCollider;
	private BoxCollider2D downCollider;
	private BoxCollider2D leftCollider;
	private BoxCollider2D rightCollider;
	private BoxCollider2D centerCollider;
	private SpriteRenderer sr;
	private SpriteRenderer pSr;
	private bool hasKey;

	void Start ()
	{
		sr = gameObject.transform.GetComponent<SpriteRenderer>();
		player = GameObject.FindGameObjectWithTag("Player");
		pSr = player.transform.GetComponent<SpriteRenderer>();
		centerCollider = player.GetComponent<BoxCollider2D>();
		upCollider = player.transform.GetChild(0).GetComponent<BoxCollider2D>();
		downCollider = player.transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		leftCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
		rightCollider = player.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (!hasKey)
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

			if (PlayerController.HasRedKey)
			{
				sr.sprite = LockKey;
				UnlockSound.Play();
				if (other == upCollider)
				{
					PlayerController.Up = true;
				}

				if (other == downCollider)
				{
					PlayerController.Down = true;
				}

				if (other == leftCollider)
				{
					PlayerController.Left = true;
				}

				if (other == rightCollider)
				{
					PlayerController.Right = true;
				}
				PlayerController.HasRedKey = false;
				hasKey = true;
			}
		}

		if (other != centerCollider)
		{
			pSr.sprite = PlayerNormal;
		}
		if (other == centerCollider)
		{
			pSr.sprite = PlayerLock;
		}
	}
}
