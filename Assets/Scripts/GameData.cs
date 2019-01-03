[System.Serializable]
public class GameData
{
	public int unlockedLevelsInt;
	public bool firstTimeEditorBool;
	public bool firstTimeSelectBool;
	public bool firstTimePlaceBool;
 
	public GameData(int unlockedLevels, bool firstTimeEditor, bool firstTimeSelect, bool firstTimePlace)
	{
		unlockedLevelsInt = unlockedLevels;
		firstTimeEditorBool = firstTimeEditor;
		firstTimeSelectBool = firstTimeSelect;
		firstTimePlaceBool = firstTimePlace;
	}
}
