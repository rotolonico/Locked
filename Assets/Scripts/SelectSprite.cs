using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectSprite : MonoBehaviour
{
    public int spriteGameObjectId;

    public bool selected;
    private EditorHandler editorHandler;

    public int limitedStep;
    public int movesLimit;
    public string randomType;

    private void Start()
    {
        editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
        if (CheckPosition())
        {
            editorHandler.placeSound.Play();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            var mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            Plane mouseHitPlane = new Plane(Vector3.forward, transform.position);
            if (mouseHitPlane.Raycast(mousePositionRay, out dist))
            {
                Vector3 mousePosition = mousePositionRay.GetPoint(dist);
                mousePosition.x = (float) Math.Round(mousePosition.x);
                mousePosition.y = (float) Math.Round(mousePosition.y);
                mousePosition.z = (float) Math.Round(mousePosition.z);
                if (Physics2D.OverlapCircle(mousePosition, 0.3f) == gameObject.GetComponent<Collider2D>())
                {
                    editorHandler.selectedObject = editorHandler.editorBlocks[spriteGameObjectId];
                }
            }
        }
    }

    public bool CheckPosition()
    {
        if (
            EditorHandler.levelColumns % 2 != 0 &&
            Mathf.Abs(transform.position.x - 1) > EditorHandler.levelColumns / 2 ||
            EditorHandler.levelRows % 2 != 0 && Mathf.Abs(transform.position.y) > EditorHandler.levelRows / 2 ||
            EditorHandler.levelColumns % 2 == 0 && transform.position.x < -EditorHandler.levelColumns / 2 + 1 ||
            EditorHandler.levelRows % 2 == 0 && transform.position.y < -EditorHandler.levelRows / 2 + 1 ||
            EditorHandler.levelColumns % 2 == 0 && Mathf.Abs(transform.position.x) > EditorHandler.levelColumns / 2 ||
            EditorHandler.levelRows % 2 == 0 && Mathf.Abs(transform.position.y) > EditorHandler.levelRows / 2)
        {
            Destroy(gameObject);
            return false;
        }

        return true;
    }

    public void Select()
    {
        selected = true;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    public void Deselect()
    {
        selected = false;
        var overlappingGameObjects = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        foreach (var i in overlappingGameObjects)
        {
            if (i != gameObject.GetComponent<Collider2D>())
            {
                Destroy(i.gameObject);
            }
        }

        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public void Duplicate()
    {
        if (spriteGameObjectId != 1)
        {
            var overlappingGameObjects = Physics2D.OverlapCircleAll(transform.position, 0.3f);
            foreach (var i in overlappingGameObjects)
            {
                if (i != gameObject.GetComponent<Collider2D>() && i.GetComponent<SelectSprite>() != null)
                {
                    Destroy(i.gameObject);
                }
            }

            var duplicatedSprite = Instantiate(gameObject, transform.position, Quaternion.identity);

            var duplicatedSpriteSelectSprite = duplicatedSprite.GetComponent<SelectSprite>();
            
            duplicatedSpriteSelectSprite.selected = false;
            duplicatedSpriteSelectSprite.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            duplicatedSprite.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void ApplyProprieties()
    {
        if (limitedStep != 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = editorHandler.LimitedSprites[limitedStep - 1];
        }

        if (movesLimit > 0)
        {
            GameObject.Find("PlayerText").GetComponent<Text>().text = movesLimit.ToString();
        }
        else if (movesLimit == 0)
        {
            GameObject.Find("PlayerText").GetComponent<Text>().text = "";
        }
    }
}