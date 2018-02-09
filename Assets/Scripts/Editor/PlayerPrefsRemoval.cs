using UnityEngine;
using UnityEditor;

public class PlayerPrefsRemoval
{
	[MenuItem ("Tools/Clear PlayerPrefs")]
	private static void NewMenuOption()
	{
		PlayerPrefs.DeleteAll();
	}
}
