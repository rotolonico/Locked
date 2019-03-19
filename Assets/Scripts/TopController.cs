using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopController : MonoBehaviour
{
    private readonly string[] unpassableBlocksTags =
        {"Wall", "Hole", "Lock", "LeftOnly", "DownOnly", "RightOnly", "LevelWall"};

    private readonly string[] movableBlocksTags = {"block", "SokobanBlock"};
    public bool Movable = true;
    public bool OnIce;
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

        if (CheckInput() && Movable && !moveBlock && !playerController.sliding)
        {
            Move();
            if (playerController.hasLimitedMoves)
            {
                playerController.movesLimit--;
                playerController.ReloadMoves();
            }
        }
        else if (CheckInput() && !moveBlock && !playerController.sliding)
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
    
    private bool CheckInput()
    {
        return !playerController.invertedControls ? Swipe.SwipeUp : Swipe.SwipeDown;
    }

    public void Move()
    {
        Player.transform.position += Vector3.up;
        MoveSound.Play();
        blocked = false;
        moveBlock = false;
        Swipe.SwipeUp = false;
        if (OnIce)
        {
            StartCoroutine(MoveCoroutine());
        }
        else
        {
            playerController.sliding = false;
        }
    }
    
    public void RemoteMove()
    {
        CheckCollider();
        
        if (Movable && !moveBlock && !playerController.sliding)
        {
            Move();
            if (playerController.hasLimitedMoves)
            {
                playerController.movesLimit--;
                playerController.ReloadMoves();
            }
        }
    }

    IEnumerator MoveCoroutine()
    {
        playerController.sliding = true;
        yield return new WaitForSeconds(0.1f);
        CheckCollider();
        if (!blocked && !moveBlock)
        {
            Move();
        }
        else
        {
            playerController.sliding = false;
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

        foreach (var i in colliders)
        {
            if (i != null && i.CompareTag("ice"))
            {
                OnIce = true;
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
        foreach (var i in colliders)
        {
            if (i != null && i.CompareTag("ice"))
            {
                OnIce = false;
            }
        }

        colliders.Clear();
    }
}