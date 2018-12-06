using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditorHandler : MonoBehaviour {
	
	public GameObject selectedObject;
	public AudioSource EditorMusic;
	
	private Plane mouseHitPlane;
	private Ray mousePositionRay;
	private Ray touchPositionRay;
	private Collider2D[] hitColliders;
	private Animator animatorSelector;
	private Animator animatorToggle;
	private Animator animatorBack;
	private Toggle blockToggle;
	private Button backButton;
	private bool musicOn;
	
	private int levelRows = 96;
	private int levelColumns = 96;	

	private static string savedLevel;
	
	public GameObject[] blocks = new GameObject[31];
	public GameObject[] editorBlocks = new GameObject[31];

	public static bool playMode;
	public static bool PublishMode;

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

	private GameObject blockImage;
	private GameObject saveLevelWindow;
	private GameObject shareLevelWindow;
	private GameObject publishLevel;
	private GameObject saveLevel;
	private GameObject loadLevel;
	private GameObject levelText;
	private GameObject levelTextHint;
	private Animator shareButtonAnimator;
	private Button shareButton;
	private Text levelCodeHint;
	private InputField shareInputField;
	private InputField inputField;

	public bool tap = true;
	
	void Start()
	{
		DontDestroyOnLoad(gameObject);
		mouseHitPlane = new Plane(Vector3.forward, transform.position);
		playMode = true;
	}
	
	private void EditorInitialize()
	{
		placeSound = GameObject.Find("PlaceSound").GetComponent<AudioSource>();
		noSound = GameObject.Find("NoSound").GetComponent<AudioSource>();
		yesSound = GameObject.Find("YesSound").GetComponent<AudioSource>();
		EditorInitialized = true;
		saveButton = GameObject.FindGameObjectWithTag("saveButton").GetComponent<Button>();
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
		saveLevelWindow = GameObject.Find("SaveLevelWindow");
		shareLevelWindow = GameObject.Find("ShareLevelWindow");
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
	}
	
	
	void Update () {

		if (SceneManager.GetActiveScene().buildIndex == 1 && !EditorInitialized)
		{
			EditorInitialize();
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

			if (Input.GetMouseButtonUp(0) && tap)
			{
				mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				float dist;
				if (mouseHitPlane.Raycast(mousePositionRay, out dist))
				{
					Vector3 mousePosition = mousePositionRay.GetPoint(dist);
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

			if (Swipe.SwipeUp && !EventSystem.current.IsPointerOverGameObject())
			{
				Camera.main.transform.position += Vector3.up;
				tap = false;
			}
			if (Swipe.SwipeDown && !EventSystem.current.IsPointerOverGameObject())
			{
				Camera.main.transform.position += Vector3.down;
				tap = false;
			}
			if (Swipe.SwipeRight && !EventSystem.current.IsPointerOverGameObject())
			{
				Camera.main.transform.position += Vector3.right;
				tap = false;
			}
			if (Swipe.SwipeLeft && !EventSystem.current.IsPointerOverGameObject())
			{
				Camera.main.transform.position += Vector3.left;
				tap = false;
			}
			
			if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				tap = true;
			}
		}
	}


	private void PlayMode()
	{
		playButtonRenderer.sprite = pauseSprite;
		backButton.enabled = false;
		blockToggle.enabled = false;
		saveButton.enabled = false;
		publishButton.enabled = false;
		shareButton.enabled = false;
		shareButtonAnimator.Play("GorightAnimation");
		animatorToggle.Play("GoleftAnimation");
		animatorBack.Play("GorightAnimation");
		loadButtonAnimator.Play("PopupAnimation");
		saveButtonAnimator.Play("PopupAnimation");
		publishButtonAnimator.Play("PopupAnimation");
		if (blockToggle.isOn)
		{
			animatorSelector = GameObject.FindGameObjectWithTag("blockSelector").GetComponent<Animator>();
			animatorSelector.Play("PopdownAnimation");
			blockToggle.isOn = false;
		}

		savedLevel = SaveLevel();
		ClearEditor();
		LoadLevel(savedLevel);
		Camera.main.transform.position = new Vector3(0.5f,0.5f,0);
	}
	
	public void EditorMode()
	{
		EditorInitialize();
		Transform keyInventory = GameObject.Find("KeyInventory").transform;
		foreach (Transform child in keyInventory.transform) {
			Destroy(child.gameObject);
		}
		playButtonRenderer.sprite = playSprite;
		backButton.enabled = true;
		blockToggle.enabled = true;
		saveButton.enabled = true;
		publishButton.enabled = true;
		shareButton.enabled = true;
		shareButtonAnimator.Play("GoleftbackAnimation");
		animatorToggle.Play("GorightbackAnimation");
		animatorBack.Play("GoleftbackAnimation");
		loadButtonAnimator.Play("PopdownAnimation");
		saveButtonAnimator.Play("PopdownAnimation");
		publishButtonAnimator.Play("PopdownAnimation");
		ClearEditor();
		LoadLevelInEditor(savedLevel);
	}

	private string SaveLevel()
	{
		var level = new string[levelRows,levelColumns];
		
		for (var i = 0; i < levelColumns; i++)
		{
			for (var j = 0; j < levelRows; j++)
			{
				var circlePos = new Vector2(-levelRows/2 + j,-levelColumns/2 + i);
				if (Physics2D.OverlapCircle(circlePos, 0.3f) == null)
				{
					level[j,i] = "0";
				}
				else
				{
					var blockHit = Physics2D.OverlapCircle(circlePos, 0.3f);
					level[j,i] = blockHit.tag;
				}	
			}
		}

		var levelString = new string(string.Join(",",level.Cast<string>().ToArray()).ToCharArray());
		
		return levelString; 
	}

	private void ClearEditor()
	{
		var levelEdgeA = new Vector2(-levelRows/2, -levelColumns/2);
		var levelEdgeB = new Vector2(levelRows/2, levelColumns/2);
		var gameobjectsToClear = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
		foreach (var i in gameobjectsToClear)
		{
			Destroy(i.gameObject);
		}
	}

	private void LoadLevel(string level)
	{
		var levelStrings = level.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
	    
		for (var i = 0; i < levelColumns; i++)
		{
			for (var j = 0; j < levelRows; j++)
			{
				int k;
				if (j + i * levelRows < levelRows*levelColumns && Int32.TryParse(levelStrings[i + j * levelRows], out k))
				{
					if (k != 0)
					{
						var blockPosition = new Vector2(-levelRows / 2 + j, -levelColumns / 2 + i);
						Instantiate(blocks[k], blockPosition, Quaternion.identity);
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
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
		PlayerPrefs.SetString("level-" + levelName,level);
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
			LoadLevelInEditor(level);
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
		EditorInitialize();
		blockImage.GetComponent<Image>().raycastTarget = true;
		blockImage.GetComponent<Animator>().Play("BlockImageTransparent");
		shareLevelWindow.GetComponent<Animator>().Play("TopCanvasDown");
		levelCodeHint.text = "Level code";
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
{dictionary,
    (Int32)(2), /* PosStateBits 2 */
    (Int32)(3), /* LitContextBits 3 */
    (Int32)(0), /* LitPosBits 0 */
    (Int32)(2), /*Algorithm  2 */
    (Int32)(128), /* NumFastBytes 128 */
    "bt4", /* MatchFinder "bt4" */
    eos   /* endMarker  eos */
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
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));
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
