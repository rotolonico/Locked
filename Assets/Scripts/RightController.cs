using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightController : MonoBehaviour
{
    private readonly string[] unpassableBlocksTags = {"Wall", "Hole", "Lock", "UpOnly", "DownOnly", "LeftOnly", "LevelWall"};
    private readonly string[] movableBlocksTags = {"block"};
    public bool Movable = true;
    private bool blocked;
    private bool moveBlock;
    private GameObject Player;
    private bool outOfMoves;
    public List<Collider2D> colliders = new List<Collider2D>();
    PlayerController playerController;

    public AudioSource HitWall;
    public AudioSource MoveSound;

    private void Start()
    {
        Player = transform.parent.gameObject;
        playerController = Player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        blocked = false;
        moveBlock = false;
        if (colliders != null)
        {
            CheckCollider();
        }

        if (Swipe.SwipeRight && Movable && !moveBlock)
        {
            Player.transform.position += Vector3.right;
            MoveSound.Play();
            blocked = false;
            moveBlock = false;
            Swipe.SwipeRight = false;
            if (playerController.hasLimitedMoves)
            {
                playerController.movesLimit--;
                playerController.ReloadMoves();
            }
        }
        else if (Swipe.SwipeRight && !moveBlock)
        {
            HitWall.Play();
        }

        if (!outOfMoves)
        {
            Movable = !blocked;
            if (playerController.hasLimitedMoves && playerController.movesLimit < 1)
            {
                Movable = false;
                GameObject.Find("PlayerText").GetComponent<Text>().text = "0";
                outOfMoves = true;
            }
        }
    }

    private void CheckCollider()
    {
        foreach (var i in unpassableBlocksTags)
        {
            foreach (var j in colliders)
            {
                if (j != null && j.CompareTag(i))
                {
                    blocked = true;
                }
            }
        }

        foreach (var i in movableBlocksTags)
        {
            foreach (var j in colliders)
            {
                if (j != null && j.CompareTag(i))
                {
                    moveBlock = true;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (colliders.Count < 3)
            colliders.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        colliders.Clear();
    }
}