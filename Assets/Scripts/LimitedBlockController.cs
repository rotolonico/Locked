﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedBlockController : MonoBehaviour
{
    public int limit = 1;
    public GameObject Hole;

    public Sprite[] limitedSprites = new Sprite[10];
    private SpriteRenderer sr;

    private void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !EditorHandler.GameOver)
        {
            limit -= 1;
            if (limit == 0)
            {
                Instantiate(Hole, transform.position, transform.rotation);
                Destroy(gameObject);
            }

            sr.sprite = limitedSprites[limit];
        }
    }
}