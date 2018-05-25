using UnityEngine;
using System.Collections;

//============================================
using DG.Tweening;

public class Player
{
	public string displayName;
	public int currentScore;
	public int currentID; // 1 or 2
	public Team team;
	public GameObject ring;
	public GameObject santa;
	public string currentRow;

	//Contructors
	public Player(){
		Team _team = Team.Cola;
		string team = PlayerPrefs.GetString ("Team", "Cola");
		if (team == "Cola")
			_team = Team.Cola;
		if (team == "Pepsi")
			_team = Team.Pepsi;
		this.team = _team;
	}

	public Player(Team team, string displayName){
		this.team = team;
		this.displayName = displayName;
		currentScore = 0;
	}
}

//============================================

public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Instance {
		get;
		private set;
	}

	void Awake () {
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	//==============================================

	[Header ("Current Player Info")]
	public Team currentPlayer;
	public bool isBot;
	public bool isEnemy;

	[Space (10)]
	public Player player;
	public Player enemy;

	[Header ("Ring")]
	public GameObject objRingRed;
	public GameObject objRingBlue;

	public float ringPosY = 1f;
	public float ringPosZ = 12f;

	[HideInInspector]
	public float animationTime = 0f;

	public void SetPlayerAttributs(Player player){
	
		switch (player.team) {
		case Team.Cola:
			player.ring = objRingRed;
			break;
		case Team.Pepsi:
			player.ring = objRingBlue;
			break;
		}
	}

	public void SetPlayerPosition(Player player,string row, bool animate){
		
		player.currentRow = row;

		switch (player.team) {
		case Team.Cola:
			
			objRingRed.transform.position = new Vector3 (GameManager.Instance.rows [row], ringPosY, -ringPosZ);
			animationTime = 0f;

			break;
		case Team.Pepsi:
			
			objRingBlue.transform.position = new Vector3 (GameManager.Instance.rows [row], ringPosY, ringPosZ);
			animationTime = 0f;

			break;
		}
	}

	public void SwitchPlayer(Player player, bool showRing = true, bool showSanta = false, float waitTime = 0f){
		if (waitTime == 0f) {
			player.ring.GetComponent<MeshRenderer> ().enabled = showRing;
		}
		else
			StartCoroutine (SwitchPlayerCoroutine(player, showRing, showSanta, waitTime));
	}

	IEnumerator SwitchPlayerCoroutine(Player player, bool showRing, bool showSanta, float waitTime){
		yield return new WaitForSeconds (waitTime);

		player.ring.GetComponent<MeshRenderer> ().enabled = showRing;
	}

	public void SetPlayersScore(Score score){
		player.currentScore = player.team == Team.Cola ? score.score_cola : score.score_pepsi;
		enemy.currentScore = enemy.team == Team.Cola ? score.score_cola : score.score_pepsi;
	}
}