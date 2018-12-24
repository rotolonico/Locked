using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    public GameObject Selectable;
    private EditorHandler editorHandler;

    private void Start()
    {
        editorHandler = GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>();
    }

    public void OnClick()
    {
        editorHandler.selectedObject = Selectable;
    }
}