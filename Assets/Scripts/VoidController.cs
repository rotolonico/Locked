using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidController : MonoBehaviour
{
    public Sprite voidFilled;
    public AudioSource fillSound;

    private SpriteRenderer sr;
    private bool filled;
    private bool done;

    void Start()
    {
        sr = transform.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!filled)
        {
            if (other.CompareTag("block"))
            {
                Destroy(other.gameObject);
                other.GetComponent<SpriteRenderer>().sortingOrder = 0;
                filled = true;
                fillSound.Play();
                sr.sprite = voidFilled;
                transform.tag = "HoleFilled";
            }
        }
    }
}