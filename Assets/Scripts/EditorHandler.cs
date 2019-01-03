using System;
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

public class EditorHandler : MonoBehaviour
{
    public GameObject selectedObject;
    public GameObject GridSprite;
    public AudioSource EditorMusic;
    private static AudioSource menuMusic;

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

    public static int levelRows = 30;
    public static int levelColumns = 30;
    readonly float mouseThreshold = 0.2f;

    public static Level objectSavedLevel;

    public GameObject[] blocks = new GameObject[34];
    public GameObject[] randomSpawnableBlocks = new GameObject[34];
    public GameObject[] editorBlocks = new GameObject[34];

    public Button[] levelButtons = new Button[10];
    private string[] levels;

    public static bool playMode;
    public static bool PublishMode;
    public static bool inEditor;
    public static bool GameOver;
    public static bool isBackToCheckpoint;

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
    private GameObject shareLevelWindow;
    private GameObject helpWindow;
    private GameObject helpWindow2;
    private GameObject helpWindow3;
    private GameObject quitLevelWindow;
    private GameObject publishLevelWindow;
    private GameObject newLevelWindow;
    private GameObject publishLevel;
    private GameObject levelSizeWindow;
    private GameObject saveLevel;
    private GameObject loadLevel;
    private GameObject levelText;
    private GameObject levelTextHint;
    private Animator shareButtonAnimator;
    private Animator helpButtonAnimator;
    private Button shareButton;
    private Button newLevelButton;
    private Toggle gridToggle;
    private Animator newLevelButtonAnimator;
    private Animator gridToggleAnimator;
    private Button helpButton;
    private Button settingsButton;
    private Animator settingsButtonAnimator;
    private Text levelCodeHint;
    private InputField shareInputField;
    private InputField inputField;
    private Animator keyHolderAnimator;
    private Toggle changeToolToggle;
    private Animator changeToolAnimator;
    private Image blockImageImage;
    private PlayerController playerController;
    public GameObject BlockSettings;
    public static Random Random = new Random();

    public static int beatenLevelNumber;
    public static bool isNotFirstTimeEditor;
    public static bool isFirstSelectTime = true;
    public static bool isFirstPlaceTime = true;
    public static int currentLevelNumber;
    public static int maxLevelNumber = 14;

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
    private InputField password;
    private Animator authWindowAnimator;
    private Text levelNameHint;
    private Text levelAuthorHint;
    private Text levelName;
    private Text levelAuthor;
    private Text levelColumnsHint;
    private Text levelRowsHint;
    private InputField levelColumnsText;
    private InputField levelRowsText;
    private GameObject blockSettings;
    private GameObject buttonProprietiesListContent;
    private SelectSprite hitColliderProprieties;

    public static string levelNameString;
    public static string levelAuthorString;

    public bool tap = true;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadToFile();
        mouseHitPlane = new Plane(Vector3.forward, transform.position);
        playMode = true;
        GameOver = false;
        LevelButtonsInitialize();
        menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        menuMusic.Play();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        email = GameObject.Find("Email").GetComponent<InputField>();
        password = GameObject.Find("Email").GetComponent<InputField>();
        authWindowAnimator = GameObject.Find("AuthenticationWindow").GetComponent<Animator>();
    }

    public void SignupUser()
    {
        InitializeButtons();
        AuthHandler.SignupUser(email.text, password.text);
    }

    public void SigninUser()
    {
        InitializeButtons();
        AuthHandler.SigninUser(email.text, password.text);
    }

    public void AuthWindow()
    {
        InitializeButtons();
        authWindowAnimator.Play("CanvasDown");
    }

    public void BackFromAuth()
    {
        InitializeButtons();
        authWindowAnimator.Play("CanvasUp");
    }

    private void LevelButtonsInitialize()
    {
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
        levelName = GameObject.Find("LevelNameText").GetComponent<Text>();
        levelAuthor = GameObject.Find("LevelAuthorText").GetComponent<Text>();
        levelNameHint = GameObject.Find("LevelNameHint").GetComponent<Text>();
        levelAuthorHint = GameObject.Find("LevelAuthorHint").GetComponent<Text>();
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
        shareLevelWindow = GameObject.Find("ShareLevelWindow");
        helpWindow = GameObject.Find("HelpWindow");
        helpWindow2 = GameObject.Find("HelpWindow 2");
        helpWindow3 = GameObject.Find("HelpWindow 3");
        quitLevelWindow = GameObject.Find("QuitLevelWindow");
        newLevelWindow = GameObject.Find("NewLevelWindow");
        levelSizeWindow = GameObject.Find("LevelSizeWindow");
        publishLevel = GameObject.Find("PublishLevel");
        saveLevel = GameObject.Find("SaveLevel");
        loadLevel = GameObject.Find("LoadLevel");
        levelText = GameObject.Find("LevelName");
        levelTextHint = GameObject.Find("LevelNameHint");
        shareButtonAnimator = GameObject.Find("ShareButton").GetComponent<Animator>();
        shareButton = GameObject.Find("ShareButton").GetComponent<Button>();
        levelCodeHint = GameObject.Find("LevelCodeHint").GetComponent<Text>();
        shareInputField = GameObject.Find("ShareInputField").GetComponent<InputField>();
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
        keyHolderAnimator = GameObject.FindGameObjectWithTag("keyInventory").GetComponent<Animator>();
        changeToolToggle = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Toggle>();
        changeToolAnimator = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Animator>();
        publishLevelWindow = GameObject.Find("PublishLevelWindow");
        publishLevelWindowAnimator = GameObject.Find("PublishLevelWindow").GetComponent<Animator>();
        settingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
        settingsButtonAnimator = GameObject.Find("SettingsButton").GetComponent<Animator>();
        editorTutorialText[0] =
            "Welcome to the game\'s editor!\nHere is where the magic happens\nDo you want to learn how it works?";
        editorTutorialText[1] =
            "Click the \"block button\" to get a list of all available blocks\nClick anywhere on the screen to place the block you selected";
        editorTutorialText[2] = "Click the \"play button\" to test your awesome level";
        editorTutorialText[3] =
            "The \"save button\" will make you able to save the level you are\ncreating\nGive the save a name and then use the \"load button\" to load it back in";
        editorTutorialText[4] =
            "Are you ready to share your level with the world?\nClick on the \"share button\" and the level code will be copied\ninto your clipboard\nYou can send the code to a friend or load in a level from a friend by clicking on the \"load button\" in the share level window";
        editorTutorialText[5] =
            "As of right now there are two different tools you can use to \nmake your levels:\n- The draw tool, that allows you to place blocks in your level\n- The selection tool, that allows you to select a group of blocks, move, clone (ctrl+c) or delete them (canc)\nUse the \"change tool button\" to switch between the tools";
        editorTutorialText[6] =
            "Finally, you can click on the \"grid button\" to show/hide an useful grid that will indicate the rooms' borders";
        editorTutorialText[7] = "You can access this tutorial at any time by clicking on the \"help button\"";

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        shareInputField.characterValidation = InputField.CharacterValidation.EmailAddress;
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


    void Update()
    {
        if (!EditorInitialized && SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isNotFirstTimeEditor)
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
                                    if (isFirstPlaceTime)
                                    {
                                        HelpWindow3();
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
                    Vector2 levelEdgeA = new Vector2(-levelRows / 2 - 5, -levelColumns / 2 -5);
                    Vector2 levelEdgeB = new Vector2(levelRows / 2 + 5, levelColumns / 2 + 5);
                    
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
                        if (i != null && i.GetComponent<SelectSprite>() != null)
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
                                if (i != null && i.GetComponent<SelectSprite>() != null)
                                {
                                    i.GetComponent<SelectSprite>().Deselect();
                                    i.GetComponent<SpriteRenderer>().color = Color.white;
                                }
                            }
                        }

                        selectedColliders = Physics2D.OverlapAreaAll(mousePosition, mouseEndPosition);
                        foreach (var i in selectedColliders)
                        {
                            if (isFirstSelectTime)
                            {
                                HelpWindow2();
                            }

                            if (i.GetComponent<SelectSprite>() != null)
                            {
                                i.GetComponent<SelectSprite>().Select();
                                i.GetComponent<SpriteRenderer>().color = Color.grey;
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
                            Destroy(i.gameObject);
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
            ApplySettings();
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
                
                if (isEmpty)
                {
                    Destroy(blockSettings);
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
                if (movesLimitIF.text != "" && movesLimitIF.text != "-1" && movesLimitIF.text != "-")
                {
                    hitColliderProprieties.movesLimit = Int32.Parse(movesLimitIF.text);
                }
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
        shareButton.enabled = false;
        changeToolToggle.enabled = false;
        changeToolAnimator.Play("InversePopupAnimation");
        shareButtonAnimator.Play("GorightAnimation");
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
        shareButton.enabled = true;
        changeToolToggle.enabled = true;
        gridToggleAnimator.Play("InversePopdownAnimation");
        changeToolAnimator.Play("InversePopdownAnimation");
        shareButtonAnimator.Play("GoleftbackAnimation");
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
        DatabaseHandler.GetLevel(LevelHandler.GetRandomOnlineLevel(), level => { LoadOnlineLevelInLevelScene(level); });
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

    public void LoadLevelInEditor(Level level = null, bool sparePlayer = false)
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
    }

    private void LoadLevelGeneric(GameObject[] spawnableBlocks, Level level, bool editorLoad, bool sparePlayer = false)
    {
        for (var i = 0; i < level.level.Length; i++)
        {
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
                    }
                    else
                    {
                        var newBlockPosition = newBlock.transform.position;
                        newBlock.transform.position = new Vector3(newBlockPosition.x, newBlockPosition.y, 515);
                        
                        // Player
                        if (block.id == 1)
                        {
                            newBlock.GetComponent<SelectSprite>().movesLimit = block.movesLimit;
                        }

                        // Limited Block
                        if (block.id == 25)
                        {
                            newBlock.GetComponent<SelectSprite>().limitedStep = block.limitedStep;
                            newBlock.GetComponent<SpriteRenderer>().sprite = LimitedSprites[block.limitedStep-1];
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
                            var blockPosition = new Vector2(-levelRows / 2 + j + 1, -levelColumns / 2 + i + 1);
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
            Swipe.delay = true;
            GameObject.Find("EditorHandler").GetComponent<Swipe>().Initiate();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (GameObject.FindGameObjectWithTag("playerSpawn"))
            {
                isBackToCheckpoint = true;
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

    public void LeaveEditorScene()
    {
        if (PublishMode)
        {
            EditorInitialized = false;
            PublishMode = false;
            playMode = false;
            reloadLevel = true;
            SceneManager.LoadScene(1);
        }
        else
        {
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
        isNotFirstTimeEditor = true;
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
        shareButton.transform.SetAsFirstSibling();
        changeToolToggle.transform.SetAsFirstSibling();
        gridToggle.transform.SetAsFirstSibling();
        saveButton.enabled = true;
        loadButtonAnimator.GetComponent<Button>().enabled = true;
        shareButton.enabled = true;
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
            shareButton.enabled = false;
            shareButton.transform.SetAsLastSibling();
        }

        if (tutorialProgress == 5)
        {
            shareButton.transform.SetAsFirstSibling();
            shareButton.enabled = true;
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

    public void ShareLevelWindow()
    {
        EditorInitialize();
        shareInputField.text = "";
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        shareLevelWindow.GetComponent<Animator>().Play("TopCanvasDown");
        levelCodeHint.text = "Level code";
    }

    public void PublishLevelWindow()
    {
        EditorInitialize();
        levelName.text = "";
        levelAuthor.text = "";
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

    public void ChangeSize(bool hidden = false)
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
        isFirstSelectTime = false;
        SaveData();
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow2.GetComponent<Animator>().Play("TopCanvasDown");
    }

    private void HelpWindow3()
    {
        isFirstPlaceTime = false;
        SaveData();
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow3.GetComponent<Animator>().Play("TopCanvasDown");
    }

    public void BackToMainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        saveLevelWindow.GetComponent<Animator>().Play("CanvasDown");
    }

    public void BackFromShareMainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        shareLevelWindow.GetComponent<Animator>().Play("TopCanvasUp");
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

    public void LoadLevelFromShare()
    {
        EditorInitialize();
        shareInputField.characterLimit = 10000;
        ClearEditor();
        levelCodeHint.text = "Level successfully loaded";
        try
        {
            DatabaseHandler.GetLevel(shareInputField.text, level => { objectSavedLevel = level; });
            LoadLevelInEditor();
            BackFromShareMainCanvas();
        }
        catch
        {
            levelCodeHint.text = "Invalid level code";
            noSound.Play();
        }

        shareInputField.text = "";
    }

    public void TutorialLevel()
    {
        SceneManager.LoadScene(2);
        menuMusic = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        menuMusic.Stop();
    }

    public void OnlineLevels()
    {
        SceneManager.LoadScene(4);
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

    public void RestartNormalLevelInLevelScene()
    {
        Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
        if (!PublishMode)
        {
            objectSavedLevel = FromJsonToLevel(levels[currentLevelNumber]);
        }

        SceneManager.LoadScene(3);
    }

    public void LoadOnlineLevelInLevelScene(Level level)
    {
        objectSavedLevel = level;
        SceneManager.LoadScene(3);
    }

    public void ShowLevels()
    {
        var mainCanvas = GameObject.Find("MainCanvas");
        var levelCanvas = GameObject.Find("LevelCanvas");
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
        SaveToFile(beatenLevelNumber, isNotFirstTimeEditor, isFirstSelectTime, isFirstPlaceTime);
    }

    public static void SaveToFile(int unlockedLevelNumber, bool firstTimeEditor, bool firstTimeSelectBool,
        bool firstTimePlaceBool)
    {
        string destination = Application.persistentDataPath + "/save.lk";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(unlockedLevelNumber, firstTimeEditor, firstTimeSelectBool, firstTimePlaceBool);
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
        isNotFirstTimeEditor = data.firstTimeEditorBool;
        isFirstSelectTime = data.firstTimeSelectBool;
        isFirstPlaceTime = data.firstTimePlaceBool;
    }

    public void ClearData()
    {
        SaveToFile(0, false, true, true);
        LoadToFile();
        LevelButtonsInitialize();
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
                if (i.GetComponent<SelectSprite>() != null && i.GetComponent<SelectSprite>().selected)
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
        levelNameString = levelName.text;
        levelAuthorString = levelAuthor.text;
        objectSavedLevel = new Level();
        PublishMode = true;
        SaveLevel();
        SceneManager.LoadScene(3);
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