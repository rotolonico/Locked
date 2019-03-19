using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertedController : MonoBehaviour
{   
    private Transform player;
    private PlayerController playerController;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerController.invertedControls = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerController.invertedControls = false;
    }
}