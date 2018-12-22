using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class EditorHandler : MonoBehaviour
{
    public GameObject selectedObject;
    public GameObject GridSprite;
    public AudioSource EditorMusic;

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

    private int levelRows = 96;
    private int levelColumns = 96;
    readonly float mouseThreshold = 0.2f;

    public static string savedLevel;

    public GameObject[] blocks = new GameObject[34];
    public GameObject[] randomSpawnableBlocks = new GameObject[34];
    public GameObject[] editorBlocks = new GameObject[34];

    public Button[] levelButtons = new Button[10];
    public String[] levels = new String[10];

    public static bool playMode;
    public static bool PublishMode;
    public static bool inEditor;
    public static bool GameOver;
    public static bool isBackToCheckpoint;

    private AudioSource placeSound;
    private AudioSource noSound;
    private AudioSource yesSound;
    private bool EditorInitialized;
    private Toggle playButtonToggle;
    private Animator loadButtonAnimator;
    private Animator saveButtonAnimator;
    private Animator publishButtonAnimator;
    private Button saveButton;
    private Button publishButton;
    private Image playButtonRenderer;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Sprite lockedLevelSprite;
    public Sprite mouseSprite;
    public Sprite selectionSprite;

    private GameObject blockImage;
    private GameObject saveLevelWindow;
    private GameObject shareLevelWindow;
    private GameObject helpWindow;
    private GameObject helpWindow2;
    private GameObject quitLevelWindow;
    private GameObject newLevelWindow;
    private GameObject publishLevel;
    private GameObject saveLevel;
    private GameObject loadLevel;
    private GameObject levelText;
    private GameObject levelTextHint;
    private Animator shareButtonAnimator;
    private Animator helpButtonAnimator;
    private Animator quitButtonAnimator;
    private Button quitButton;
    private Button shareButton;
    private Button newLevelButton;
    private Toggle gridToggle;
    private Animator newLevelButtonAnimator;
    private Animator gridToggleAnimator;
    private Button helpButton;
    private Text levelCodeHint;
    private InputField shareInputField;
    private InputField inputField;
    private Animator keyHolderAnimator;
    private Toggle changeToolToggle;
    private Animator changeToolAnimator;
    private Image blockImageImage;
    private PlayerController playerController;
    public Random Random = new Random();
    
    public static int beatenLevelNumber;
    public static bool isNotFirstTimeEditor;
    public static bool isFirstSelectTime = true;
    public static int currentLevelNumber;
    public static int maxLevelNumber = 9;
    
    private Vector3 mousePosition;
    private Vector3 mouseEndPosition;
    private RectTransform selectionBox;
    private Vector2 startMousePosition = Vector2.zero;
    private Collider2D[] checkSpriteCollision;
    private bool movingSelection;
    private bool cameraDelay;
    private Vector3 newMousePosition;
    private Vector3 mousePosition1;
    private int tutorialProgress;
    private Text tutorialText;
    private String[] editorTutorialText = new String[8];
    bool isPointerOverGameObject = true;
    private bool hittingSelection;
    private bool hittingGUI;
    private GameObject[] grid;
    

    public bool tap = true;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadToFile();
        mouseHitPlane = new Plane(Vector3.forward, transform.position);
        playMode = true;
        GameOver = false;
        LevelButtonsInitialize();
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
        saveButton = GameObject.FindGameObjectWithTag("saveButton").GetComponent<Button>();
        helpButton = GameObject.Find("HelpButton").GetComponent<Button>();
        helpButtonAnimator = GameObject.Find("HelpButton").GetComponent<Animator>();
        newLevelButton = GameObject.Find("NewLevelButton").GetComponent<Button>();
        newLevelButtonAnimator = GameObject.Find("NewLevelButton").GetComponent<Animator>();
        gridToggle = GameObject.Find("GridToggle").GetComponent<Toggle>();
        gridToggleAnimator = GameObject.Find("GridToggle").GetComponent<Animator>();
        //publishButton = GameObject.FindGameObjectWithTag("publishButton").GetComponent<Button>();
        loadButtonAnimator = GameObject.FindGameObjectWithTag("loadButton").GetComponent<Animator>();
        saveButtonAnimator = GameObject.FindGameObjectWithTag("saveButton").GetComponent<Animator>();
        //publishButtonAnimator = GameObject.FindGameObjectWithTag("publishButton").GetComponent<Animator>();
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
        quitLevelWindow = GameObject.Find("QuitLevelWindow");
        newLevelWindow = GameObject.Find("NewLevelWindow");
        publishLevel = GameObject.Find("PublishLevel");
        saveLevel = GameObject.Find("SaveLevel");
        loadLevel = GameObject.Find("LoadLevel");
        levelText = GameObject.Find("LevelName");
        levelTextHint = GameObject.Find("LevelNameHint");
        shareButtonAnimator = GameObject.Find("ShareButton").GetComponent<Animator>();
        shareButton = GameObject.Find("ShareButton").GetComponent<Button>();
        quitButtonAnimator = GameObject.Find("ShareButton").GetComponent<Animator>();
        quitButton = GameObject.Find("ShareButton").GetComponent<Button>();
        levelCodeHint = GameObject.Find("LevelCodeHint").GetComponent<Text>();
        shareInputField = GameObject.Find("ShareInputField").GetComponent<InputField>();
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
        keyHolderAnimator = GameObject.FindGameObjectWithTag("keyInventory").GetComponent<Animator>();
        changeToolToggle = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Toggle>();
        changeToolAnimator = GameObject.FindGameObjectWithTag("changeTool").GetComponent<Animator>();
        editorTutorialText[0] = "Welcome to the game\'s editor!\nHere is where the magic happens\nDo you want to learn how it works?";
        editorTutorialText[1] = "Click the \"block button\" to get a list of all available blocks\nClick anywhere on the screen to place the block you selected";
        editorTutorialText[2] = "Click the \"play button\" to test your awesome level";
        editorTutorialText[3] = "The \"save button\" will make you able to save the level you are\ncreating\nGive the save a name and then use the \"load button\" to load it back in";
        editorTutorialText[4] = "Are you ready to share your level with the world?\nClick on the \"share button\" and the level code will be copied\ninto your clipboard\nYou can send the code to a friend or load in a level from a friend by clicking on the \"load button\" in the share level window";
        editorTutorialText[5] = "As of right now there are two different tools you can use to \nmake your levels:\n- The draw tool, that allows you to place blocks in your level\n- The selection tool, that allows you to select a group of blocks, move, clone (ctrl+c) or delete them (canc)\nUse the \"change tool button\" to switch between the tools";
        editorTutorialText[6] = "Finally, you can click on the \"grid button\" to show/hide an useful grid that will indicate the rooms' borders";
        editorTutorialText[7] = "You can access this tutorial at any time by clicking on the \"help button\"";
        
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        shareInputField.characterValidation = InputField.CharacterValidation.EmailAddress;
        inputField.characterValidation = InputField.CharacterValidation.EmailAddress;
    }


    void Update()
    {
        if (!EditorInitialized && SceneManager.GetActiveScene().buildIndex == 1)
        {
            CreateGrid();
            if (!isNotFirstTimeEditor)
            {
                EditorFirstTime();
            }
            inEditor = true;
            EditorInitialize();
        }
        else if (inEditor && SceneManager.GetActiveScene().buildIndex != 1)
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
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            hittingGUI = false;
                            foreach (var i in hitColliders)
                            {
                                if (i.gameObject.GetComponent<SelectSprite>().selected)
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
                }

            if (Input.GetMouseButton(0))
            {
                if (movingSelection)
                {
                    var levelEdgeA = new Vector2(-levelRows / 2, -levelColumns / 2);
                    var levelEdgeB = new Vector2(levelRows / 2, levelColumns / 2);
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
                                if (i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.down;
                                }
                            }

                            mousePosition.y -= 1;
                        }

                        if (mousePosition.y < newMousePosition.y)
                        {
                            foreach (var i in allSprites)
                            {
                                if (i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.up;
                                }
                            }
                            mousePosition.y += 1;
                        }

                        if (mousePosition.x > newMousePosition.x)
                        {
                            foreach (var i in allSprites)
                            {
                                if (i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.left;
                                }
                            }
                            mousePosition.x -= 1;
                        }

                        if (mousePosition.x < newMousePosition.x)
                        {
                            foreach (var i in allSprites)
                            {
                                if (i.GetComponent<SelectSprite>().selected)
                                {
                                    i.transform.position += Vector3.right;
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
                        if (i != null)
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
                        if (i != null)
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
                                if (i != null)
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
                            i.GetComponent<SelectSprite>().Select();
                            i.GetComponent<SpriteRenderer>().color = Color.grey;
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

            if (Camera.main.orthographicSize > 1 && Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize--;
            }

            if (Camera.main.orthographicSize < 20 && Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize++;
            }

            if (!Input.GetMouseButton(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                tap = true;
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

    private void CreateGrid()
    {
        Vector2 startGridPosition = new Vector2(-(levelColumns / 2) + 5.5f, -(levelRows / 2) + 5.5f);
        for (int x = 0; x < levelColumns; x += 12) {
            for (int y = 0; y < levelRows; y += 12) {
                GameObject gridObject = Instantiate(GridSprite, startGridPosition, Quaternion.identity);
                gridObject.transform.position = new Vector2(gridObject.transform.position.x+x, gridObject.transform.position.y+y);
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
        newLevelButton.enabled = false;
        gridToggle.enabled = false;
        gridToggle.GetComponent<Toggle>().isOn = false;
        blockToggle.enabled = false;
        helpButton.enabled = false;
        saveButton.enabled = false;
        //publishButton.enabled = false;
        shareButton.enabled = false;
        changeToolToggle.enabled = false;
        changeToolAnimator.Play("InversePopupAnimation");
        shareButtonAnimator.Play("GorightAnimation");
        animatorToggle.Play("GoleftAnimation");
        animatorBack.Play("GorightAnimation");
        newLevelButtonAnimator.Play("GorightAnimation");
        gridToggleAnimator.Play("InversePopupAnimation");
        helpButtonAnimator.Play("GoleftbackAnimation");
        loadButtonAnimator.Play("PopupAnimation");
        saveButtonAnimator.Play("PopupAnimation");
        //publishButtonAnimator.Play("PopupAnimation");
        if (blockToggle.isOn)
        {
            animatorSelector = GameObject.FindGameObjectWithTag("blockSelector").GetComponent<Animator>();
            animatorSelector.Play("PopdownAnimation");
            blockToggle.isOn = false;
        }

        GameObject.Find("CheckpointButton").GetComponent<Button>().enabled = true;
        savedLevel = SaveLevel();
        ClearEditor();
        LoadLevel(savedLevel);
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
        //publishButton.enabled = true;
        shareButton.enabled = true;
        changeToolToggle.enabled = true;
        gridToggleAnimator.Play("InversePopdownAnimation");
        changeToolAnimator.Play("InversePopdownAnimation");
        shareButtonAnimator.Play("GoleftbackAnimation");
        animatorToggle.Play("GorightbackAnimation");
        animatorBack.Play("GoleftbackAnimation");
        newLevelButtonAnimator.Play("GoleftbackAnimation");
        helpButtonAnimator.Play("GorightAnimation");
        loadButtonAnimator.Play("PopdownAnimation");
        saveButtonAnimator.Play("PopdownAnimation");
        //publishButtonAnimator.Play("PopdownAnimation");
        if (GameObject.FindGameObjectWithTag("playerSpawn") != null)
        {
            var checkPointButton = GameObject.Find("CheckpointButton");
            checkPointButton.GetComponent<Animator>().Play("CheckpointPopupAnimation");
            checkPointButton.GetComponent<Button>().enabled = false;
            Destroy(GameObject.FindGameObjectWithTag("playerSpawn"));
        }

        ClearEditor();
        LoadLevelInEditor(savedLevel);
    }

    private string SaveLevel()
    {
        var level = new string[levelRows, levelColumns];

        for (var i = 0; i < levelColumns; i++)
        {
            for (var j = 0; j < levelRows; j++)
            {
                var circlePos = new Vector2(-levelRows / 2 + j, -levelColumns / 2 + i);
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
        var levelEdgeA = new Vector2(-levelRows / 2, -levelColumns / 2);
        var levelEdgeB = new Vector2(levelRows / 2, levelColumns / 2);
        var gameobjectsToClear = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
        foreach (var i in gameobjectsToClear)
        {
            if (!sparePlayer || !i.gameObject.CompareTag("Player") && i.gameObject.name != "rightCol" &&
                i.gameObject.name != "leftCol" && i.gameObject.name != "topCol" && i.gameObject.name != "botCol" &&
                i.gameObject.name != "RedLockSprite" && i.gameObject.name != "LockSprite" &&
                i.gameObject.name != "RedKeySprite" && i.gameObject.name != "KeySprite" &&
                i.gameObject.name != "GreenLockSprite" && i.gameObject.name != "BlueLockSprite" &&
                i.gameObject.name != "GreenKeySprite" && i.gameObject.name != "BlueKeySprite")
            {
                Destroy(i.gameObject);
            }
        }
    }

    public void LoadLevel(string level, bool sparePlayer = false)
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
                            var blockPosition = new Vector2(-levelRows / 2 + j, -levelColumns / 2 + i);
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

    private void LoadLevelInEditor(string level)
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
                            var blockPosition = new Vector2(-levelRows / 2 + j, -levelColumns / 2 + i);
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
                LoadLevel(savedLevel, true);
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
        playMode = false;
    }

    public void LeaveEditorScene()
    {
        SceneManager.LoadScene(0);
        Destroy(GameObject.FindGameObjectWithTag("EditorHandler"));
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
        BackFromNewMainCanvas();
        yesSound.Play();
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
        EditorInitialize();
        yesSound.Play();
        String levelName = levelText.GetComponent<Text>().text;
        var level = SaveLevel();
        PlayerPrefs.SetString("level-" + levelName, Convert.ToBase64String(Compress(level)));
        BackToMainCanvas();
    }

    public void LoadLevelFromDisk()
    {
        EditorInitialize();
        String levelName = levelText.GetComponent<Text>().text;
        var level = PlayerPrefs.GetString("level-" + levelName, "");
        if (level != "")
        {
            ClearEditor();
            LoadLevelInEditor(Decompress(Convert.FromBase64String(level)));
            BackToMainCanvas();
        }
        else
        {
            noSound.Play();
            levelTextHint.GetComponent<Text>().text = "Level not found";
            inputField.text = "";
        }
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
        publishLevel.GetComponent<Image>().enabled = false;
    }

    public void LoadLevelWindow()
    {
        EditorInitialize();
        LevelWindow();
        loadLevel.GetComponent<Image>().enabled = true;
        saveLevel.GetComponent<Image>().enabled = false;
        publishLevel.GetComponent<Image>().enabled = false;
        levelTextHint.GetComponent<Text>().text = "Level name";
    }

    public void PublishLevelWindow()
    {
        EditorInitialize();
        LevelWindow();
        loadLevel.GetComponent<Image>().enabled = false;
        saveLevel.GetComponent<Image>().enabled = false;
        publishLevel.GetComponent<Image>().enabled = true;
    }

    public void ShareLevelWindow()
    {
        shareInputField.text = "";
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        shareLevelWindow.GetComponent<Animator>().Play("TopCanvasDown");
        levelCodeHint.text = "Level code";
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
    
    private void HelpWindow2()
    {
        isFirstSelectTime = false;
        SaveData();
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = true;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
        helpWindow2.GetComponent<Animator>().Play("TopCanvasDown");
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
    
    public void BackFromNewMainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        newLevelWindow.GetComponent<Animator>().Play("TopCanvasUp");
    }
    
    public void BackFromHelpWindow2MainCanvas()
    {
        EditorInitialize();
        blockImage.GetComponent<Image>().raycastTarget = false;
        blockImage.GetComponent<Animator>().Play("BlockImageTransparentReverse");
        helpWindow2.GetComponent<Animator>().Play("TopCanvasUp");
    }


    public void CopyLevelToClipboard()
    {
        EditorInitialize();
        TextEditor te = new TextEditor();
        te.text = Convert.ToBase64String(Compress(SaveLevel()));
        te.SelectAll();
        te.Copy();
        levelCodeHint.text = "Level code copied to clipboard";
        yesSound.Play();
    }

    public void LoadLevelFromShare()
    {
        EditorInitialize();
        shareInputField.characterLimit = 10000;
        ClearEditor();
        levelCodeHint.text = "Level successfully loaded";
        try
        {
            LoadLevelInEditor(Decompress(Convert.FromBase64String(shareInputField.text)));
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
    }

    public void OnlineLevels()
    {
        SceneManager.LoadScene(3);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadNormalLevelInLevelScene(int levelNumber = 0)
    {
        savedLevel = Decompress(Convert.FromBase64String(levels[levelNumber]));
        currentLevelNumber = levelNumber;
        SceneManager.LoadScene(3);
    }

    public void RestartNormalLevelInLevelScene()
    {
        savedLevel = Decompress(Convert.FromBase64String(levels[currentLevelNumber]));
        SceneManager.LoadScene(3);
    }

    public void LoadOnlineLevelInLevelScene(string level)
    {
        savedLevel = Decompress(Convert.FromBase64String(level));
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
        SaveToFile(beatenLevelNumber, isNotFirstTimeEditor, isFirstSelectTime);
    }

    public static void SaveToFile(int unlockedLevelNumber, bool firstTimeEditor, bool firstTimeSelectBool)
    {
        string destination = Application.persistentDataPath + "/save.lk";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(unlockedLevelNumber, firstTimeEditor, firstTimeSelectBool);
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
    }

    public void ClearData()
    {
        SaveToFile(0, false, true);
        LoadToFile();
        LevelButtonsInitialize();
    }

    public void ChangeTool()
    {
        EditorInitialize();
        if (!changeToolToggle.isOn)
        {
            changeToolToggle.GetComponentInChildren<Image>().sprite = selectionSprite;
            var levelEdgeA = new Vector2(-levelRows / 2, -levelColumns / 2);
            var levelEdgeB = new Vector2(levelRows / 2, levelColumns / 2);
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