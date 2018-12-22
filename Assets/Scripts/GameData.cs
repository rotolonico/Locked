[System.Serializable]
public class GameData
{
	public int unlockedLevelsInt;
	public bool firstTimeEditorBool;
	public bool firstTimeSelectBool;
 
	public GameData(int unlockedLevels, bool firstTimeEditor, bool firstTimeSelect)
	{
		unlockedLevelsInt = unlockedLevels;
		firstTimeEditorBool = firstTimeEditor;
		firstTimeSelectBool = firstTimeSelect;
	}
}
