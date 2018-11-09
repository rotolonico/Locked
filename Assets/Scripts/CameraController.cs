using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public AudioSource CameraScroll;
    
    private float distanceX;
    private float distanceY;

    private Transform playerTransform;
    private Transform cameraTransform;
    
    private Vector3 playerPosition;
    private Vector3 cameraPosition;
    
    private Vector2 playerPositionX;
    private Vector2 cameraPositionX;
    
    private Vector2 playerPositionY;
    private Vector2 cameraPositionY;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        cameraTransform = gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        playerPosition = playerTransform.position;
        cameraPosition = cameraTransform.position;
        
        cameraPosition.z = -10;
        
        playerPositionX = new Vector2(playerPosition.x,0);
        cameraPositionX = new Vector2(cameraPosition.x,0);
        
        playerPositionY = new Vector2(0,playerPosition.y);
        cameraPositionY = new Vector2(0,cameraPosition.y);

        distanceX = Vector2.Distance(cameraPositionX, playerPositionX);
        distanceY = Vector2.Distance(cameraPositionY, playerPositionY);
        
        if (distanceX > 3)
        {
            if (playerPosition.x > cameraPosition.x)
            {
                transform.position += new Vector3(7,0,0);
            }
            else
            {
                transform.position += new Vector3(-7,0,0);
            }
            CameraScroll.Play();
        }
        if (distanceY > 3)
        {
            if (playerPosition.y > cameraPosition.y)
            {
                transform.position += new Vector3(0,7,0);
            }
            else
            {
                transform.position += new Vector3(0,-7,0);
            }
            CameraScroll.Play();
        }
    }
}