using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Swipe : MonoBehaviour
{
    public static bool Tap;
    public static bool SwipeLeft;
    public static bool SwipeRight;
    public static bool SwipeUp;
    public static bool SwipeDown;
    public bool IsDragging;
    public static Vector2 StartTouch, SwipeDelta;
    
    public static bool readyToSwipe;
    public static bool delay = true;

    void Start()
    {
        Initiate();
    }

    public void Initiate()
    {
        readyToSwipe = false;
        StartCoroutine(StartDelay());
    }

    void Update()
    {
        Tap = SwipeDown = SwipeLeft = SwipeRight = SwipeUp = false;
//        if (Input.GetMouseButtonDown(0))
//        {
//            IsDragging = true;
//            Tap = true;
//            StartTouch = Input.mousePosition;
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            Reset();
//        }

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

        if (SwipeDelta.magnitude > 100 && IsDragging)
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

        if (readyToSwipe && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && !delay)
        {
            SwipeRight = true;
            StartCoroutine(Delay());
        }
        else if (readyToSwipe && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && !delay)
        {
            SwipeLeft = true;
            StartCoroutine(Delay());
        }
        else if (readyToSwipe && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !delay)
        {
            SwipeUp = true;
            StartCoroutine(Delay());
        }
        else if (readyToSwipe && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !delay)
        {
            SwipeDown = true;
            StartCoroutine(Delay());
        }
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
    
    private IEnumerator StartDelay()
    {
        delay = true;
        Tap = false;
        SwipeLeft = false;
        SwipeRight = false;
        SwipeUp = false;
        SwipeDown = false;
        yield return new WaitForSeconds(0.2f);
        delay = false;
        readyToSwipe = true;
    }
    
    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if( _whiteTexture == null )
            {
                _whiteTexture = new Texture2D( 1, 1 );
                _whiteTexture.SetPixel( 0, 0, Color.white );
                _whiteTexture.Apply();
            }
 
            return _whiteTexture;
        }
    }
 
    public static void DrawScreenRect( Rect rect, Color color )
    {
        GUI.color = color;
        GUI.DrawTexture( rect, WhiteTexture );
        GUI.color = Color.white;
    }
    
    public static void DrawScreenRectBorder( Rect rect, float thickness, Color color )
    {
        // Top
        DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
        // Left
        DrawScreenRect( new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
        // Right
        DrawScreenRect( new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color);
        // Bottom
        DrawScreenRect( new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
    }
    
    public static Rect GetScreenRect( Vector3 screenPosition1, Vector3 screenPosition2 )
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min( screenPosition1, screenPosition2 );
        var bottomRight = Vector3.Max( screenPosition1, screenPosition2 );
        // Create Rect
        return Rect.MinMaxRect( topLeft.x, topLeft.y, bottomRight.x, bottomRight.y );
    }
}