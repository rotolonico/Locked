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

    private readonly Vector3 right = new Vector3(6, 0, 0);
    private readonly Vector3 up = new Vector3(0, 6, 0);
    private readonly Vector3 left = new Vector3(-6, 0, 0);
    private readonly Vector3 down = new Vector3(0, -6, 0);

    private bool moveUp;
    private bool moveRight;
    private bool moveDown;
    private bool moveLeft;

    private bool cameraMode;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        cameraTransform = gameObject.transform;
        Camera.main.orthographicSize = 3;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            playerPosition = playerTransform.position;
            cameraPosition = cameraTransform.position;

            cameraPosition.z = -10;

            playerPositionX = new Vector2(playerPosition.x, 0);
            cameraPositionX = new Vector2(cameraPosition.x, 0);

            playerPositionY = new Vector2(0, playerPosition.y);
            cameraPositionY = new Vector2(0, cameraPosition.y);

            distanceX = Vector2.Distance(cameraPositionX, playerPositionX);
            distanceY = Vector2.Distance(cameraPositionY, playerPositionY);

//            if (Input.GetKeyDown(KeyCode.C))
//            {
//                cameraMode = true;
//            }

            if (!cameraMode)
            {
                if (distanceX > 2.9)
                {
                    if (playerPosition.x > cameraPosition.x)
                    {
                        transform.position = transform.position + right;
                    }
                    else
                    {
                        transform.position = transform.position + left;
                    }

                    CameraScroll.Play();
                }

                if (distanceY > 2.9)
                {
                    if (playerPosition.y > cameraPosition.y)
                    {
                        transform.position = transform.position + up;
                    }
                    else
                    {
                        transform.position = transform.position + down;
                    }

                    CameraScroll.Play();
                }
            }

            if (cameraMode)
            {
                if (distanceX > 2.9)
                {
                    if (playerPosition.x > cameraPosition.x)
                    {
                        moveRight = true;
                    }
                    else
                    {
                        moveLeft = true;
                    }
                }

                if (distanceY > 2.9)
                {
                    if (playerPosition.y > cameraPosition.y)
                    {
                        moveUp = true;
                    }
                    else
                    {
                        moveDown = true;
                    }
                }

                if (distanceX < 0.9)
                {
                    moveRight = false;
                    moveLeft = false;
                }

                if (distanceY < 0.9)
                {
                    moveUp = false;
                    moveDown = false;
                }

                if (moveRight)
                {
                    transform.position = Vector2.MoveTowards(transform.position, transform.position + right,
                        5 * Time.deltaTime);
                }

                if (moveLeft)
                {
                    transform.position =
                        Vector2.MoveTowards(transform.position, transform.position + left, 5 * Time.deltaTime);
                }

                if (moveUp)
                {
                    transform.position =
                        Vector2.MoveTowards(transform.position, transform.position + up, 5 * Time.deltaTime);
                }

                if (moveDown)
                {
                    transform.position =
                        Vector2.MoveTowards(transform.position, transform.position + down, 5 * Time.deltaTime);
                }
            }
        }
    }
}