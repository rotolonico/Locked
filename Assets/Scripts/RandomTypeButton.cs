using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomTypeButton : MonoBehaviour
{
    public void onClick()
    {
        var text = transform.GetChild(0).GetComponent<Text>();

        if (text.text == "all")
        {
            text.text = "portal";
        } else if (text.text == "portal")
        {
            text.text = "number";
        } else if (text.text == "number")
        {
            text.text = "other";
        }  else if (text.text == "other")
        {
            text.text = "all";
        }
    }
}
