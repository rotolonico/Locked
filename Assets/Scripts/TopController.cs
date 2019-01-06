﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopController : MonoBehaviour
{
    private readonly string[] unpassableBlocksTags = {"Wall", "Hole", "Lock", "LeftOnly", "DownOnly", "RightOnly", "LevelWall"};
    private readonly string[] movableBlocksTags = {"block"};
    public bool Movable = true;
    private bool blocked;
    private bool moveBlock;
    private GameObject Player;
    private bool outOfMoves;
    List<Collider2D> collider = new List<Collider2D>();
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
        if (collider != null)
        {
            CheckCollider();
        }

        if (Swipe.SwipeUp && Movable && !moveBlock)
        {
            Player.transform.position += Vector3.up;
            MoveSound.Play();
            blocked = false;
            moveBlock = false;
            Swipe.SwipeUp = false;
            if (playerController.hasLimitedMoves)
            {
                playerController.movesLimit--;
                playerController.ReloadMoves();
            }
        }
        else if (Swipe.SwipeUp && !moveBlock)
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
            foreach (var j in collider)
            {
                if (j != null && j.CompareTag(i))
                {
                    blocked = true;
                }
            }
        }

        foreach (var i in movableBlocksTags)
        {
            foreach (var j in collider)
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
        if (collider.Count < 3)
            collider.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        collider.Clear();
    }
}