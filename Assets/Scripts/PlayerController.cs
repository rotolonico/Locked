using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Sprite Background;
    public SpriteRenderer KeySr;
    public SpriteRenderer RedKeySr;
    public SpriteRenderer GreenKeySr;
    public SpriteRenderer BlueKeySr;
    public AudioSource GetKeySound;
    public Transform KeyImage;
    public Transform RedKeyImage;
    public Transform GreenKeyImage;
    public Transform BlueKeyImage;

    private readonly Vector3 screenSize = new Vector3(Screen.width / 130, 3, 0);
    private Vector2 transformedPosition;
    private GameObject camera;
    private GameObject[] keyImages;
    private GameObject[] redKeyImages;
    private GameObject[] greenKeyImages;
    private GameObject[] blueKeyImages;
    private Rigidbody2D[] keyImagesRb;
    private Rigidbody2D[] redKeyImagesRb;
    private Rigidbody2D[] greenKeyImagesRb;
    private Rigidbody2D[] blueKeyImagesRb;
    private Transform KeySpawn;
    private readonly Vector2 keyShift = new Vector2(1, 0);
    private GameObject[] gameObjects;
    

    public int HasKey;
    public int HasRedKey;
    public int HasGreenKey;
    public int HasBlueKey;

    public bool Teleporting;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        KeySpawn = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0);
    }

    private void Update()
    {
            Rescale();
            if (HasKey > 0)
            {
                KeySr.enabled = true;
            }
            else
            {
                KeySr.enabled = false;
            }

            if (HasRedKey > 0)
            {
                RedKeySr.enabled = true;
            }
            else
            {
                RedKeySr.enabled = false;
            }

            if (HasGreenKey > 0)
            {
                GreenKeySr.enabled = true;
            }
            else
            {
                GreenKeySr.enabled = false;
            }

            if (HasBlueKey > 0)
            {
                BlueKeySr.enabled = true;
            }
            else
            {
                BlueKeySr.enabled = false;
            }

            if (HasKey < keyImages.Length)
            {
                Destroy(keyImages[0]);
            }

            if (HasRedKey < redKeyImages.Length)
            {
                Destroy(redKeyImages[0]);
            }

            if (HasGreenKey < greenKeyImages.Length)
            {
                Destroy(greenKeyImages[0]);
            }

            if (HasBlueKey < blueKeyImages.Length)
            {
                Destroy(blueKeyImages[0]);
            }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            if (other.gameObject.CompareTag("key") && other.GetComponent<SpriteRenderer>().sprite != Background)
            {
                HasKey += 1;
                other.GetComponent<SpriteRenderer>().sprite = Background;
                KeySr.enabled = true;
                Instantiate(KeyImage, camera.transform.position - screenSize - (Vector3) keyShift * 50,
                        Quaternion.identity).transform.parent =
                    GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0);
                GetKeySound.Play();
            }

            if (other.gameObject.CompareTag("redKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
            {
                HasRedKey += 1;
                other.GetComponent<SpriteRenderer>().sprite = Background;
                RedKeySr.enabled = true;
                Instantiate(RedKeyImage, camera.transform.position - screenSize - (Vector3) keyShift * 50,
                        Quaternion.identity).transform.parent =
                    GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0);
                GetKeySound.Play();
            }

            if (other.gameObject.CompareTag("greenKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
            {
                HasGreenKey += 1;
                other.GetComponent<SpriteRenderer>().sprite = Background;
                GreenKeySr.enabled = true;
                Instantiate(GreenKeyImage, camera.transform.position - screenSize - (Vector3) keyShift * 50,
                        Quaternion.identity).transform.parent =
                    GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0);
                GetKeySound.Play();
            }

            if (other.gameObject.CompareTag("blueKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
            {
                HasBlueKey += 1;
                other.GetComponent<SpriteRenderer>().sprite = Background;
                BlueKeySr.enabled = true;
                Instantiate(BlueKeyImage, camera.transform.position - screenSize - (Vector3) keyShift * 50,
                        Quaternion.identity).transform.parent =
                    GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0);
                GetKeySound.Play();
            }
    }

    private void ReloadKeys()
    {
        keyImages = GameObject.FindGameObjectsWithTag("KeyImage");
        redKeyImages = GameObject.FindGameObjectsWithTag("RedKeyImage");
        greenKeyImages = GameObject.FindGameObjectsWithTag("GreenKeyImage");
        blueKeyImages = GameObject.FindGameObjectsWithTag("BlueKeyImage");
        keyImagesRb = new Rigidbody2D[keyImages.Length];
        redKeyImagesRb = new Rigidbody2D[redKeyImages.Length];
        greenKeyImagesRb = new Rigidbody2D[greenKeyImages.Length];
        blueKeyImagesRb = new Rigidbody2D[blueKeyImages.Length];
        for (var i = 0; i < keyImages.Length; i++)
        {
            keyImagesRb[i] = keyImages[i].GetComponent<Rigidbody2D>();
        }

        for (var i = 0; i < redKeyImages.Length; i++)
        {
            redKeyImagesRb[i] = redKeyImages[i].GetComponent<Rigidbody2D>();
        }

        for (var i = 0; i < greenKeyImages.Length; i++)
        {
            greenKeyImagesRb[i] = greenKeyImages[i].GetComponent<Rigidbody2D>();
        }

        for (var i = 0; i < blueKeyImages.Length; i++)
        {
            blueKeyImagesRb[i] = blueKeyImages[i].GetComponent<Rigidbody2D>();
        }
    }

    private void Rescale()
    {
        ReloadKeys();
        for (var i = 0; i < keyImagesRb.Length; i++)
        {
            keyImagesRb[i].MovePosition(KeySpawn.position + (Vector3) keyShift * i);
        }

        for (var i = 0; i < redKeyImagesRb.Length; i++)
        {
            redKeyImagesRb[i].MovePosition(KeySpawn.position + (Vector3) keyShift * (i + keyImagesRb.Length));
        }

        for (var i = 0; i < greenKeyImagesRb.Length; i++)
        {
            greenKeyImagesRb[i].MovePosition(KeySpawn.position + (Vector3) keyShift * (i + keyImagesRb.Length + redKeyImagesRb.Length));
        }

        for (var i = 0; i < blueKeyImagesRb.Length; i++)
        {
            blueKeyImagesRb[i].MovePosition(KeySpawn.position + (Vector3) keyShift * (i + keyImagesRb.Length + redKeyImagesRb.Length + greenKeyImagesRb.Length));
        }
    }
}