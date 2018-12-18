[System.Serializable]
public class GameData
{
	public int unlockedLevelsInt;
	public bool firstTimeEditorBool;
 
	public GameData(int unlockedLevels, bool firstTimeEditor)
	{
		unlockedLevelsInt = unlockedLevels;
		firstTimeEditorBool = firstTimeEditor;
	}
}
