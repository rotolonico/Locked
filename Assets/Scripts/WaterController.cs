using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    public Sprite voidFilled;
    public AudioSource fillSound;
    
    public Vector3 Scale = new Vector3(0.01f, 0.01f, 0.01f);
    
    
    private Transform player;
    private SpriteRenderer sr;
    private PlayerController playerController;

    private bool filled;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        sr = transform.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("block") || filled) return;
        Destroy(other.gameObject);
        other.GetComponent<SpriteRenderer>().sortingOrder = 0;
        filled = true;
        fillSound.Play();
        sr.sprite = voidFilled;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !filled) playerController.inWater = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerController.inWater = false;
    }
}
