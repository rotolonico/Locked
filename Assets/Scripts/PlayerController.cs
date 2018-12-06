using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private EditorHandler editorHandler;
    private Toggle playToggle;
    

    public int HasKey;
    public int HasRedKey;
    public int HasGreenKey;
    public int HasBlueKey;

    public bool Teleporting;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        KeySpawn = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0);
        editorHandler = GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>();
    }

    private void Update()
    {
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

        if (other.gameObject.CompareTag("end") && !EditorHandler.PublishMode)
        {
            if (EditorHandler.PublishMode)
            {
                
            } else if (EditorHandler.playMode)
            {
                playToggle = GameObject.FindGameObjectWithTag("PlayButton").GetComponent<Toggle>();
                playToggle.isOn = false;
                editorHandler.EditorMode();
            }
            else
            {
                SceneManager.LoadScene(3);
            }
        }
    }
}