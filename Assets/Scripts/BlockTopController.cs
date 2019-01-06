using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTopController : MonoBehaviour
{
    private readonly String[] unpassableBlocksTags = {"Wall", "Lock", "block", "RightOnly", "DownOnly", "LeftOnly", "LevelWall"};
    public bool movable = true;
    private bool blocked;
    private bool playerTouch;
    private GameObject block;
    private GameObject player;
    private BlockBotController antiBlock;
    private BotController playerMovement;
    private BoxCollider2D collider;
    private PlayerController playerController;

    public AudioSource HitWall;
    public AudioSource MoveSound;

    void Start()
    {
        block = transform.parent.gameObject;
        antiBlock = transform.parent.GetChild(1).GetComponent<BlockBotController>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerMovement = player.transform.GetChild(2).GetComponent<BotController>();
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Swipe.SwipeDown && playerTouch && playerMovement.Movable && antiBlock.CheckMovement())
        {
            block.transform.position += Vector3.down;
            player.transform.position += Vector3.down;
            MoveSound.Play();
            blocked = false;
            Swipe.SwipeDown = false;
            if (playerController.hasLimitedMoves)
            {
                playerController.movesLimit--;
                playerController.ReloadMoves();
            }
        }
        else if (Swipe.SwipeDown && !movable && playerTouch)
        {
            HitWall.Play();
        }
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
        collider.enabled = false;
        collider.enabled = true;
    }
    
    public bool CheckMovement()
    {
        var hitColliders = Physics2D.OverlapCircleAll(block.transform.position + Vector3.up, 0.1f);
        foreach (var collider in hitColliders)
        {
            foreach (var i in unpassableBlocksTags)
            {
                if (collider.CompareTag(i))
                {
                    return false;
                }
            }
        }

        return true;
    }
}