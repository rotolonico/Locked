using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBotController : MonoBehaviour
{
    private String[] unpassableBlocksTags = {"Wall", "Lock", "block", "UpOnly", "RightOnly", "LeftOnly", "LevelWall", "SokobanBlock"};
    private readonly String[] unpassableSokobanBlocksTags = {"Wall", "Lock", "block", "UpOnly", "RightOnly", "LeftOnly", "LevelWall", "SokobanBlock", "Hole"};
    public bool movable = true;
    public bool iceBlock;
    private bool blocked;
    private bool playerTouch;
    private GameObject block;
    private GameObject player;
    private IceBlockController iceBlockController;
    private BlockTopController antiBlock;
    private TopController playerMovement;
    private BoxCollider2D collider;
    private PlayerController playerController;

    public AudioSource HitWall;
    public AudioSource MoveSound;

    void Start()
    {
        if (gameObject.transform.parent.CompareTag("SokobanBlock"))
        {
            unpassableBlocksTags = unpassableSokobanBlocksTags;
        }
        block = transform.parent.gameObject;
        iceBlockController = block.GetComponent<IceBlockController>();
        antiBlock = transform.parent.GetChild(0).GetComponent<BlockTopController>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerMovement = player.transform.GetChild(1).GetComponent<TopController>();
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (CheckInput() && playerTouch && playerMovement.Movable && antiBlock.CheckMovement() && !playerController.sliding)
        {
            if (iceBlock)
            {
                var blocks = GameObject.FindGameObjectsWithTag("block");
                var sokobanBlocks = GameObject.FindGameObjectsWithTag("SokobanBlock");

                foreach (var movableBlock in blocks)
                {
                    movableBlock.GetComponent<IceBlockController>().muted = true;
                }

                foreach (var movableSokobanBlock in sokobanBlocks)
                {
                    movableSokobanBlock.GetComponent<IceBlockController>().muted = true;
                }
            }
            iceBlockController.muted = false;
            Move();
            player.transform.position += Vector3.up;
            if (playerController.hasLimitedMoves)
            {
                playerController.movesLimit--;
                playerController.ReloadMoves();
            }
        }
        else if (CheckInput() && playerMovement.Movable && playerTouch && !playerController.sliding)
        {
            HitWall.Play();
        }
    }
    
    private bool CheckInput()
    {
        return !playerController.invertedControls ? Swipe.SwipeUp : Swipe.SwipeDown;
    }

    private void Move()
    {
        block.transform.position += Vector3.up;
        if (!iceBlockController.muted)
        {
            MoveSound.Play();
        }
        blocked = false;
        Swipe.SwipeUp = false;
        if (iceBlock)
        {
            StartCoroutine(MoveCoroutine());
        }
    }

    private IEnumerator MoveCoroutine()
    {
        iceBlockController.sliding = true;
        yield return new WaitForSeconds(0.1f);
        if (antiBlock.CheckMovement())
        {
            Move();
        }
        else
        {
            iceBlockController.sliding = false;
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
        var hitColliders = Physics2D.OverlapCircleAll(block.transform.position + Vector3.down, 0.1f);
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