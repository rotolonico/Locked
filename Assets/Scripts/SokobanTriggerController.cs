using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanTriggerController : MonoBehaviour
{
    public bool isActive;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SokobanBlock"))
        {
            isActive = true;
            EditorHandler.CheckSokobanBlocks();    
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SokobanBlock"))
        {
            isActive = false;
        }
    }
}
