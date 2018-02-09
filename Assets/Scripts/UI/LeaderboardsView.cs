using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class LeaderboardsView : MonoBehaviour
{
	#region Instance Initialization
	public static LeaderboardsView Instance {
		get;
		private set;
	}

	void Awake(){
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		} else {
			Instance = this;
		}
	}
	#endregion

	#region Public Variables
	public GameObject pnlRatings;
	public Button btnCloseRatings;

	[Space (10)]
	public GameObject pnlUser;

	[Header ("Sliders")]
	public ScrollRect scrollMain;
	public Transform trDay;
	public Transform trMonth;
	public Transform trOverall;

	[Space (10)]
	public Text txtBoardTitle;
	public Button btnNextBoard;
	public Button btnPrevBoard;

	[Space (10)]
	public Sprite imgCola;
	public Sprite imgPepsi;

	[Space (10)]
	public Text txtColaWins;
	public Text txtPepsiWins;

	#endregion

	#region Private Variables

	//Overall - 0/Month - 1/Day - -1
	private int currentBoard = 0;

	#endregion

	void Start () {
		ManagersController.Message (Message.Create (this, MessageData.EVENT_LEADERBOARD_UPDATE));
	}

	void OnEnable(){
		btnCloseRatings.onClick.AddListener (() => HideRatings ());
		btnNextBoard.onClick.AddListener (() => NextBoard ());
		btnPrevBoard.onClick.AddListener (() => PrevBoard ());

		scrollMain.onValueChanged.AddListener (value => CheckBoard ());
	}

	public void UpdateTeamsScore(string score_cola, string score_pepsi){
		txtColaWins.text = score_cola;
		txtPepsiWins.text = score_pepsi;
	}

	public void UpdateOverall (List<GlobalScore> leaderboard){
		FillLeaderboard (leaderboard, trOverall);
	}

	public void UpdateDay (List<GlobalScore> leaderboard){
		FillLeaderboard (leaderboard, trDay);
	}

	public void UpdateMonth (List<GlobalScore> leaderboard){
		FillLeaderboard (leaderboard, trMonth);
	}

	void FillLeaderboard(List<GlobalScore> leaderboard_list, Transform leaderboard){
		CleanLeaderboard (leaderboard);
		int counter = 1;
		foreach(GlobalScore gs in leaderboard_list){
			pnlUser.transform.Find ("txtNum").GetComponent<Text> ().text = counter.ToString ();
			pnlUser.transform.Find ("txtName").GetComponent<Text> ().text = gs.displayName.ToString ();
			pnlUser.transform.Find ("txtScore").GetComponent<Text> ().text = gs.score.ToString ();

			pnlUser.transform.Find ("imgTeam").GetComponent<Image> ().sprite = gs.team == "Cola" ? imgCola : imgPepsi;

			Instantiate (pnlUser, leaderboard);
			counter++;
		}
	}

	void CleanLeaderboard (Transform leaderboard){
		foreach (Transform t in leaderboard) {
			Destroy (t.gameObject);
		}
	}

	public void SwitchRating(){
		if (!pnlRatings.activeSelf) {
			ManagersController.Message (Message.Create (this, MessageData.EVENT_LEADERBOARD_UPDATE));
			ShowRatings ();
		}
		else
			HideRatings ();
	}

	void ShowRatings(){
		pnlRatings.SetActive (true);
		currentBoard = 0;
	}

	void HideRatings (){
		pnlRatings.SetActive (false);
	}

	void NextBoard (){
		switch (currentBoard) {
		case -1:
			currentBoard = 0;
			scrollMain.DOHorizontalNormalizedPos (0.5f, 0.5f).SetEase (Ease.OutBack);
			break;
		case 0:
			currentBoard = 1;
			scrollMain.DOHorizontalNormalizedPos (1f, 0.5f).SetEase (Ease.OutBack);
			break;
		}
		SetBoardTitle ();
	}

	void PrevBoard (){
		switch (currentBoard) {
		case 0:
			currentBoard = -1;
			scrollMain.DOHorizontalNormalizedPos (0f, 0.5f).SetEase (Ease.OutBack);
			break;
		case 1:
			currentBoard = 0;
			scrollMain.DOHorizontalNormalizedPos (0.5f, 0.5f).SetEase (Ease.OutBack);
			break;
		}
		SetBoardTitle ();
	}

	void SetBoardTitle (){
		switch (currentBoard) {
		case -1:
			txtBoardTitle.text = "ДЕНЬ";
			break;
		case 0:
			txtBoardTitle.text = "ОБЩИЙ";
			break;
		case 1:
			txtBoardTitle.text = "МЕСЯЦ";
			break;
		}
	}

	void CheckBoard (){
		if(scrollMain.horizontalNormalizedPosition < 0.3f)
			currentBoard = -1;
		if(scrollMain.horizontalNormalizedPosition > 0.3f && scrollMain.horizontalNormalizedPosition < 0.7f)
			currentBoard = 0;
		if(scrollMain.horizontalNormalizedPosition > 0.7f)
			currentBoard = 1;
		
		SetBoardTitle ();
	}
}

