using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    public GameObject Selectable;
    private EditorHandler playerController;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>();
    }

    public void OnClick()
    {
        playerController.selectedObject = Selectable;
    }
}