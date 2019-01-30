[System.Serializable]
public class GameData
{
	public int unlockedLevelsInt;
	public bool firstTimeEditorBool;
	public bool firstTimeSelectBool;
	public bool firstTimePlaceBool;
	public bool firstTimeSokobanPlaceBool;
	public bool firstTimeChangeLayerBool;
 
	public GameData(int unlockedLevels, bool firstTimeEditor, bool firstTimeSelect, bool firstTimePlace, bool firstTimeSokobanPlace, bool firstTimeChangeLayer)
	{
		unlockedLevelsInt = unlockedLevels;
		firstTimeEditorBool = firstTimeEditor;
		firstTimeSelectBool = firstTimeSelect;
		firstTimePlaceBool = firstTimePlace;
		firstTimeSokobanPlaceBool = firstTimeSokobanPlace;
		firstTimeChangeLayerBool = firstTimeChangeLayer;
	}
}
