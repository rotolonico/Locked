using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLeftController : MonoBehaviour
{
    private readonly String[] unpassableBlocksTags = {"Wall", "Lock", "block", "UpOnly", "DownOnly", "RightOnly"};
    public bool movable = true;
    private bool blocked;
    private bool playerTouch;
    private GameObject block;
    private GameObject player;
    private Transform antiBlock;
    private RightController playerMovement;
    private BoxCollider2D collider;

    public AudioSource HitWall;
    public AudioSource MoveSound;

    void Start()
    {
        block = transform.parent.gameObject;
        antiBlock = transform.parent.GetChild(3);
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.transform.GetChild(0).GetComponent<RightController>();
    }

    void Update()
    {
        if (Swipe.SwipeRight && movable && playerTouch && playerMovement.Movable)
        {
            block.transform.position += Vector3.right;
            player.transform.position += Vector3.right;
            MoveSound.Play();
            blocked = false;
            Swipe.SwipeRight = false;
        }
        else if (Swipe.SwipeRight && !movable && playerTouch)
        {
            HitWall.Play();
        }

        antiBlock.GetComponent<BlockRightController>().movable = !blocked;
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
        collider.enabled = false;
        collider.enabled = true;
        if (other.CompareTag("Player"))
        {
            playerTouch = false;
        }

        blocked = false;
    }
}