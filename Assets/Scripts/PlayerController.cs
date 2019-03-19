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
    public Transform playerSpawn;

    private readonly Vector3 screenSize = new Vector3(Screen.width / 130, 3, 0);
    private Transform keyHolder;
    private Animator keyHolderAnimator;
    private EditorHandler editorHandler;
    private Toggle playToggle;
    private GameObject currentPlayerSpawn;
    
    public bool hasLimitedMoves;
    public bool isKeyListUp;
    public bool sliding;
    public bool inWater;
    public bool invertedControls;
    
    private Vector2 waterScale = new Vector2(0.01f, 0.01f);
    private Vector2 waterScale2 = new Vector2(0.01f, 0.01f);

    public int movesLimit;

    public int HasKey;
    public int HasRedKey;
    public int HasGreenKey;
    public int HasBlueKey;

    public bool Teleporting;
    
    private BotController botController;
    private LeftController leftController;
    private RightController rightController;
    private TopController topController;
    private SpriteRenderer sr;

    void Start()
    {
        StartCoroutine(ReloadMovesCoroutine());
        if (EditorHandler.isBackToCheckpoint)
        {
            IsBackToCheckpoint();
        }

        editorHandler = GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>();
        keyHolder = GameObject.FindGameObjectWithTag("keyList").transform;
        keyHolderAnimator = GameObject.FindGameObjectWithTag("keyInventory").GetComponent<Animator>();
        
        sr = gameObject.GetComponent<SpriteRenderer>();
        botController = gameObject.GetComponentInChildren<BotController>();
        leftController = gameObject.GetComponentInChildren<LeftController>();
        rightController = gameObject.GetComponentInChildren<RightController>();
        topController = gameObject.GetComponentInChildren<TopController>();
    }

    private IEnumerator ReloadMovesCoroutine()
    {
        yield return new WaitForSeconds(0);
        ReloadMoves();
    }

    public void ReloadMoves()
    {
        if (movesLimit > 0)
        {
            GameObject.Find("PlayerText").GetComponent<Text>().text = movesLimit.ToString();
            hasLimitedMoves = true;
        }
    }

    public void IsBackToCheckpoint()
    {
        EditorHandler.GameOver = false;
        var playerSpawn = GameObject.FindGameObjectWithTag("playerSpawn");
        transform.position = playerSpawn.transform.position;
        EditorHandler.isBackToCheckpoint = false;
        playerSpawn.GetComponent<PlayerSpawnController>().Initialize();
        ClearKeys();
        CheckpointTouched();
    }

    public void ClearKeys()
    {
        Transform keyInventory = GameObject.Find("KeyListContent").transform;
        if (keyInventory.childCount != 0)
        {
            isKeyListUp = true;
            HasKey = 0;
            HasRedKey = 0;
            HasBlueKey = 0;
            HasGreenKey = 0;
        }
        foreach (Transform child in keyInventory.transform)
        {
            Destroy(child.gameObject);
        }
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

        if (!isKeyListUp && (HasKey > 0 || HasRedKey > 0 || HasGreenKey > 0 || HasBlueKey > 0))
        {
            keyHolderAnimator.Play("PopupAnimation");
            isKeyListUp = true;
        }

        if (isKeyListUp && HasKey == 0 && HasRedKey == 0 && HasGreenKey == 0 && HasBlueKey == 0)
        {
            keyHolderAnimator.Play("PopdownAnimation");
            isKeyListUp = false;
        }

        if (inWater)
        {
            sr.size -= waterScale;
            var srColor = sr.color;
            srColor.a -= 0.01f;
            sr.color = srColor;
            if (!(sr.size.x < 0.01)) return;
            Destroy(gameObject);
            editorHandler.RestartLevelFromCp();
        } else if (sr.size.x < 0.99)
        {
            var srColor = sr.color;
            srColor.a += 0.01f;
            sr.color = srColor;
            sr.size += waterScale2;
        } else if (sr.size.x >= 0.99)
        {
            var srColor = sr.color;
            srColor.a = 1f;
            sr.color = srColor;
            sr.size = new Vector2(1,1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("topForce"))
        {
            topController.RemoteMove();
        }
        
        if (other.gameObject.CompareTag("botForce"))
        {
            botController.RemoteMove();
        }
        
        if (other.gameObject.CompareTag("leftForce"))
        {
            leftController.RemoteMove();
        }
        
        if (other.gameObject.CompareTag("rightForce"))
        {
            rightController.RemoteMove();
        }
        
        if (other.gameObject.CompareTag("key") && other.GetComponent<SpriteRenderer>().sprite != Background)
        {
            HasKey += 1;
            other.GetComponent<SpriteRenderer>().sprite = Background;
            KeySr.enabled = true;
            Instantiate(KeyImage, keyHolder.position, Quaternion.identity).SetParent(keyHolder, false);
            GetKeySound.Play();
        }

        if (other.gameObject.CompareTag("redKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
        {
            HasRedKey += 1;
            other.GetComponent<SpriteRenderer>().sprite = Background;
            RedKeySr.enabled = true;
            Instantiate(RedKeyImage, keyHolder.position,
                Quaternion.identity).SetParent(keyHolder, false);
            GetKeySound.Play();
        }

        if (other.gameObject.CompareTag("greenKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
        {
            HasGreenKey += 1;
            other.GetComponent<SpriteRenderer>().sprite = Background;
            GreenKeySr.enabled = true;
            Instantiate(GreenKeyImage, keyHolder.position,
                Quaternion.identity).SetParent(keyHolder, false);
            GetKeySound.Play();
        }

        if (other.gameObject.CompareTag("blueKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
        {
            HasBlueKey += 1;
            other.GetComponent<SpriteRenderer>().sprite = Background;
            BlueKeySr.enabled = true;
            Instantiate(BlueKeyImage, keyHolder.position,
                Quaternion.identity).SetParent(keyHolder, false);
            GetKeySound.Play();
        }
        
        if (other.gameObject.CompareTag("checkpoint"))
        {
            CheckpointTouched();
        }

        if (other.gameObject.CompareTag("end"))
        {
            EndLevel();
        }
    }

    private void CheckpointTouched()
    {
        if (GameObject.FindGameObjectWithTag("playerSpawn") == null)
        {
            Instantiate(playerSpawn, transform.position, Quaternion.identity);
        }

        currentPlayerSpawn = GameObject.FindGameObjectWithTag("playerSpawn");
        currentPlayerSpawn.transform.position = transform.position;
    }

    public void RemoveKeys()
    {
        var keyImages = GameObject.FindGameObjectsWithTag("KeyImage");
        for (var i = 0; i < keyImages.Length - HasKey; i++)
        {
            Destroy(keyImages[i]);
        }
    }

    public void RemoveRedKeys()
    {
        var keyImages = GameObject.FindGameObjectsWithTag("RedKeyImage");
        for (var i = 0; i < keyImages.Length - HasRedKey; i++)
        {
            Destroy(keyImages[i]);
        }
    }

    public void RemoveGreenKeys()
    {
        var keyImages = GameObject.FindGameObjectsWithTag("GreenKeyImage");
        for (var i = 0; i < keyImages.Length - HasGreenKey; i++)
        {
            Destroy(keyImages[i]);
        }
    }

    public void RemoveBlueKeys()
    {
        var keyImages = GameObject.FindGameObjectsWithTag("BlueKeyImage");
        for (var i = 0; i < keyImages.Length - HasBlueKey; i++)
        {
            Destroy(keyImages[i]);
        }
    }
    
    public void EndLevel()
    {
        Debug.Log(EditorHandler.PublishMode);
        if (EditorHandler.playingChallenge)
        {
            DatabaseHandler.PostDailyChallengeScore(EditorHandler.DailyChallengeScore.ToString());
            DatabaseHandler.PostDailyChallengeStreak(EditorHandler.DailyChallengeStreak.ToString());
            EditorHandler.playingChallenge = false;
        }
        
        if (EditorHandler.playingOnlineLevel && !EditorHandler.onChallenge)
        {
            DatabaseHandler.WinLevel(EditorHandler.onlineLevelId);
        }

        if (EditorHandler.onChallenge && !EditorHandler.playingChallenge)
        {
            if (EditorHandler.challengeDay > int.Parse(EditorHandler.currentChallengeNumber) + 1)
            {
                editorHandler.LoadDailyChallengeLevelInLevelScene(
                    (int.Parse(EditorHandler.currentChallengeNumber) + 1).ToString(), false);
            }
            else
            {
                SceneManager.LoadScene(0);
                Destroy(GameObject.Find("EditorHandler"));
            }
        }
        
        if (EditorHandler.PublishMode)
        {
            if (!EditorHandler.PublishDailyLevel)
            {
                DatabaseHandler.PostLevel(EditorHandler.objectSavedLevel);
            }
            else
            {
                DatabaseHandler.GetDailyChallengeNumber(day =>
                {
                    var challengeDay = int.Parse(day.Substring(1, day.Length - 2));
                    DatabaseHandler.PostDailyChallengeLevel(EditorHandler.objectSavedLevel, (challengeDay + 1).ToString());
                    EditorHandler.challengeDay = challengeDay + 1;
                    DatabaseHandler.PostDailyChallengeNumber(EditorHandler.challengeDay.ToString());
                });
            }

            EditorHandler.PublishMode = false;
            EditorHandler.PublishMode = false;
            EditorHandler.playMode = false;
            EditorHandler.reloadLevel = true;
            SceneManager.LoadScene(4);
            EditorHandler.PublishDailyLevel = false;
        }
        else if (EditorHandler.playMode && EditorHandler.inEditor)
        {
            playToggle = GameObject.FindGameObjectWithTag("PlayButton").GetComponent<Toggle>();
            playToggle.isOn = false;
            editorHandler.EditorMode();
        }
        else
        {
            if (EditorHandler.currentLevelNumber != 0 && EditorHandler.currentLevelNumber < EditorHandler.maxLevelNumber)
            {
                if (EditorHandler.beatenLevelNumber < EditorHandler.currentLevelNumber)
                {
                    EditorHandler.beatenLevelNumber = EditorHandler.currentLevelNumber;
                    EditorHandler.SaveToFile(EditorHandler.beatenLevelNumber, EditorHandler.isnotFirstTimeEditor, !EditorHandler.isnotFirstSelectTime, !EditorHandler.isnotFirstPlaceTime, EditorHandler.isnotFirstSokobanPlaceTime, EditorHandler.isnotFirstChangeLayerTime);
                }
                editorHandler.LoadNormalLevelInLevelScene(EditorHandler.currentLevelNumber+1);
                Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
            }
            else
            {   
                Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
                
                if (EditorHandler.playingOnlineLevel && !EditorHandler.onChallenge)
                {
                    editorHandler.OnlineLevelsRoom();
                }
                else if (!EditorHandler.onChallenge)
                {
                    SceneManager.LoadScene(0);
                    Destroy(GameObject.Find("EditorHandler"));
                }
            }
        }
    }
}