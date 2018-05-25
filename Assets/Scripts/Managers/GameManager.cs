using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	#region Authentication
	public static GameManager Instance {
		get;
		private set;
	}

	void Awake(){
		if (Instance != null && Instance != this)
			Destroy (gameObject);
		else {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}

	}
	#endregion

	//Game Mode
	#region Public Variables

	//Game Mode
	public GameMode currentMode {get; private set;}
	public GameSubMode currentSubMode {get; private set;}

	//Game State
	public GameState currentState {get; private set;}

	//Player
	public Player currentPlayer {get; private set;}

	//Online Game
	public bool isOnlineGameActive {get; private set;}

	#endregion

	//PassedShootsCounter
	int numPassedShootsCola = 0;
	int numPassedShootsPepsi = 0;
	//Rows
	public Dictionary<string, float> rows = new Dictionary<string, float> ();
	//Implementation
	private Mode mode;
	//Online Game
	private UnityEvent _updateFieldandScore = new UnityEvent ();
	private bool isRingStopped = false;
	private bool isOnlineScoreRecieved = false;
	private Score online_score;
	private List<bool> online_cola_bottles;
	private List<bool> online_pepsi_bottles;

	void Start(){

		//Initial values
		currentMode = GameMode.None;
		currentSubMode = GameSubMode.None;
		currentState = GameState.None;
		currentPlayer = null;

		//Rows
		rows.Add("most_left", -4);
		rows.Add("left", -2);
		rows.Add("middle", 0);
		rows.Add("right", 2);
		rows.Add("most_right", 4);

		//OnlineGame
		_updateFieldandScore.AddListener(OnlineCountScore);
	}

	public void SetCurrentPlayer(Player player){
		currentPlayer = player;
	}

	public void StartGame(ModeData mode_data){
		
		currentMode = mode_data.mode;
		currentSubMode = mode_data.sub_mode;

		//Every game starts from field view
		CameraManager.Instance.ShowField ();

		switch (currentMode) {
		case GameMode.Practice:
			mode = new PracticeMode ();
			break;
		case GameMode.SinglePlayer:
			mode = new SingleMode (currentSubMode, mode_data.teamPlayer, mode_data.teamEnemy);
			break;
		case GameMode.MultiPlayer:
			mode = new MultiOffMode (mode_data.teamPlayer, mode_data.teamEnemy);
			break;
		case GameMode.MultiplayerOnline:
			mode = new MultiOnMode (mode_data.teamPlayer, mode_data.teamEnemy);
			isOnlineGameActive = true;
			isOnlineScoreRecieved = false;
			isRingStopped = false;
			break;
		}

		mode.CreateGame ();
		GameView.Instance.SwitchPanelGame ();

	}

	public void FinishGame(){

		if (currentMode == GameMode.MultiplayerOnline)
			isOnlineGameActive = false;

		GameView.Instance.SwitchPanelGame ();
		
		currentMode = GameMode.None;
		currentSubMode = GameSubMode.None;
		currentState = GameState.None;
		currentPlayer = null;

		numPassedShootsCola = 0;
		numPassedShootsPepsi = 0;

		Invoke ("DestroyGame", 1f);
	}

	void DestroyGame(){
		if (mode != null) {
			mode.DestroyGame ();
			mode = null;
		}
	}

	public void GameIsReady(){

		if (currentMode == GameMode.None)
			return;

		//Show Player
		switch(currentMode){
		case GameMode.SinglePlayer:

			bool isCurrentPlayerBot = PlayerManager.Instance.isBot;

			if (isCurrentPlayerBot) {
				currentState = GameState.ViewResult;
				PlayerManager.Instance.SwitchPlayer (currentPlayer, true, true);
				currentPlayer.ring.GetComponent<Ring> ().Bot_ShootRing ();
			} 
			else {
				currentState = GameState.ObserveField;
			}

			break;
		case GameMode.MultiplayerOnline:

			bool isCurrentPlayerEnemy = PlayerManager.Instance.isEnemy;

			if (isCurrentPlayerEnemy) {
				currentState = GameState.ViewResult;
			}
			else {
				currentState = GameState.ObserveField;
			}

			break;
		default:
			currentState = GameState.ObserveField;
			break;
		}

		if(!PlayerManager.Instance.isBot  && !PlayerManager.Instance.isEnemy)
			GameView.Instance.ShowGameState (currentState);
	}

	public void ChangeGameState(){
		switch (currentState) {
		case GameState.ObserveField:
			CameraManager.Instance.ShowRing (0f, currentPlayer);
			PlayerManager.Instance.SwitchPlayer (currentPlayer, true, false);

			currentState = GameState.RotateDisk;
			break;
		case GameState.RotateDisk:
			currentState = GameState.MoveDisk;
			break;
		case GameState.MoveDisk:
			currentState = GameState.Shoot;
			break;
		case GameState.Shoot:
			currentState = GameState.ViewResult;
			break;
		}

		GameView.Instance.ShowGameState (currentState);
	}

	public void NextRound(){

		currentState = GameState.None;
		GameView.Instance.ShowGameState (currentState);

		mode.ActivatePlayers ();
		CameraManager.Instance.ShowField ();

		if (!PlayerManager.Instance.isEnemy) {
			Invoke ("CountScore", 3f);
			Invoke ("SetCurrentPlayer", 4f);
		} 
		else {
			isRingStopped = true;
			_updateFieldandScore.Invoke ();
		}
	}

	public void SetOnlineShoot(Vector3 position, Vector3 rotation, Vector3 velocity){
		//if (velocity.magnitude > 0.5f) {
			PlayerManager.Instance.SwitchPlayer (currentPlayer, true, true);
			currentPlayer.ring.GetComponent<Ring> ().OnlinePlayer_ShootRing (position, rotation, velocity);
		//} else
			//TimeIsOut ();
	}

	public void SetOnlineScore (Score score, List<bool> cola_bottles, List<bool> pepsi_bottles){
		online_score = score;
		online_cola_bottles = cola_bottles;
		online_pepsi_bottles = pepsi_bottles;

		isOnlineScoreRecieved = true;
		_updateFieldandScore.Invoke ();
	}

	void OnlineCountScore (){
		//Debug.Log (isOnlineScoreRecieved + " " + isRingStopped);
		if (isOnlineScoreRecieved && isRingStopped) {
			isOnlineScoreRecieved = false;
			isRingStopped = false;

			FieldManager.Instance.SetOnlineField (online_cola_bottles, online_pepsi_bottles);
			PlayerManager.Instance.SetPlayersScore (online_score);
			GameView.Instance.SetColaScore (online_score.score_cola);
			GameView.Instance.SetPepsiScore (online_score.score_pepsi);

			Invoke ("SetCurrentPlayer", 1f);
		}
	}

	void CountScore(){

		Score score = FieldManager.Instance.CountBottles ();

		switch (currentMode) {
		case GameMode.Practice:
			GameView.Instance.SetPlayerScore (score.score);
			break;
		default:
			PlayerManager.Instance.SetPlayersScore (score);

			GameView.Instance.SetColaScore (score.score_cola);
			GameView.Instance.SetPepsiScore (score.score_pepsi);

			break;
		}

	}

	void SetCurrentPlayer(){
		if(mode != null)
			currentPlayer = mode.CheckTurn ();
	}

	public void SetPlayerToNextRow(Player player = null){

		if (player == null)
			player = currentPlayer;

		if (currentMode == GameMode.Practice) {
			player.currentRow = "middle";
			return;
		}

		switch (player.currentRow) {
		case "most_left":
			player.currentRow = "left";
			break;
		case "left":
			player.currentRow = "middle";
			break;
		case "middle":
			player.currentRow = "right";
			break;
		case "right":
			player.currentRow = "most_right";
			break;
		case "most_right":
			player.currentRow = "most_left";
			break;
		}
	}

	public void TimeIsOut(){

		if (currentMode == GameMode.Practice) {
			NextPlayer ();
			return;
		}

		if (currentPlayer.team == Team.Cola) {
			numPassedShootsCola++;

			if (numPassedShootsCola == 2) {
				bool isWon = PlayerManager.Instance.player.team == Team.Cola ? false : true;
				ManagersController.Message (Message.Create (this, MessageData.EVENT_FINISH_GAME, isWon));
			} else
				NextPlayer ();
		} 
		else{
			numPassedShootsPepsi++;

			if (numPassedShootsPepsi == 2) {
				bool isWon = PlayerManager.Instance.player.team == Team.Pepsi ? false : true;
				ManagersController.Message (Message.Create (this, MessageData.EVENT_FINISH_GAME, isWon));
			} else
				NextPlayer ();
		}


	}

	void NextPlayer(){
		currentPlayer.ring.GetComponent<Ring> ().SendRingShootInfo (Vector3.zero);

		currentState = GameState.None;

		currentPlayer.ring.GetComponent<Ring> ().ResetRing ();
		SetPlayerToNextRow ();
		NextRound ();
	}

}

