using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockController : MonoBehaviour
{
    public Sprite LockKey;
    public AudioSource UnlockSound;

    private GameObject player;
    private PlayerController playerController;
    private BoxCollider2D centerCollider;
    private SpriteRenderer sr;
    private SpriteRenderer pSr;
    private bool hasKey;

    void Start()
    {
        sr = gameObject.transform.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        centerCollider = player.GetComponent<BoxCollider2D>();
        pSr = GameObject.FindGameObjectWithTag("Player").transform.GetChild(5).GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!hasKey && (other.transform.name == "rightCol" || other.transform.name == "leftCol" ||
                        other.transform.name == "topCol" || other.transform.name == "botCol"))
        {
            if (playerController.HasKey > 0)
            {
                sr.sprite = LockKey;
                UnlockSound.Play();
                hasKey = true;
                playerController.HasKey -= 1;
                transform.tag = "LockOpen";
                playerController.RemoveKeys();
            }
        }

        if (other == centerCollider)
        {
            pSr.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == centerCollider)
        {
            pSr.enabled = false;
        }
    }
}