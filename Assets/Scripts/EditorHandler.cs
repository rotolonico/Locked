﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FullSerializer;
using Proyecto26;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;
using Version = System.Version;

public class EditorHandler : MonoBehaviour
{
    public string Version = "v0.8-alpha"; 
    
    public GameObject selectedObject;
    public GameObject GridSprite;
    public GameObject ChallengeLevelButton;
    public AudioSource EditorMusic;
    private static AudioSource menuMusic;

    public Color hiddenLayerColor;
    public Color normalColor;

    private Plane mouseHitPlane;
    private Ray mousePositionRay;
    private Ray touchPositionRay;
    private Collider2D[] hitColliders;
    private Collider2D[] selectedColliders;
    private Animator animatorSelector;
    private Animator animatorToggle;
    private Animator animatorBack;
    private Toggle blockToggle;
    private Button backButton;
    private bool musicOn;
    public static bool PublishDailyLevel;

    public static int levelRows = 30;
    public static int levelColumns = 30;
    readonly float mouseThreshold = 0.2f;

    public static Level objectSavedLevel;

    public Sprite likedSprite;
    public GameObject[] randomSpawnableBlocks = new GameObject[34];
    public GameObject[] randomSpawnablePortals = new GameObject[4];
    public GameObject[] randomSpawnableOther = new GameObject[21];
    public GameObject randomSpawnableLimitedBlock;
    public GameObject[] blocks = new GameObject[36];
    public GameObject[] editorBlocks = new GameObject[36];

    public Button[] levelButtons = new Button[10];
    private string[] levels;

    public static bool playMode;
    public static bool PublishMode;
    public static bool inEditor;
    public static bool GameOver;
    public static bool isBackToCheckpoint;
    public static bool playingOnlineLevel;
    public static bool loadOnlineLevel;
    public static bool playingChallenge;

    public static int DailyChallengeScore;
    public static int DailyChallengeStreak;

    public Sprite[] LimitedSprites;

    public AudioSource placeSound;
    public AudioSource noSound;
    public AudioSource yesSound;
    private bool EditorInitialized;
    private Toggle playButtonToggle;
    private Animator loadButtonAnimator;
    private Animator saveButtonAnimator;
    private Animator publishButtonAnimator;
    private Animator publishLevelWindowAnimator;
    private Button saveButton;
    private Button publishButton;
    private Image playButtonRenderer;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Sprite lockedLevelSprite;
    public Sprite mouseSprite;
    public Sprite selectionSprite;
    public GameObject LevelWall;

    private GameObject blockImage;
    private GameObject saveLevelWindow;
    private GameObject helpWindow;
    private GameObject helpWindow2;
    private GameObject helpWindow3;
    private GameObject helpWindow4;
    private GameObject helpWindow5;
    private GameObject quitLevelWindow;
    private GameObject publishLevelWindow;
    private GameObject newLevelWindow;
    private GameObject publishLevel;
    private GameObject levelSizeWindow;
    private GameObject saveLevel;
    private GameObject loadLevel;
    private GameObject levelText;
    private GameObject levelTextHint;
    private Animator helpButtonAnimator;
    private Button newLevelButton;
    private Toggle gridToggle;
    private Animator newLevelButtonAnimator;
    private Animator gridToggleAnimator;
    private Button helpButton;
    private Button settingsButton;
    private Animator settingsButtonAnimator;
    private Text levelCodeHint;
    private InputField inputField;
    private Animator keyHolderAnimator;
    private Toggle changeToolToggle;
    private Animator changeToolAnimator;
    private Toggle changeLayerToggle;
    private Animator changeLayerAnimator;
    private Image blockImageImage;
    private PlayerController playerController;
    public GameObject BlockSettings;
    public static Random Random = new Random();

    public static int beatenLevelNumber;
    public static bool isnotFirstTimeEditor;
    public static bool isnotFirstSelectTime;
    public static bool isnotFirstPlaceTime;
    public static bool isnotFirstSokobanPlaceTime;
    public static bool isnotFirstChangeLayerTime;
    public static int currentLevelNumber;
    public static int maxLevelNumber = 14;
    public static bool isOnline;
    public static bool checkVersion;
    public static string News;
    public static string onlineLevelId;
    public static bool onChallenge;
    public static string currentChallengeNumber;
    public static bool dailyManager;

    private Vector3 mousePosition;
    private Vector3 mouseEndPosition;
    private RectTransform selectionBox;
    private Collider2D[] checkSpriteCollision;
    private bool movingSelection;
    private bool cameraDelay;
    public static bool reloadLevel;
    private Vector3 newMousePosition;
    private Vector3 mousePosition1;
    private int tutorialProgress;
    private Text tutorialText;
    private String[] editorTutorialText = new String[8];
    bool isPointerOverGameObject = true;
    private bool hittingSelection;
    private bool hittingGUI;
    private GameObject[] grid;
    private InputField email;
    private InputField username;
    private InputField password;
    private Animator authWindowAnimator;
    private Animator clearDataWindowAnimator;
    private Animator dailyChallengeWindowAnimator;
    private Animator newsWindowAnimator;
    private Text levelNameHint;
    private InputField levelName;
    private Text levelColumnsHint;
    private Text levelRowsHint;
    public InputField levelColumnsText;
    public InputField levelRowsText;
    private GameObject blockSettings;
    private GameObject buttonProprietiesListContent;
    private SelectSprite hitColliderProprieties;
    private Button signupButton;
    private Button signinButton;
    private Button backFromAuthButton;
    private Button verifyLevel;
    private Button backFromPublish;
    private Canvas levelCanvas;
    private Canvas mainCanvas;
    private Text newsText;
    public static int challengeDay;
    private string alreadyAttemptedDailyChallengeText = "You have already attempted the challenge today!\n Come back tomorrow";
    private bool dailyManagerCheck;
    
    public delegate void OnDailyChallengeDayDownloadCompleted(string day); 
    public delegate void OnDailyChallengeLevelDownloadCompleted(Level level);
    public delegate void OnCheckAttemptChallengeCompleted(bool canAttempt);
    public delegate void GenericStringCallback(string gString);

    public static string levelNameString;
    public static string levelAuthorString;

    public bool tap = true;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadToFile();
        levelCanvas = GameObject.Find("LevelCanvas").GetComponent<Canvas>();
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        GameObject.Find("Version").GetComponent<Text>().text = "Version " + Version;
        mouseHitPlane = new Plane(Vector3.forward, transform.position);
        playMode = true;
        GameOver = false;
        LevelButtonsInitialize();
        menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        menuMusic.Play();
        currentLevelNumber = 0;
        playingOnlineLevel = false;
        placeSound = GameObject.Find("PlaceSound").GetComponent<AudioSource>();
        noSound = GameObject.Find("NoSound").GetComponent<AudioSource>();
        yesSound = GameObject.Find("YesSound").GetComponent<AudioSource>();
        InitializeButtons();
        if (isOnline)
        {
            OnlineMode();
        }
        if (!checkVersion)
        {
            GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;
            DatabaseHandler.CheckVersion(Version);
            DatabaseHandler.GetNews();
        }
    }

    private void InitializeChallengeButtons()
    {
        DatabaseHandler.GetDailyChallengeNumber(day =>
        {
            challengeDay = int.Parse(day.Substring(1, day.Length - 2));
            
            for (int i = 0; i < challengeDay-1; i++)
            {
                var challengeLevel = Instantiate(ChallengeLevelButton, GameObject.Find("ButtonChallengeListContent").transform, false);
                var dayNumber = i+1;
                challengeLevel.GetComponentInChildren<Text>().text = dayNumber.ToString();
                challengeLevel.GetComponent<Button>().onClick.AddListener(() => LoadDailyChallengeLevelInLevelScene(dayNumber.ToString(), false));
            }
        });
    }

    public static void CheckVersionFailed()
    {
        var mainCanvas = GameObject.Find("MainCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
        GameObject.Find("OfflineMessage").GetComponent<Animator>().Play("TopCanvasDown");
    }
    
    public static void CheckVersionOutdated()
    {
        var mainCanvas = GameObject.Find("MainCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
        GameObject.Find("OutdatedMessage").GetComponent<Animator>().Play("TopCanvasDown");
    }
    
    public void BackFromFailed()
    {
        GameObject.Find("OfflineMessage").GetComponent<Animator>().Play("TopCanvasUp");
        var mainCanvas = GameObject.Find("MainCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
        mainCanvas.GetComponent<Canvas>().enabled = true;
        
    }

    public void BackFromOutdated()
    {
        GameObject.Find("OutdatedMessage").GetComponent<Animator>().Play("TopCanvasUp");
        var mainCanvas = GameObject.Find("MainCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
        mainCanvas.GetComponent<Canvas>().enabled = true;
    }
    
    public void NewsWindow()
    {
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
        newsWindowAnimator.Play("TopCanvasDown");
        newsText.text = News.Replace ("\\n", "\n").Trim('"');
    }
    
    public void BackFromNews()
    {
        GameObject.Find("NewsWindow").GetComponent<Animator>().Play("TopCanvasUp");
        var mainCanvas = GameObject.Find("MainCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
    }

    public void OnlineMode()
    {
        isOnline = true;
        GameObject.Find("NewsButton").GetComponent<Button>().interactable = true;
        GameObject.Find("Authentication").GetComponent<Button>().interactable = true;
        GameObject.Find("Online Levels").GetComponent<Button>().interactable = true;
        GameObject.Find("Challenges").GetComponent<Button>().interactable = true;
        InitializeChallengeButtons();
    }
    
    private void InitializeButtons()
    {
        username = GameObject.Find("Username").GetComponent<InputField>();
        email = GameObject.Find("Email").GetComponent<InputField>();
        password = GameObject.Find("Password").GetComponent<InputField>();
        authWindowAnimator = GameObject.Find("AuthenticationWindow").GetComponent<Animator>();
        clearDataWindowAnimator = GameObject.Find("ClearDataWindow").GetComponent<Animator>();
        dailyChallengeWindowAnimator = GameObject.Find("DailyChallengeWindow").GetComponent<Animator>();
        newsText = GameObject.Find("News").GetComponent<Text>();
        newsWindowAnimator = GameObject.Find("NewsWindow").GetComponent<Animator>();
        signinButton = GameObject.Find("Signin").GetComponent<Button>();
        signupButton = GameObject.Find("Signup").GetComponent<Button>();
        backFromAuthButton = GameObject.Find("BackFromLogin").GetComponent<Button>();
    }

    private void AuthWindowReset()
    {
        username.placeholder.GetComponent<Text>().text = "Username (signup only)";
        email.placeholder.GetComponent<Text>().text = "Email";
        password.placeholder.GetComponent<Text>().text = "Password";
        username.text = "";
        email.text = "";
        password.text = "";
    }

    public void LikeLevel()
    {
       DatabaseHandler.LikeLevel(onlineLevelId);
       var likeButton = GameObject.Find("LikeButton").GetComponent<Button>();
       likeButton.enabled = false;
       likeButton.GetComponent<Image>().sprite = likedSprite;
    }

    public void SignupUser()
    {
        InitializeButtons();
        AuthHandler.SignupUser(email.text, username.text, password.text);
        
        signinButton.enabled = true;
        signupButton.enabled = true;
        backFromAuthButton.enabled = true;
        AuthWindowReset();
    }

    public void SigninUser()
    {
        InitializeButtons();
        AuthHandler.SigninUser(email.text, password.text);
        
        signinButton.enabled = false;
        signupButton.enabled = false;
        backFromAuthButton.enabled = false;
        AuthWindowReset();
    }
    
    public void SigninSucceded()
    {
        InitializeButtons();
        BackFromAuthWindow();
        yesSound.Play();
    }

    public void SigninFailed()
    {
        InitializeButtons();
        email.text = "";
        password.text = "";
        email.placeholder.GetComponent<Text>().text = "wrong email";
        password.placeholder.GetComponent<Text>().text = "or password";
        noSound.Play();
        
        signinButton.enabled = true;
        signupButton.enabled = true;
        backFromAuthButton.enabled = true;
    }
    
    public void SignupFailed()
    {
        InitializeButtons();
        username.text = "";
        email.text = "";
        password.text = "";
        username.placeholder.GetComponent<Text>().text = "username or";
        email.placeholder.GetComponent<Text>().text = "email or";
        password.placeholder.GetComponent<Text>().text = "password is/are invalid";
        noSound.Play();
        
        signinButton.enabled = true;
        signupButton.enabled = true;
        backFromAuthButton.enabled = true;
    }

    public void AuthWindow()
    {
        InitializeButtons();
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
        authWindowAnimator.Play("TopCanvasDown");
    }

    public void BackFromAuthWindow()
    {
        InitializeButtons();
        authWindowAnimator.Play("TopCanvasUp");
        var mainCanvas = GameObject.Find("MainCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
        AuthWindowReset();
    }
    
    public void ClearDataWindow()
    {
        InitializeButtons();
        levelCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
        clearDataWindowAnimator.Play("TopCanvasDown");
    }

    public void BackFromClearDataWindow()
    {
        InitializeButtons();
        clearDataWindowAnimator.Play("TopCanvasUp");
        var levelCanvas = GameObject.Find("LevelCanvas");
        levelCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
    }
    
    public void DailyChallengeWindow()
    {
        if (AuthHandler.userId != null){
            InitializeButtons();
            GameObject.Find("PlayDailyChallenge").GetComponent<Button>().interactable = true;
            mainCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
            dailyChallengeWindowAnimator.Play("TopCanvasDown");
            
            DatabaseHandler.GetDailyChallengeScore(score =>
            {
                GameObject.Find("DailyScore").GetComponent<Text>().text = "Your score: " + score;
                DailyChallengeScore = int.Parse(score)+1;
            });
        
            DatabaseHandler.GetDailyChallengeStreak(streak =>
            {
                GameObject.Find("DailyWinStreak").GetComponent<Text>().text = "Your winstreak: " + streak;
                DailyChallengeStreak = int.Parse(streak)+1;
            });
        }
        else
        {
            AuthWindow();
        }
    }
    
    public void BackFromDailyChallengeWindow()
    {
        InitializeButtons();
        dailyChallengeWindowAnimator.Play("TopCanvasUp");
        var mainCanvas = GameObject.Find("MainCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
    }

    private void LevelButtonsInitialize()
    {
        levelCanvas.GetComponent<CanvasGroup>().interactable = true;
        for (var i = 0; i < levelButtons.Length; i++)
        {
            if (beatenLevelNumber >= i)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].GetComponent<Image>().sprite = lockedLevelSprite;
                levelButtons[i].transform.GetChild(0).GetComponent<Text>().text = "";
                levelButtons[i].interactable = false;
            }
        }
    }

    private void EditorInitialize()
    {
        placeSound = GameObject.Find("PlaceSound").GetComponent<AudioSource>();
        noSound = GameObject.Find("NoSound").GetComponent<AudioSource>();
        yesSound = GameObject.Find("YesSound").GetComponent<AudioSource>();
        EditorInitialized = true;
        levelColumnsText = GameObject.Find("LevelColumns").GetComponent<InputField>();
        levelRowsText = GameObject.Find("LevelRows").GetComponent<InputField>();
        levelColumnsHint = GameObject.Find("LevelColumnsHint").GetComponent<Text>();
        levelRowsHint = GameObject.Find("LevelRowsHint").GetComponent<Text>();
        levelName = GameObject.Find("LevelNamePublish").GetComponent<InputField>();
        levelNameHint = levelName.placeholder.GetComponent<Text>();
        saveButton = GameObject.FindGameObjectWithTag("saveButton").GetComponent<Button>();
        helpButton = GameObject.Find("HelpButton").GetComponent<Button>();
        helpButtonAnimator = GameObject.Find("HelpButton").GetComponent<Animator>();
        newLevelButton = GameObject.Find("NewLevelButton").GetComponent<Button>();
        newLevelButtonAnimator = GameObject.Find("NewLevelButton").GetComponent<Animator>();
        gridToggle = GameObject.Find("GridToggle").GetComponent<Toggle>();
        gridToggleAnimator = GameObject.Find("GridToggle").GetComponent<Animator>();
        publishButton = GameObject.FindGameObjectWithTag("publishButton").GetComponent<Button>();
        loadButtonAnimator = GameObject.FindGameObjectWithTag("loadButton").GetComponent<Animator>();
        saveButtonAnimator = GameObject.FindGameObjectWithTag("saveButton").GetComponent<Animator>();
        publishButtonAnimator = GameObject.FindGameObjectWithTag("publishButton").GetComponent<Animator>();
        backButton = GameObject.FindGameObjectWithTag("back").GetComponent<Button>();
        blockToggle = GameObject.FindGameObjectWithTag("selectorActive").GetComponent<Toggle>();
        animatorToggle = GameObject.FindGameObjectWithTag("selectorActive").GetComponent<Animator>();
        animatorBack = GameObject.FindGameObjectWithTag("back").GetComponent<Animator>();
        playButtonRenderer = GameObject.FindGameObjectWithTag("PlayButton").GetComponentInChildren<Image>();
        playButtonToggle = GameObject.FindGameObjectWithTag("PlayButton").GetComponent<Toggle>();
        blockImage = GameObject.Find("BlockImage");
        blockImageImage = blockImage.GetComponent<Image>();
        saveLevelWindow = GameObject.Find("SaveLevelWindow");
        tutorialText = GameObject.Find("TutorialText").GetComponent<Text>();
        helpWindow = GameObject.Find("HelpWindow");
        helpWindow2 = GameObject.Find("HelpWindow 2");
        helpWindow3 = GameObject.Find("HelpWindow 3");
        helpWindow4 = GameObject.Find("HelpWindow 4");
        helpWindow5 = GameObject.Find("HelpWindow 5");
        quitLevelWindow = GameObject.Find("QuitLevelWindow");
        newLevelWindow = GameObject.Find("NewLevelWindow");
        levelSizeWindow = GameObject.Find("LevelSizeWindow");
        publishLevel = GameObject.Find("PublishLevel");
        saveLevel = GameObject.Find("SaveLevel");
        loadLevel = GameObject.Find("LoadLevel");
        levelText = GameObject.Find("LevelName");
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
        keyHolderAnimator = GameObject.FindGameObjectWithTag("keyInventory").GetComponent<Animator>();
        changeToolToggle = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Toggle>();
        changeLayerToggle = GameObject.Find("LayerToggle").GetComponent<Toggle>();
        changeToolAnimator = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Animator>();
        changeLayerAnimator = GameObject.Find("LayerToggle").GetComponent<Animator>();
        publishLevelWindow = GameObject.Find("PublishLevelWindow");
        publishLevelWindowAnimator = GameObject.Find("PublishLevelWindow").GetComponent<Animator>();
        settingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
        settingsButtonAnimator = GameObject.Find("SettingsButton").GetComponent<Animator>();
        verifyLevel = GameObject.Find("VerifyLevel").GetComponent<Button>();
        backFromPublish = GameObject.Find("BackFromPublish").GetComponent<Button>();
        editorTutorialText[0] =
            "Welcome to the game\'s editor!\nHere is where the magic happens\nDo you want to learn how it works?";
        editorTutorialText[1] =
            "Click the \"block button\" to get a list of all available blocks\nClick anywhere on the screen to place the block you selected! \nYou can \"right-click\" some blocks to change their proprieties";
        editorTutorialText[2] = "Click the \"play button\" to test your awesome level";
        editorTutorialText[3] =
            "The \"save button\" will make you able to save the level you are\ncreating\nGive the save a name and then use the \"load button\" to load it back in";
        editorTutorialText[4] =
            "Are you ready to publish your level to the world?\nClick on the \"publish button\" and, after you verified it\nyour level will be visible to everyone in the online levels section!";
        editorTutorialText[5] =
            "As of right now there are two different tools you can use to \nmake your levels:\n- The draw tool, that allows you to place blocks in your level\n- The selection tool, that allows you to select a group of blocks\nmove, clone (ctrl+c) or delete them (canc)\nUse the \"change tool button\" to switch between the tools";
        editorTutorialText[6] =
            "Finally, you can click on the \"grid button\" to show/hide\n an useful grid that will indicate the rooms' borders";
        editorTutorialText[7] = "You can access this tutorial at any time by clicking on the \"help button\"";

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if (dailyManagerCheck == false)
        {
            DatabaseHandler.CheckDailyChallengeManager(isManager =>
            {
                dailyManagerCheck = true;
                if (isManager)
                {
                    dailyManager = true;
                }
            });
        }

        if (dailyManager)
        {
            GameObject.Find("VerifyDailyLevel").GetComponent<Image>().enabled = true;
            GameObject.Find("VerifyDailyLevel").GetComponentInChildren<Text>().enabled = true;
        }
        inputField.characterValidation = InputField.CharacterValidation.EmailAddress;
        levelRowsText.characterValidation = InputField.CharacterValidation.Integer;
        levelColumnsText.characterValidation = InputField.CharacterValidation.Integer;
    }

    private void BordersInitialize()
    {
        levelRowsText.text = levelRows.ToString();
        levelColumnsText.text = levelColumns.ToString();
        ChangeSize(true);
    }

    public static void CheckSokobanBlocks()
    {
        var sokobanBlocks = GameObject.FindGameObjectsWithTag("SokobanTrigger");
        foreach (var block in sokobanBlocks)
        {
            if (block.GetComponent<SokobanTriggerController>() != null && !block.GetComponent<SokobanTriggerController>().isActive)
            {
                return;
            }
        }
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.EndLevel();
    }

    void Update()
    {        
        if (!EditorInitialized && SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (loadOnlineLevel)
            {
                LoadLevelInEditor(onlineLevel: true);
            }

            if (!isnotFirstTimeEditor)
            {
                EditorFirstTime();
            }

            inEditor = true;
            EditorInitialize();
            BordersInitialize();
        }

        if (reloadLevel && SceneManager.GetActiveScene().buildIndex == 1)
        {
            LoadLevelInEditor(objectSavedLevel);
            reloadLevel = false;
        }

        if (inEditor && SceneManager.GetActiveScene().buildIndex != 1)
        {
            inEditor = false;
        }

        if (playMode)
        {
            if (musicOn)
            {
                EditorMusic.Stop();
                musicOn = false;
            }
        }

        if (!playMode)
        {
            if (!musicOn)
            {
                EditorMusic.Play();
                musicOn = true;
            }

            if (EditorInitialized && Input.GetMouseButtonDown(0) && tap)
            {
                mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                float dist;
                if (mouseHitPlane.Raycast(mousePositionRay, out dist))
                {
                    mousePosition = mousePositionRay.GetPoint(dist);
                    mousePosition.x = (float) Math.Round(mousePosition.x);
                    mousePosition.y = (float) Math.Round(mousePosition.y);
                    mousePosition.z = (float) Math.Round(mousePosition.z);
                    hitColliders = Physics2D.OverlapCircleAll(mousePosition, 0.3f);
                    hittingGUI = true;
                    if (hitColliders.Length == 0 || !hitColliders[0].CompareTag("BlockSettings"))
                    {
                        ApplySettings();
                    }
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (GameObject.FindGameObjectsWithTag("BlockSettings").Length == 0)
                        {
                            hittingGUI = false;
                            foreach (var i in hitColliders)
                            {
                                if (i.gameObject.GetComponent<SelectSprite>() != null &&
                                    i.gameObject.GetComponent<SelectSprite>().selected)
                                {
                                    hittingSelection = true;
                                    break;
                                }
                            }

                            if (changeToolToggle.isOn && hitColliders.Length > 0 && hittingSelection)
                            {
                                movingSelection = true;
                            }

                            hittingSelection = false;
                            changeToolToggle = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Toggle>();
                            if (!changeToolToggle.isOn)
                            {
                                foreach (var i in hitColliders)
                                {
                                    if (!i.gameObject.CompareTag("LevelWall"))
                                    {
                                        if (!i.gameObject.GetComponent<SelectSprite>().hidden)
                                        {
                                            Destroy(i.gameObject);
                                        }
                                    }
                                }

                                if (selectedObject.CompareTag("1"))
                                {
                                    Destroy(GameObject.FindGameObjectWithTag("1"));
                                }

                                if (!selectedObject.CompareTag("0"))
                                {
                                    Instantiate(selectedObject, mousePosition, Quaternion.identity);
                                    if (!isnotFirstPlaceTime)
                                    {
                                        HelpWindow3();
                                    }
                                    if (!isnotFirstSokobanPlaceTime && (selectedObject.CompareTag("35") || selectedObject.CompareTag("36")))
                                    {
                                        HelpWindow4();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (movingSelection)
                {
                    Vector2 levelEdgeA = new Vector2(-levelColumns / 2 - 5, -levelRows / 2 -5);
                    Vector2 levelEdgeB = new Vector2(levelColumns / 2 + 5, levelRows / 2 + 5);
                    
                    var allSprites = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
                    mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float dist;
                    if (mouseHitPlane.Raycast(mousePositionRay, out dist))
                    {
                        newMousePosition = mousePositionRay.GetPoint(dist);
                        newMousePosition.x = (float) Math.Round(newMousePosition.x);
                        newMousePosition.y = (float) Math.Round(newMousePosition.y);
                        newMousePosition.z = (float) Math.Round(newMousePosition.z);
                    }

                    if (Vector3.Distance(mousePosition, newMousePosition) > mouseThreshold)
                    {
                        if (mousePosition.y > newMousePosition.y)
                        {
                            foreach (var i in allSprites)
                            {
                                if (i.GetComponent<SelectSprite>() != null && i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.down;
                                    i.GetComponent<SelectSprite>().CheckPosition();
                                }
                            }

                            mousePosition.y -= 1;
                        }

                        if (mousePosition.y < newMousePosition.y)
                        {
                            foreach (var i in allSprites)
                            {
                                if (i.GetComponent<SelectSprite>() != null && i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.up;
                                    i.GetComponent<SelectSprite>().CheckPosition();
                                }
                            }

                            mousePosition.y += 1;
                        }

                        if (mousePosition.x > newMousePosition.x)
                        {
                            foreach (var i in allSprites)
                            {
                                if (i.GetComponent<SelectSprite>() != null && i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.left;
                                    i.GetComponent<SelectSprite>().CheckPosition();
                                }
                            }

                            mousePosition.x -= 1;
                        }

                        if (mousePosition.x < newMousePosition.x)
                        {
                            foreach (var i in allSprites)
                            {
                                if (i.GetComponent<SelectSprite>() != null && i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.right;
                                    i.GetComponent<SelectSprite>().CheckPosition();
                                }
                            }

                            mousePosition.x += 1;
                        }
                    }
                }
            }


            if (EditorInitialized && Input.GetKeyDown(KeyCode.Delete))
            {
                if (selectedColliders != null)
                {
                    foreach (var i in selectedColliders)
                    {
                        if (i != null && i.GetComponent<SelectSprite>() != null && !i.gameObject.GetComponent<SelectSprite>().hidden)
                        {
                            Destroy(i.gameObject);
                        }
                    }
                }
            }

            if (EditorInitialized && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
            {
                if (selectedColliders != null)
                {
                    foreach (var i in selectedColliders)
                    {
                        if (i != null && i.GetComponent<SelectSprite>() != null)
                        {
                            i.GetComponent<SelectSprite>().Duplicate();
                        }
                    }
                }
            }

            if (EditorInitialized && Input.GetMouseButtonUp(0) && tap && !hittingGUI)
            {
                isPointerOverGameObject = true;
                changeToolToggle = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Toggle>();
                if (!movingSelection && changeToolToggle.isOn)
                {
                    mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float dist;
                    if (mouseHitPlane.Raycast(mousePositionRay, out dist))
                    {
                        mouseEndPosition = mousePositionRay.GetPoint(dist);
                        mouseEndPosition.x = (float) Math.Round(mouseEndPosition.x);
                        mouseEndPosition.y = (float) Math.Round(mouseEndPosition.y);
                        mouseEndPosition.z = (float) Math.Round(mouseEndPosition.z);
                        if (selectedColliders != null)
                        {
                            foreach (var i in selectedColliders)
                            {
                                if (i != null && i.GetComponent<SelectSprite>() != null && !i.GetComponent<SelectSprite>().hidden)
                                {
                                    i.GetComponent<SelectSprite>().Deselect();
                                    i.GetComponent<SpriteRenderer>().color = Color.white;
                                }
                            }
                        }

                        selectedColliders = Physics2D.OverlapAreaAll(mousePosition, mouseEndPosition);
                        foreach (var i in selectedColliders)
                        {
                            if (!isnotFirstSelectTime)
                            {
                                HelpWindow2();
                            }

                            if (i.GetComponent<SelectSprite>() != null)
                            {
                                if (!i.GetComponent<SelectSprite>().hidden)
                                {
                                    i.GetComponent<SelectSprite>().Select();
                                    i.GetComponent<SpriteRenderer>().color = Color.grey;
                                }
                            }
                        }
                    }
                }

                movingSelection = false;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && tap)
            {
                Touch touch = Input.GetTouch(0);
                touchPositionRay = Camera.main.ScreenPointToRay(touch.position);
                float dist;
                if (mouseHitPlane.Raycast(touchPositionRay, out dist))
                {
                    Vector3 mousePosition = touchPositionRay.GetPoint(dist);
                    mousePosition.x = (float) Math.Round(mousePosition.x);
                    mousePosition.y = (float) Math.Round(mousePosition.y);
                    mousePosition.z = (float) Math.Round(mousePosition.z);
                    hitColliders = Physics2D.OverlapCircleAll(mousePosition, 0.3f);
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        foreach (var i in hitColliders)
                        {
                            if (!i.gameObject.GetComponent<SelectSprite>().hidden)
                            {
                                Destroy(i.gameObject);
                            }
                        }

                        if (selectedObject.CompareTag("1"))
                        {
                            Destroy(GameObject.FindGameObjectWithTag("1"));
                        }

                        if (!selectedObject.CompareTag("0"))
                        {
                            Instantiate(selectedObject, mousePosition, Quaternion.identity);
                            placeSound.Play();
                        }
                    }
                }
            }

            if (blockImageImage == null)
            {
                try
                {
                    blockImage = GameObject.Find("BlockImage");
                    blockImageImage = blockImage.GetComponent<Image>();
                    blockImageImage.raycastTarget = false;
                }
                catch (NullReferenceException)
                {
                }
            }

            if (Input.GetAxisRaw("Vertical") == 1 && !blockImageImage.raycastTarget && !cameraDelay)
            {
                Camera.main.transform.position += Vector3.up;
                StartCoroutine(CameraMoveDelay());
            }

            if (Input.GetAxisRaw("Vertical") == -1 && !blockImageImage.raycastTarget && !cameraDelay)
            {
                Camera.main.transform.position += Vector3.down;
                StartCoroutine(CameraMoveDelay());
            }

            if (Input.GetAxisRaw("Horizontal") == 1 && !blockImageImage.raycastTarget && !cameraDelay)
            {
                Camera.main.transform.position += Vector3.right;
                StartCoroutine(CameraMoveDelay());
            }

            if (Input.GetAxisRaw("Horizontal") == -1 && !blockImageImage.raycastTarget && !cameraDelay)
            {
                Camera.main.transform.position += Vector3.left;
                StartCoroutine(CameraMoveDelay());
            }

            if (Camera.main.orthographicSize > 1 && Input.GetAxisRaw("Mouse ScrollWheel") > 0 &&
                !blockImageImage.raycastTarget)
            {
                ApplySettings();
                Camera.main.orthographicSize--;
            }

            if (Camera.main.orthographicSize < 20 && Input.GetAxisRaw("Mouse ScrollWheel") < 0 &&
                !blockImageImage.raycastTarget)
            {
                ApplySettings();
                Camera.main.orthographicSize++;
            }

            if (!Input.GetMouseButton(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                tap = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && SceneManager.GetActiveScene().buildIndex > 0 &&
            SceneManager.GetActiveScene().buildIndex < 4 && playMode)
        {
            RestartLevelFromCp();
        }

        if (Input.GetMouseButtonDown(1) && SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (GameObject.FindGameObjectWithTag("BlockSettings") != null)
            {
                ApplySettings();
            }
            else
            {            
            mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if (mouseHitPlane.Raycast(mousePositionRay, out dist))
            {
                mousePosition = mousePositionRay.GetPoint(dist);
            }

            var hitCollider = Physics2D.OverlapCircle(mousePosition, 0.01f);
            if (hitCollider != null && hitCollider.GetComponent<SelectSprite>() != null)
            {
                bool isEmpty = true;
                hitColliderProprieties = hitCollider.GetComponent<SelectSprite>();
                blockSettings = Instantiate(BlockSettings, mousePosition, Quaternion.identity);
                blockSettings.transform.SetParent(GameObject.Find("MainCanvas").transform, false);
                blockSettings.transform.position = new Vector3(mousePosition.x, mousePosition.y, 5);
                buttonProprietiesListContent = blockSettings.transform.GetChild(0).GetChild(0).gameObject;

                if (hitColliderProprieties.limitedStep != 0)
                {
                    buttonProprietiesListContent.transform.GetChild(0).gameObject.SetActive(true);
                    var stepLimitIF = GameObject.Find("StepLimitIF").GetComponent<InputField>();
                    stepLimitIF.placeholder.GetComponent<Text>().text = hitColliderProprieties.limitedStep.ToString();
                    isEmpty = false;
                }

                if (hitColliderProprieties.movesLimit != -1)
                {
                    buttonProprietiesListContent.transform.GetChild(1).gameObject.SetActive(true);
                    var movesLimitIF = GameObject.Find("MovesLimitIF").GetComponent<InputField>();
                    movesLimitIF.placeholder.GetComponent<Text>().text = hitColliderProprieties.movesLimit.ToString();
                    isEmpty = false;
                }

                if (hitColliderProprieties.randomType != "")
                {
                    buttonProprietiesListContent.transform.GetChild(2).gameObject.SetActive(true);
                    var randomTypeButton = GameObject.Find("RandomTypeButton");
                    randomTypeButton.transform.GetChild(0).GetComponent<Text>().text =
                        hitColliderProprieties.randomType;
                    isEmpty = false;
                }

                if (isEmpty)
                {
                    Destroy(blockSettings);
                }
            }
            }
        }
    }

    IEnumerator CameraMoveDelay()
    {
        cameraDelay = true;
        yield return new WaitForSeconds(0.05f);
        cameraDelay = false;
    }

    private void OnGUI()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            mousePosition1 = Input.mousePosition;
            isPointerOverGameObject = false;
        }

        if (!isPointerOverGameObject && Input.GetMouseButton(0))
        {
            if (changeToolToggle != null && changeToolToggle.isOn && !movingSelection)
            {
                var rect = Swipe.GetScreenRect(mousePosition1, Input.mousePosition);
                Swipe.DrawScreenRect(rect, new Color(0.3f, 0.29f, 0.29f, 0.25f));
                Swipe.DrawScreenRectBorder(rect, 2, new Color(0.3f, 0.29f, 0.29f, 0.95f));
            }
        }
    }

    private void ApplySettings()
    {
        if (buttonProprietiesListContent != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("BlockSettings"));

            if (GameObject.Find("StepLimitIF") != null)
            {
                var stepLimitIF = GameObject.Find("StepLimitIF").GetComponent<InputField>();
                if (stepLimitIF.text != "" && stepLimitIF.text != "0" && stepLimitIF.text != "-")
                {
                    hitColliderProprieties.limitedStep = Int32.Parse(stepLimitIF.text);
                }
            }

            if (GameObject.Find("MovesLimitIF") != null)
            {
                var movesLimitIF = GameObject.Find("MovesLimitIF").GetComponent<InputField>();
                if (movesLimitIF.text != "" && Int32.Parse(movesLimitIF.text) > -1 && movesLimitIF.text != "-")
                {
                    hitColliderProprieties.movesLimit = Int32.Parse(movesLimitIF.text);
                }
            }
            
            if (GameObject.Find("RandomType") != null)
            {
                var randomType = GameObject.Find("RandomTypeButton").transform.GetChild(0).GetComponent<Text>();
                hitColliderProprieties.randomType = randomType.text;
            }

            hitColliderProprieties.ApplyProprieties();
        }
    }

    private void CreateGrid()
    {
        var gridTiles = GameObject.FindGameObjectsWithTag("grid");
        foreach (var gridTile in gridTiles)
        {
            Destroy(gridTile);
        }
        Vector2 startGridPosition = new Vector2(-48.5f, -48.5f);
        for (int x = 0; x < 100; x += 12)
        {
            for (int y = 0; y < 100; y += 12)
            {
                GameObject gridObject = Instantiate(GridSprite, startGridPosition, Quaternion.identity);
                var gridObjectPosition = gridObject.transform.position;
                gridObject.transform.position = new Vector2(gridObjectPosition.x + x,
                    gridObjectPosition.y + y);
            }
        }
    }

    private void PlayMode()
    {
        grid = GameObject.FindGameObjectsWithTag("grid");
        foreach (var i in grid)
        {
            i.GetComponent<SpriteRenderer>().enabled = false;
        }
        
            changeToolToggle.GetComponentInChildren<Image>().sprite = selectionSprite;
            var levelEdgeA = new Vector2(-levelRows / 2 + 1, -levelColumns / 2 + 1);
            var levelEdgeB = new Vector2(levelRows / 2 + 1, levelColumns / 2 + 1);
            var allSprites = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
            foreach (var i in allSprites)
            {
                if (i.GetComponent<SelectSprite>() != null && i.GetComponent<SelectSprite>().selected)
                {
                    if (!i.GetComponent<SelectSprite>().hidden)
                    {
                        i.GetComponent<SelectSprite>().Deselect();
                        i.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }

            changeToolToggle.isOn = false;

            selectedColliders = null;
        
        Swipe.delay = true;
        GameObject.Find("EditorHandler").GetComponent<Swipe>().Initiate();
        GameOver = false;
        playButtonRenderer.sprite = pauseSprite;
        backButton.enabled = false;
        settingsButton.enabled = false;
        newLevelButton.enabled = false;
        gridToggle.enabled = false;
        gridToggle.GetComponent<Toggle>().isOn = false;
        blockToggle.enabled = false;
        helpButton.enabled = false;
        saveButton.enabled = false;
        publishButton.enabled = false;
        changeToolToggle.enabled = false;
        changeLayerToggle.enabled = false;
        changeToolAnimator.Play("InversePopupAnimation");
        changeLayerAnimator.Play("InversePopupAnimation");
        animatorToggle.Play("GoleftAnimation");
        settingsButtonAnimator.Play("GoleftAnimation");
        animatorBack.Play("GorightAnimation");
        newLevelButtonAnimator.Play("GorightAnimation");
        gridToggleAnimator.Play("InversePopupAnimation");
        helpButtonAnimator.Play("GoleftbackAnimation");
        loadButtonAnimator.Play("PopupAnimation");
        saveButtonAnimator.Play("PopupAnimation");
        publishButtonAnimator.Play("PopupAnimation");
        if (blockToggle.isOn)
        {
            animatorSelector = GameObject.FindGameObjectWithTag("blockSelector").GetComponent<Animator>();
            animatorSelector.Play("PopdownAnimation");
            blockToggle.isOn = false;
        }

        GameObject.Find("CheckpointButton").GetComponent<Button>().enabled = true;
        SaveLevel();
        ClearEditor();
        LoadLevel(objectSavedLevel);
        Camera.main.transform.position = new Vector3(0.5f, 0.5f, 0);
    }

    public void EditorMode()
    {
        GameOver = true;
        EditorInitialize();
        if (playerController != null && playerController.isKeyListUp)
        {
            keyHolderAnimator.Play("PopdownAnimation");
        }

        Transform keyInventory = GameObject.Find("KeyListContent").transform;
        foreach (Transform child in keyInventory.transform)
        {
            Destroy(child.gameObject);
        }

        playButtonRenderer.sprite = playSprite;
        backButton.enabled = true;
        gridToggle.enabled = true;
        newLevelButton.enabled = true;
        blockToggle.enabled = true;
        helpButton.enabled = true;
        saveButton.enabled = true;
        publishButton.enabled = true;
        settingsButton.enabled = true;
        changeToolToggle.enabled = true;
        changeLayerToggle.enabled = true;
        gridToggleAnimator.Play("InversePopdownAnimation");
        changeToolAnimator.Play("InversePopdownAnimation");
        changeLayerAnimator.Play("InversePopdownAnimation");
        animatorToggle.Play("GorightbackAnimation");
        settingsButtonAnimator.Play("GorightbackAnimation");
        animatorBack.Play("GoleftbackAnimation");
        newLevelButtonAnimator.Play("GoleftbackAnimation");
        helpButtonAnimator.Play("GorightAnimation");
        loadButtonAnimator.Play("PopdownAnimation");
        saveButtonAnimator.Play("PopdownAnimation");
        publishButtonAnimator.Play("PopdownAnimation");
        if (GameObject.FindGameObjectWithTag("playerSpawn") != null)
        {
            var checkPointButton = GameObject.Find("CheckpointButton");
            checkPointButton.GetComponent<Animator>().Play("CheckpointPopupAnimation");
            checkPointButton.GetComponent<Button>().enabled = false;
            Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
        }

        ClearEditor();
        LoadLevelInEditor(objectSavedLevel);
    }

    public void LoadRandomLevel()
    {
        DatabaseHandler.GetLevel(LevelHandler.GetRandomOnlineLevel(), level =>
        {
            onlineLevelId = level.id;
            playingOnlineLevel = true;
            LoadOnlineLevelInLevelScene(level);
        });
    }

    private string SaveLevel1()
    {
        var level = new string[levelRows, levelColumns];

        for (var i = 0; i < levelColumns; i++)
        {
            for (var j = 0; j < levelRows; j++)
            {
                var circlePos = new Vector2(-levelRows / 2 + j + 1, -levelColumns / 2 + i + 1);
                if (Physics2D.OverlapCircle(circlePos, 0.3f) == null)
                {
                    level[j, i] = "0";
                }
                else
                {
                    var blockHit = Physics2D.OverlapCircle(circlePos, 0.3f);
                    level[j, i] = blockHit.tag;
                }
            }
        }

        var levelString = new string(string.Join(",", level.Cast<string>().ToArray()).ToCharArray());
        return levelString;
    }

    public void ClearEditor(bool sparePlayer = false)
    {
        var levelEdgeA = new Vector2(-levelColumns / 2-5, -levelRows / 2-5);
        var levelEdgeB = new Vector2(levelColumns / 2+5, levelRows / 2+5);
        var gameobjectsToClear = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
        foreach (var i in gameobjectsToClear)
        {
            if (!i.gameObject.CompareTag("LevelWall") && (!sparePlayer || !i.gameObject.CompareTag("Player") && i.gameObject.name != "rightCol" &&
                i.gameObject.name != "leftCol" && i.gameObject.name != "topCol" && i.gameObject.name != "botCol" &&
                i.gameObject.name != "RedLockSprite" && i.gameObject.name != "LockSprite" &&
                i.gameObject.name != "RedKeySprite" && i.gameObject.name != "KeySprite" &&
                i.gameObject.name != "GreenLockSprite" && i.gameObject.name != "BlueLockSprite" &&
                i.gameObject.name != "GreenKeySprite" && i.gameObject.name != "BlueKeySprite"))
            {
                Destroy(i.gameObject);
            }
        }
    }

    public void LoadLevel1(string level, bool sparePlayer = false)
    {
        var levelStrings = level.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < levelColumns; i++)
        {
            for (var j = 0; j < levelRows; j++)
            {
                int k;
                if (j + i * levelRows < levelRows * levelColumns &&
                    Int32.TryParse(levelStrings[i + j * levelRows], out k))
                {
                    if (k != 0)
                    {
                        if (!sparePlayer || !blocks[k].CompareTag("Player"))
                        {
                            var blockPosition = new Vector2(-levelRows / 2 + j + 1, -levelColumns / 2 + i + 1);
                            Instantiate(blocks[k], blockPosition, Quaternion.identity);
                        }
                    }
                }
            }
        }

        playMode = true;
        EditorMusic.Stop();
        musicOn = false;
        Camera.main.GetComponent<CameraController>().Initialize();
    }

    public void LoadLevel(Level level = null, bool sparePlayer = false)
    {
        if (level == null)
        {
            level = objectSavedLevel;
        }
        LoadLevelGeneric(blocks, level, false, sparePlayer);
        playMode = true;
        EditorMusic.Stop();
        musicOn = false;
        Camera.main.GetComponent<CameraController>().Initialize();
    }

    public void LoadOnlineLevelInEditor()
    {
        EditorScene();
        loadOnlineLevel = true;
    }

    public void LoadLevelInEditor(Level level = null, bool sparePlayer = false, bool onlineLevel = false)
    {
        EditorInitialize();
        if (level == null)
        {
            level = objectSavedLevel;
        }
        levelRowsText.text = level.levelRows.ToString();
        levelColumnsText.text = level.levelColumns.ToString();
        ChangeSize(true);
        LoadLevelGeneric(editorBlocks, level, true, sparePlayer);
        playMode = false;
        yesSound.Play();
        loadOnlineLevel = false;
        if (onlineLevel)
        {
            publishButtonAnimator.GetComponent<Button>().interactable = false;
            saveButtonAnimator.GetComponent<Button>().interactable = false;
            loadButtonAnimator.GetComponent<Button>().interactable = false;
            newLevelButtonAnimator.GetComponent<Button>().interactable = false;
            helpButtonAnimator.GetComponent<Button>().interactable = false;
            Destroy(publishButtonAnimator.GetComponent<Image>());
            Destroy(saveButtonAnimator.gameObject.GetComponent<Image>());
            Destroy(loadButtonAnimator.gameObject.GetComponent<Image>());
            Destroy(newLevelButtonAnimator.gameObject.GetComponent<Image>());
            Destroy(helpButtonAnimator.gameObject.GetComponent<Image>());
        }
    }

    private void LoadLevelGeneric(GameObject[] spawnableBlocks, Level level, bool editorLoad, bool sparePlayer = false)
    {
        for (var i = 0; i < level.level.Length; i++)
        {
            levelRows = level.levelRows;
            levelColumns = level.levelColumns;
            
            var x = level.level[i].position % levelColumns;
            var y = (int) Math.Truncate((double) (level.level[i].position / levelColumns));
            Vector2 spawnPosition = new Vector2(-levelColumns / 2 + x + 1, levelRows / 2 - y);

            foreach (var block in level.level[i].blocks)
            {
                if (block != null && (!sparePlayer || block.id != 1))
                {
                    var newBlock = Instantiate(spawnableBlocks[block.id], spawnPosition, Quaternion.identity);

                    if (!editorLoad)
                    {
                        // Player
                        if (block.id == 1)
                        {
                            newBlock.GetComponent<PlayerController>().movesLimit = block.movesLimit;
                        }

                        // Limited Block
                        if (block.id == 25)
                        {
                            newBlock.GetComponent<LimitedBlockController>().limit = block.limitedStep;
                            newBlock.GetComponent<SpriteRenderer>().sprite = LimitedSprites[block.limitedStep-1];
                        }
                        
                        // Random Block
                        if (block.id == 34)
                        {
                            newBlock.GetComponent<RandomSpriteSpawn>().randomType = block.randomType;
                        }
                    }
                    else
                    {
                        var newBlockPosition = newBlock.transform.position;
                        newBlock.transform.position = new Vector3(newBlockPosition.x, newBlockPosition.y, 515);

                        var newBlockSelectSprite = newBlock.GetComponent<SelectSprite>();
                        var newBlockSpriteRenderer = newBlock.GetComponent<SpriteRenderer>();

                        if (block.hidden)
                        {
                            newBlockSpriteRenderer.color = hiddenLayerColor;
                            newBlockSpriteRenderer.sortingOrder -= 5;
                            newBlockSelectSprite.hidden = true;
                        }
                        
                        // Player
                        if (block.id == 1)
                        {
                            newBlockSelectSprite.movesLimit = block.movesLimit;
                        }

                        // Limited Block
                        if (block.id == 25)
                        {
                            newBlockSelectSprite.limitedStep = block.limitedStep;
                            newBlockSpriteRenderer.sprite = LimitedSprites[block.limitedStep-1];
                        }
                        
                        if (block.id == 34)
                        {
                            newBlockSelectSprite.randomType = block.randomType;
                        }
                    }
                }
            }
        }
    }

    public void SaveLevel()
    {
        Level level = new Level();
        objectSavedLevel = level;
    }

    private void LoadLevelInEditor1(string level)
    {
        try
        {
            var levelStrings = level.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < 96; i++)
            {
                for (var j = 0; j < 96; j++)
                {
                    int k;
                    if (j + i * 96 < 96 * 96 &&
                        Int32.TryParse(levelStrings[i + j * 96], out k))
                    {
                        if (k != 0)
                        {
                            var blockPosition = new Vector2(-96 / 2 + j, -96 / 2 + i);
                            Instantiate(editorBlocks[k], blockPosition, Quaternion.identity);
                        }
                    }
                }
            }

            playMode = false;
            yesSound.Play();
        }
        catch
        {
            levelCodeHint.text = "Invalid level code";
            noSound.Play();
        }
    }

    public void RestartLevel()
    {
        Swipe.delay = true;
        GameObject.Find("EditorHandler").GetComponent<Swipe>().Initiate();
        if (GameObject.FindGameObjectWithTag("playerSpawn") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevelFromCp()
    {
        if (playingOnlineLevel && !onChallenge)
        {
            DatabaseHandler.RestartLevel(onlineLevelId);
        }
        if (inEditor)
        {
            GameOver = true;
            if (GameObject.FindGameObjectWithTag("playerSpawn"))
            {
                Swipe.delay = true;
                GameObject.Find("EditorHandler").GetComponent<Swipe>().Initiate();
                ClearEditor(true);
                LoadLevel(objectSavedLevel, true);
                playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                playerController.IsBackToCheckpoint();
            }
            else
            {
                playButtonToggle.isOn = false;
                EditorMode();
            }
        }
        else
        {
            if (!playingChallenge)
            {
                Swipe.delay = true;
                GameObject.Find("EditorHandler").GetComponent<Swipe>().Initiate();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                if (GameObject.FindGameObjectWithTag("playerSpawn"))
                {
                    isBackToCheckpoint = true;
                }
            }
            else
            {
                LeaveEditorScene();
            }
        }
    }

    public void EditorScene()
    {
        SceneManager.LoadScene(1);
        menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        menuMusic.Stop();
        playMode = false;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(GameObject.FindGameObjectWithTag("EditorHandler"));
    }

    public void LeaveEditorScene()
    {
        if (PublishMode)
        {
            EditorHandler.PublishDailyLevel = false;
            EditorInitialized = false;
            PublishMode = false;
            playMode = false;
            reloadLevel = true;
            SceneManager.LoadScene(1);
        } else if (playingOnlineLevel && !onChallenge)
        {
            OnlineLevelsRoom();
        }
        else
        {
            onChallenge = false;
            SceneManager.LoadScene(0);
            Destroy(GameObject.FindGameObjectWithTag("EditorHandler"));
        }

        Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
        if (GameObject.FindGameObjectWithTag("playerSpawn") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
        }
    }

    private void EditorFirstTime()
    {
        isnotFirstTimeEditor = true;
        EditorGuide();
        SaveData();
    }

    public void EditorGuide()
    {
        EditorInitialize();
        tutorialText.text = editorTutorialText[0];
        tutorialProgress = 0;
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void CreateNewLevel()
    {
        ClearEditor();
        newLevelWindow.GetComponent<Animator>().Play("TopCanvasUp");
        yesSound.Play();
        LevelSizeWindow();
    }

    public void GuideCancel()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        helpWindow.GetComponent<Animator>().Play("TopCanvasUp");
        blockToggle.transform.SetAsFirstSibling();
        playButtonToggle.transform.SetAsFirstSibling();
        saveButton.transform.SetAsFirstSibling();
        loadButtonAnimator.transform.SetAsFirstSibling();
        changeToolToggle.transform.SetAsFirstSibling();
        gridToggle.transform.SetAsFirstSibling();
        saveButton.enabled = true;
        publishButton.enabled = true;
        loadButtonAnimator.GetComponent<Button>().enabled = true;
        helpButton.enabled = true;
        noSound.Play();
    }

    public void GuideProgress()
    {
        EditorInitialize();
        tutorialProgress++;
        if (tutorialProgress < 8)
        {
            tutorialText.text = editorTutorialText[tutorialProgress];
            yesSound.Play();
        }
        else
        {
            GuideCancel();
        }

        if (tutorialProgress == 1)
        {
            blockToggle.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 2)
        {
            blockToggle.transform.SetAsFirstSibling();
            playButtonToggle.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 3)
        {
            playButtonToggle.transform.SetAsFirstSibling();
            saveButton.enabled = false;
            saveButton.transform.SetAsLastSibling();
            loadButtonAnimator.GetComponent<Button>().enabled = false;
            loadButtonAnimator.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 4)
        {
            saveButton.transform.SetAsFirstSibling();
            saveButton.enabled = true;
            loadButtonAnimator.transform.SetAsFirstSibling();
            loadButtonAnimator.GetComponent<Button>().enabled = true;
            publishButton.enabled = false;
            publishButton.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 5)
        {
            publishButton.transform.SetAsFirstSibling();
            publishButton.enabled = true;
            changeToolToggle.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 6)
        {
            changeToolToggle.transform.SetAsFirstSibling();
            gridToggle.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 7)
        {
            gridToggle.transform.SetAsFirstSibling();
            helpButton.enabled = false;
            helpButton.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 8)
        {
            helpButton.transform.SetAsFirstSibling();
            helpButton.enabled = true;
        }
    }

    public void GridToggle()
    {
        EditorInitialize();
        if (gridToggle.GetComponent<Toggle>().isOn)
        {
            ShowGrid();
        }
        else
        {
            HideGrid();
        }
    }

    private void ShowGrid()
    {
        grid = GameObject.FindGameObjectsWithTag("grid");
        foreach (var i in grid)
        {
            i.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void HideGrid()
    {
        grid = GameObject.FindGameObjectsWithTag("grid");
        foreach (var i in grid)
        {
            i.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void PlayOrStop()
    {
        EditorInitialize();
        if (playButtonToggle.isOn)
        {
            PlayMode();
        }
        else
        {
            EditorMode();
        }
    }

    public void SaveLevelOnDisk()
    {
        Level levelData = new Level();
        string levelJson = FromLevelToJson(levelData);
        string levelName = levelText.GetComponent<Text>().text;

        string destination = Application.persistentDataPath + "/" + levelName + ".lk";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, levelJson);
        file.Close();

        BackToMainCanvas();
    }

    public void LoadLevelFromDisk()
    {
        String levelName = levelText.GetComponent<Text>().text;
        string destination = Application.persistentDataPath + "/" + levelName + ".lk";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            noSound.Play();
            levelTextHint.GetComponent<Text>().text = "Level not found";
            inputField.text = "";
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        string levelJson = (string) bf.Deserialize(file);
        file.Close();

        objectSavedLevel = FromJsonToLevel(levelJson);

        ClearEditor();
        LoadLevelInEditor(objectSavedLevel);
        BackToMainCanvas();
    }

    public static Level FromJsonToLevel(string levelJson)
    {
        fsData data = fsJsonParser.Parse(levelJson);
        Level level = null;
        DatabaseHandler.serializer.TryDeserialize(data, ref level);
        return level;
    }

    public static string FromLevelToJson(Level level)
    {
        fsData levelDataSerialized;
        DatabaseHandler.serializer.TrySerialize(level, out levelDataSerialized).AssertSuccessWithoutWarnings();
        return fsJsonPrinter.CompressedJson(levelDataSerialized);
    }


    private void LevelWindow()
    {
        inputField.text = "";
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        saveLevelWindow.GetComponent<Animator>().Play("CanvasUp");
    }

    public void SaveLevelWindow()
    {
        EditorInitialize();
        LevelWindow();
        loadLevel.GetComponent<Image>().enabled = false;
        saveLevel.GetComponent<Image>().enabled = true;
    }

    public void LoadLevelWindow()
    {
        EditorInitialize();
        LevelWindow();
        loadLevel.GetComponent<Image>().enabled = true;
        saveLevel.GetComponent<Image>().enabled = false;
        levelTextHint.GetComponent<Text>().text = "Level name";
    }
    
    public void PublishLevelWindow()
    {
        EditorInitialize();
        levelName.text = "";
        levelNameHint.text = "Level name";
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        publishLevelWindow.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void QuitLevelWindow()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        quitLevelWindow.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void NewLevelWindow()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        newLevelWindow.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void LevelSizeWindow()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        levelSizeWindow.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void ChangeSizeOnClick()
    {
        ChangeSize();
    }

    public void ChangeSize(bool hidden = false, int ilevelRows = 0, int ilevelColumns = 0)
    {
        if (ilevelRows == 0 && ilevelColumns == 0)
        {
            if (Int32.Parse(levelColumnsText.text) < 1 || Int32.Parse(levelColumnsText.text) > 100 ||
                Int32.Parse(levelRowsText.text) < 1 || Int32.Parse(levelRowsText.text) > 100)
            {
                if (Int32.Parse(levelRowsText.text) < 1 || Int32.Parse(levelRowsText.text) > 100)
                {
                    levelRowsHint.text = "Must be a number from 1 to 100";
                    levelRowsText.text = "";
                }

                if (Int32.Parse(levelColumnsText.text) < 1 || Int32.Parse(levelColumnsText.text) > 100)
                {
                    levelColumnsHint.text = "Must be a number from 1 to 100";
                    levelColumnsText.text = "";
                }

                noSound.Play();
                return;
            }

            levelRows = Int32.Parse(levelRowsText.text);
            levelColumns = Int32.Parse(levelColumnsText.text);
        }
        else
        {
            levelRows = ilevelRows;
            levelColumns = ilevelColumns;
        }

        var levelWalls = GameObject.FindGameObjectsWithTag("LevelWall");
        foreach (var levelWall in levelWalls)
        {
            Destroy(levelWall);
        }

        var edgeA = new Vector2(-levelColumns / 2 - 0, levelRows / 2 + 1);
        var edgeB = new Vector2(levelColumns / 2 + 2, levelRows / 2 + 1);
        var edgeC = new Vector2(levelColumns / 2 + 2, -levelRows / 2 - 1);
        var edgeD = new Vector2(-levelColumns / 2 - 0, -levelRows / 2 - 1);

        LevelWallBorder(edgeA, edgeB, edgeC, edgeD);
        if (!hidden)
        {
            yesSound.Play();
            BackFromLevelSizeCanvas();
        }
        CreateGrid();
    }

    private void LevelWallBorder(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
        var instantiatePosition = pointA;
        if (levelRows % 2 == 0)
        {
            pointC += Vector2.up;
            pointD += Vector2.up;
        }

        if (levelColumns % 2 == 0)
        {
            pointB += Vector2.left;
            pointC += Vector2.left;
        }

        while (instantiatePosition != pointB)
        {
            instantiatePosition += Vector2.right;
            Instantiate(LevelWall, instantiatePosition, Quaternion.identity);
        }

        while (instantiatePosition != pointC)
        {
            instantiatePosition += Vector2.down;
            Instantiate(LevelWall, instantiatePosition, Quaternion.identity);
        }

        while (instantiatePosition != pointD)
        {
            instantiatePosition += Vector2.left;
            Instantiate(LevelWall, instantiatePosition, Quaternion.identity);
        }

        while (instantiatePosition != pointA)
        {
            instantiatePosition += Vector2.up;
            Instantiate(LevelWall, instantiatePosition, Quaternion.identity);
        }
    }

    private void HelpWindow2()
    {
        isnotFirstSelectTime = true;
        SaveData();
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow2.GetComponent<Animator>().Play("TopCanvasDown");
    }

    private void HelpWindow3()
    {
        isnotFirstPlaceTime = true;
        SaveData();
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow3.GetComponent<Animator>().Play("TopCanvasDown");
    }
    
    private void HelpWindow4()
    {
        isnotFirstSokobanPlaceTime = true;
        SaveData();
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow4.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void HelpWindow5()
    {
        isnotFirstChangeLayerTime = true;
        SaveData();
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow5.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void BackToMainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        saveLevelWindow.GetComponent<Animator>().Play("CanvasDown");
    }

    public void BackFromQuitMainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        quitLevelWindow.GetComponent<Animator>().Play("TopCanvasUp");
    }

    public void BackFromPublishMainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        publishLevelWindow.GetComponent<Animator>().Play("TopCanvasUp");
    }

    public void BackFromNewMainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        newLevelWindow.GetComponent<Animator>().Play("TopCanvasUp");
    }

    public void BackFromLevelSizeCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        levelSizeWindow.GetComponent<Animator>().Play("TopCanvasUp");
    }

    public void BackFromHelpWindow2MainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        helpWindow2.GetComponent<Animator>().Play("TopCanvasUp");
    }

    public void BackFromHelpWindow3MainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        helpWindow3.GetComponent<Animator>().Play("TopCanvasUp");
    }
    
    public void BackFromHelpWindow4MainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        helpWindow4.GetComponent<Animator>().Play("TopCanvasUp");
    }
    
    public void BackFromHelpWindow5MainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        helpWindow5.GetComponent<Animator>().Play("TopCanvasUp");
    }


    public void CopyLevelToClipboard()
    {
        EditorInitialize();
        TextEditor te = new TextEditor();
        SaveLevel();
        if (objectSavedLevel.id != "")
        {
            te.text = objectSavedLevel.id;
            te.SelectAll();
            te.Copy();
            levelCodeHint.text = "Level code copied to clipboard";
            yesSound.Play();
        }
        else
        {
            levelCodeHint.text = "You need to publish the level first";
            noSound.Play();
        }
    }

    public void TutorialLevel()
    {
        SceneManager.LoadScene(2);
        menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        menuMusic.Stop();
    }

    public void OnlineLevels()
    {
        if (AuthHandler.userId != null){
            SceneManager.LoadScene(4);
        }
        else
        {
            AuthWindow();
        }
    }

    public void OnlineLevelsRoom()
    {
        SceneManager.LoadScene(4);
        playMode = true;
        EditorMusic.Stop();
        musicOn = false;
        menuMusic.Play();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadNormalLevelInLevelScene(int levelNumber = 0)
    {
        objectSavedLevel = FromJsonToLevel(Resources.Load<TextAsset>("Levels/level" + levelNumber).text);
        currentLevelNumber = levelNumber;
        SceneManager.LoadScene(3);
        menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        menuMusic.Stop();
    }

    public void LoadCurrentDailyChallengeLevel()
    {
        placeSound = GameObject.Find("PlaceSound").GetComponent<AudioSource>();
        noSound = GameObject.Find("NoSound").GetComponent<AudioSource>();
        yesSound = GameObject.Find("YesSound").GetComponent<AudioSource>();
        GameObject.Find("PlayDailyChallenge").GetComponent<Button>().interactable = false;
        DatabaseHandler.GetDailyChallengeNumber(day =>
        {
            string challengeDay = day.Substring(1, day.Length - 2);
            DatabaseHandler.CheckAttemptChallenge(challengeDay, canAttempt =>
            {
                if (canAttempt)
                {
                    DatabaseHandler.PostDailyChallengeStreak("0");
                    DatabaseHandler.AttemptChallenge(challengeDay);
                    LoadDailyChallengeLevelInLevelScene(challengeDay);
                }
                else
                {
                    AlreadyAttemptedDailyChallenge();
                }
            });
        });
        
    }

    private void AlreadyAttemptedDailyChallenge()
    {
        GameObject.Find("DailyChallengeExplanationText").GetComponent<Text>().text = alreadyAttemptedDailyChallengeText;
        noSound.Play();
    }

    public void LoadDailyChallengeLevelInLevelScene(string challengeNumber, bool isPlayingChallenge = true)
    {
        playingChallenge = isPlayingChallenge;
        playingOnlineLevel = true;
        onChallenge = true;
        currentChallengeNumber = challengeNumber;
        DatabaseHandler.GetDailyChallengeLevel(challengeNumber, LoadOnlineLevelInLevelScene);
    }

    public void RestartNormalLevelInLevelScene()
    {
        if (playingOnlineLevel && !onChallenge)
        {
            DatabaseHandler.RestartLevel(onlineLevelId);
        } else if (onChallenge)
        {
            LoadDailyChallengeLevelInLevelScene(currentChallengeNumber, false);
        }
        Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
        if (!PublishMode && !playingOnlineLevel)
        {
            objectSavedLevel = FromJsonToLevel(Resources.Load<TextAsset>("Levels/level" + currentLevelNumber).text);
        }
        SceneManager.LoadScene(3);
    }

    public void LoadOnlineLevelInLevelScene(Level level)
    {
        objectSavedLevel = level;
        menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        menuMusic.Stop();
        if (playingChallenge)
        {
            SceneManager.LoadScene(5);
        }
        else
        {
            SceneManager.LoadScene(3);
        }
    }

    public void ShowLevels()
    {
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
        mainCanvas.GetComponent<Canvas>().sortingOrder = 0;
        levelCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
        levelCanvas.GetComponent<Canvas>().sortingOrder = 1;
    }

    public void HideLevels()
    {
        var mainCanvas = GameObject.Find("MainCanvas");
        var levelCanvas = GameObject.Find("LevelCanvas");
        mainCanvas.GetComponent<Animator>().Play("CanvasOpacity100");
        mainCanvas.GetComponent<Canvas>().sortingOrder = 1;
        levelCanvas.GetComponent<Animator>().Play("CanvasOpacity0");
        levelCanvas.GetComponent<Canvas>().sortingOrder = 0;
    }


    public static void SaveData()
    {
        SaveToFile(beatenLevelNumber, isnotFirstTimeEditor, isnotFirstSelectTime, isnotFirstPlaceTime, isnotFirstSokobanPlaceTime, isnotFirstChangeLayerTime);
    }

    public static void SaveToFile(int unlockedLevelNumber, bool firstTimeEditor, bool firstTimeSelectBool,
        bool firstTimePlaceBool, bool firstTimeSokobanPlaceBool, bool firstTimeChangeLayerBool)
    {
        string destination = Application.persistentDataPath + "/save.lk";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(unlockedLevelNumber, firstTimeEditor, firstTimeSelectBool, firstTimePlaceBool, firstTimeSokobanPlaceBool, firstTimeChangeLayerBool);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public static void LoadToFile()
    {
        string destination = Application.persistentDataPath + "/save.lk";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData) bf.Deserialize(file);
        file.Close();

        beatenLevelNumber = data.unlockedLevelsInt;
        isnotFirstTimeEditor = data.firstTimeEditorBool;
        isnotFirstSelectTime = data.firstTimeSelectBool;
        isnotFirstPlaceTime = data.firstTimePlaceBool;
        isnotFirstSokobanPlaceTime = data.firstTimeSokobanPlaceBool;
        isnotFirstChangeLayerTime = data.firstTimeChangeLayerBool;
    }

    public void ClearData()
    {
        SaveToFile(0, false, false, false, false, false);
        LoadToFile();
        BackFromClearDataWindow();
    }

    public void ChangeLayer()
    {
        var levelEdgeA = new Vector2(-levelColumns / 2-5, -levelRows / 2-5);
        var levelEdgeB = new Vector2(levelColumns / 2+5, levelRows / 2+5);
        var gameobjectsToClear = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
        foreach (var i in gameobjectsToClear)
        {
            var selectSprite = i.GetComponent<SelectSprite>();
            var spriteRenderer = i.GetComponent<SpriteRenderer>();
            if (selectSprite != null)
            {
                if (!selectSprite.hidden)
                {
                    spriteRenderer.color = hiddenLayerColor;
                    spriteRenderer.sortingOrder -= 5;
                    selectSprite.hidden = true;
                    selectSprite.selected = false;
                    
                }
                else
                {
                    spriteRenderer.color = normalColor;
                    spriteRenderer.sortingOrder += 5;
                    selectSprite.hidden = false;
                }
            }
        }
        
    }

    public void ChangeTool()
    {
        EditorInitialize();
        if (!changeToolToggle.isOn)
        {
            changeToolToggle.GetComponentInChildren<Image>().sprite = selectionSprite;
            var levelEdgeA = new Vector2(-levelRows / 2 + 1, -levelColumns / 2 + 1);
            var levelEdgeB = new Vector2(levelRows / 2 + 1, levelColumns / 2 + 1);
            var allSprites = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
            foreach (var i in allSprites)
            {
                if (i.GetComponent<SelectSprite>() != null && i.GetComponent<SelectSprite>().selected && !i.GetComponent<SelectSprite>().hidden)
                {
                    i.GetComponent<SelectSprite>().Deselect();
                    i.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }

            selectedColliders = null;
        }
        else
        {
            changeToolToggle.GetComponentInChildren<Image>().sprite = mouseSprite;
        }
    }
   

    public void VerifyLevel()
    {
        EditorInitialize();
        verifyLevel.enabled = false;
        backFromPublish.enabled = false;
        if (isOnline)
        {
            if (AuthHandler.userId != null)
            {
                RestClient.Get<User>(DatabaseHandler.DatabaseURL + "users/" + AuthHandler.userId + ".json?auth=" +
                                     AuthHandler.idToken).Then(user =>
                {
                    levelNameString = levelName.text;
                    levelAuthorString = user.userName;
                    objectSavedLevel = new Level();
                    PublishMode = true;
                    SaveLevel();
                    SceneManager.LoadScene(3);
                });
            }
            else
            {
                levelName.text = "";
                levelNameHint.text = "You are not signed in!";
                noSound.Play();
                verifyLevel.enabled = true;
                backFromPublish.enabled = true;
            }
        }
        else
        {
            levelName.text = "";
            levelNameHint.text = "You are in offline-mode!";
            noSound.Play();
            verifyLevel.enabled = true;
            backFromPublish.enabled = true;
        }
    }

    public void VerifyDailyLevel()
    {
        PublishDailyLevel = true;
        VerifyLevel();
    }
    
    static int dictionary = 1 << 23; // 1 << 23;
    static bool eos = false;

    static SevenZip.CoderPropID[] propIDs =
    {
        SevenZip.CoderPropID.DictionarySize,
        SevenZip.CoderPropID.PosStateBits,
        SevenZip.CoderPropID.LitContextBits,
        SevenZip.CoderPropID.LitPosBits,
        SevenZip.CoderPropID.Algorithm,
        SevenZip.CoderPropID.NumFastBytes,
        SevenZip.CoderPropID.MatchFinder,
        SevenZip.CoderPropID.EndMarker
    };

    static object[] properties =
    {
        dictionary,
        (Int32) (2), /* PosStateBits 2 */
        (Int32) (3), /* LitContextBits 3 */
        (Int32) (0), /* LitPosBits 0 */
        (Int32) (2), /*Algorithm  2 */
        (Int32) (128), /* NumFastBytes 128 */
        "bt4", /* MatchFinder "bt4" */
        eos /* endMarker  eos */
    };

    private byte[] Compress(string inputString)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        using (MemoryStream inStream = new MemoryStream(inputBytes))
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
                encoder.SetCoderProperties(propIDs, properties);
                encoder.WriteCoderProperties(outStream);
                long fileSize = inStream.Length;
                for (int i = 0; i < 8; i++)
                    outStream.WriteByte((Byte) (fileSize >> (8 * i)));
                encoder.Code(inStream, outStream, -1, -1, null);
                return outStream.ToArray();
            }
        }
    }

    private string Decompress(byte[] inputBytes)
    {
        try
        {
            using (MemoryStream newInStream = new MemoryStream(inputBytes))
            {
                SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
                newInStream.Seek(0, 0);
                using (MemoryStream newOutStream = new MemoryStream())
                {
                    byte[] properties2 = new byte[5];
                    if (newInStream.Read(properties2, 0, 5) != 5)
                        throw (new Exception("input .lzma is too short"));
                    long outSize = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        int v = newInStream.ReadByte();
                        if (v < 0)
                            throw (new Exception("Can't Read 1"));
                        outSize |= ((long) (byte) v) << (8 * i);
                    }

                    decoder.SetDecoderProperties(properties2);
                    long compressedSize = newInStream.Length - newInStream.Position;
                    decoder.Code(newInStream, newOutStream, compressedSize, outSize, null);
                    return Encoding.UTF8.GetString(newOutStream.ToArray());
                }
            }
        }
        catch
        {
            return null;
        }
    }
}