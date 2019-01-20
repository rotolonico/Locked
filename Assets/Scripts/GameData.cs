[System.Serializable]
public class GameData
{
	public int unlockedLevelsInt;
	public bool firstTimeEditorBool;
	public bool firstTimeSelectBool;
	public bool firstTimePlaceBool;
	public bool firstTimeSokobanPlaceBool;
 
	public GameData(int unlockedLevels, bool firstTimeEditor, bool firstTimeSelect, bool firstTimePlace, bool firstTimeSokobanPlace)
	{
		unlockedLevelsInt = unlockedLevels;
		firstTimeEditorBool = firstTimeEditor;
		firstTimeSelectBool = firstTimeSelect;
		firstTimePlaceBool = firstTimePlace;
		firstTimeSokobanPlaceBool = firstTimeSokobanPlace;
	}
}
