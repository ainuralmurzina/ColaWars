using UnityEngine;
using System.Collections;

public class GlobalScore{
	public string displayName;
	public string score;
	public string team;

	public GlobalScore(string displayName, string score, string team){
		this.displayName = displayName;
		this.score = score;
		this.team = team;
	}
}

