using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Swipe : MonoBehaviour
{
    public bool Tap;
    public static bool SwipeLeft;
    public static bool SwipeRight;
    public static bool SwipeUp;
    public static bool SwipeDown;
    public bool IsDragging;
    public Vector2 StartTouch, SwipeDelta;

    private bool delay;

    void Start()
    {
        StartCoroutine(Delay());
    }

    void Update()
    {
        Tap = SwipeDown = SwipeLeft = SwipeRight = SwipeUp = false;
        if (Input.GetMouseButtonDown(0))
        {
            IsDragging = true;
            Tap = true;
            StartTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Reset();
        }

        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                IsDragging = true;
                Tap = true;
                StartTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Reset();
            }
        }

        if (Input.touchCount > 0)
        {
            SwipeDelta = Input.touches[0].position - StartTouch;
        }

        if (Input.GetMouseButton(0))
        {
            SwipeDelta = (Vector2) Input.mousePosition - StartTouch;
        }

        if (SwipeDelta.magnitude > 50 && IsDragging)
        {
            float x = SwipeDelta.x;
            float y = SwipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x < 0)
                {
                    SwipeLeft = true;
                }
                else
                {
                    SwipeRight = true;
                }
            }
            else
            {
                if (y < 0)
                {
                    SwipeDown = true;
                }
                else
                {
                    SwipeUp = true;
                }
            }

            Reset();
        }

        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && !delay)
        {
            SwipeRight = true;
            StartCoroutine(Delay());
        }
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && !delay)
        {
            SwipeLeft = true;
            StartCoroutine(Delay());
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !delay)
        {
            SwipeUp = true;
            StartCoroutine(Delay());
        }
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !delay)
        {
            SwipeDown = true;
            StartCoroutine(Delay());
        }
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Reset()
    {
        StartTouch = SwipeDelta = Vector2.zero;
        IsDragging = false;
    }

    private IEnumerator Delay()
    {
        delay = true;
        yield return new WaitForSeconds(0.1f);
        delay = false;
    }
}