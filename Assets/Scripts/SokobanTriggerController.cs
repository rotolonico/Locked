using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanTriggerController : MonoBehaviour
{
    public bool isActive;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var iceBlockController = other.GetComponent<IceBlockController>();
        if (other.CompareTag("SokobanBlock") && !iceBlockController.sliding)
        {
            isActive = true;
            EditorHandler.CheckSokobanBlocks();    
        } else if (other.CompareTag("SokobanBlock") && iceBlockController.sliding)
        {
            StartCoroutine(CheckSliding(iceBlockController));
        }
    }

    private IEnumerator CheckSliding(IceBlockController iceBlockController)
    {
        yield return new WaitForSeconds(0.1f);
        if (!iceBlockController.sliding)
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
