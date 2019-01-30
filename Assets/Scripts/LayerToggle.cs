using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerToggle : MonoBehaviour
{
    public Sprite layer1;
    public Sprite layer2;
    
    private EditorHandler editorHandler;
    private Toggle transformToggle;
    private Image transformImage;
    
    
    void Start()
    {
        editorHandler = GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>();
        transformToggle = transform.GetComponent<Toggle>();
        transformImage = transform.GetComponent<Image>();
    }

    public void OnClick()
    {
        if (!EditorHandler.isnotFirstChangeLayerTime)
        {
            editorHandler.HelpWindow5();
        }

        if (transformToggle.isOn)
        {
            transformImage.sprite = layer1;
        }
        else
        {
            transformImage.sprite = layer2;
        }

        editorHandler.ChangeLayer();
    }
}
