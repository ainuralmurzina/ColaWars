using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class GameView : MonoBehaviour
{
	#region Authentication
	public static GameView Instance {
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

	#region Public Variables

	public GameObject pnlGame;
	public Button btnPause;
	public GameObject objStates;

	[Header ("Score")]
	public Text txtScorePlayer;
	public Text txtScoreCola;
	public Text txtScorePepsi;
	public GameObject objSingleScore;
	public GameObject objMulitpleScore;

	[Header ("Timer")]
	public GameObject objTimer;
	public Image timerGreen;
	public Image timerYellow;
	public Image timerRed;
	public Text txtTimer;
	public float timeMax = 31f;

	#endregion

	//Timer
	private bool isTimerActive = false;
	private float currentTime = 31f;

	void OnEnable(){
		btnPause.onClick.AddListener (() => {
			Time.timeScale = 0f;
			ManagersController.Message(Message.Create(this, MessageData.EVENT_PAUSE_GAME));
		});
	}

	public void SwitchPanelGame(){
		if (!pnlGame.activeSelf)
			Invoke ("ShowGame", 0.5f);
		else
			HideGame ();
	}

	void ShowGame(){
		if (GameManager.Instance.currentMode == GameMode.Practice) {
			btnPause.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-105.85f, 105.85f);
			objSingleScore.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-300f, -160f);

			objMulitpleScore.SetActive (false);
			objSingleScore.SetActive (true);
			pnlGame.SetActive (true);

			btnPause.GetComponent<RectTransform> ().DOAnchorPosX (105.85f, 1f).SetEase (Ease.OutElastic, 0.1f);
			objSingleScore.GetComponent<RectTransform> ().DOAnchorPosX (540f, 0.5f).SetEase (Ease.OutBack);
		} 
		else {

			btnPause.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-105.85f, 105.85f);
			txtScoreCola.transform.parent.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-885f, -19.9f);
			txtScorePepsi.transform.parent.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (885f, -19.9f);

			objSingleScore.SetActive (false);
			objMulitpleScore.SetActive (true);
			pnlGame.SetActive (true);

			btnPause.GetComponent<RectTransform> ().DOAnchorPosX (105.85f, 1f).SetEase (Ease.OutElastic, 0.1f);
			txtScoreCola.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (-325f, 0.5f).SetEase (Ease.OutBack);
			txtScorePepsi.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (325f, 0.5f).SetEase (Ease.OutBack);

		}
	}

	void HideGame (){

		//Hide Score
		if (GameManager.Instance.currentMode == GameMode.Practice) {
			objSingleScore.GetComponent<RectTransform> ().DOAnchorPosX (-300f, 0.25f).SetEase (Ease.InBack);
		} 
		else {
			txtScoreCola.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (-885f, 0.25f).SetEase (Ease.InBack);
			txtScorePepsi.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (885f, 0.25f).SetEase (Ease.InBack);
		}

		//Hide Timer and Timer
		HideTimer();
		HideState ();

		//Hide Pause Button
		btnPause.GetComponent<RectTransform> ().DOAnchorPosX (-105.85f, 0.25f).SetEase (Ease.InBack)
			.OnComplete (() => {
				pnlGame.SetActive (false);
				SetPepsiScore (0);
				SetColaScore (0);
				SetPlayerScore (0);
			});
	}

	void Update(){

		if (isTimerActive) {
			currentTime -= Time.deltaTime;
			txtTimer.text = Mathf.Round(currentTime).ToString ();

			if (currentTime <= 12f && timerGreen.color.a == 1f)
				timerGreen.DOFade (0f, 2f);

			if (currentTime <= 7f && timerYellow.color.a == 1f)
				timerYellow.DOFade (0f, 3f);

			if (currentTime <= 0.1f) {

				isTimerActive = false;

				HideTimer ();
				HideState ();

				GameManager.Instance.TimeIsOut ();
			}
		}

	}

	void ShowTimer(){

		currentTime = timeMax;
		txtTimer.text = currentTime.ToString ();

		timerGreen.DOFade (1f, 0f);
		timerYellow.DOFade (1f, 0f);

		objTimer.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (360f, 99f);
		objTimer.GetComponent<RectTransform> ().DOAnchorPosX (-360f, 0.5f).SetEase (Ease.OutBack)
			.OnComplete(() => isTimerActive = true);
	}

	void HideTimer(){
		objTimer.GetComponent<RectTransform> ().DOAnchorPosX (360f, 0.25f).SetEase (Ease.InBack)
			.OnPlay(() => isTimerActive = false);
	}

	void ShowState(Transform state, float waitTime = 0f){
		state.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (137.65f, 137.65f);
		state.GetComponent<RectTransform> ().DOAnchorPosX (-137.65f, 0.25f).SetEase (Ease.OutBack).SetDelay(waitTime);
	}

	void HideState(){
		foreach (Transform t in objStates.transform) {
			t.GetComponent<RectTransform> ().DOAnchorPosX (137.65f, 0.25f).SetEase (Ease.InBack);
		}
	}

	public void SetPlayerScore(int score){
		txtScorePlayer.text = score.ToString ();
	}

	public void SetColaScore(int score){
		txtScoreCola.text = score.ToString ();
		if (score >= 50 && GameManager.Instance.currentPlayer.currentID == 2) {
			bool isWon = PlayerManager.Instance.player.team == Team.Cola ? true : false;
			ManagersController.Message(Message.Create(this, MessageData.EVENT_FINISH_GAME, isWon));
		}
	}

	public void SetPepsiScore(int score){
		txtScorePepsi.text = score.ToString ();
		if (score >= 50 && GameManager.Instance.currentPlayer.currentID == 2) {
			bool isWon = PlayerManager.Instance.player.team == Team.Pepsi ? true : false;
			ManagersController.Message(Message.Create(this, MessageData.EVENT_FINISH_GAME, isWon));
		}
	}

	public void ShowGameState(GameState currentState){
		HideState ();
		switch(currentState){
		case GameState.ObserveField:
			ShowTimer ();
			ShowState (objStates.transform.GetChild (0), 0f);
			break;
		case GameState.RotateDisk:
			ShowState (objStates.transform.GetChild (1), 0.5f);
			break;
		case GameState.MoveDisk:
			ShowState (objStates.transform.GetChild (2), 0.5f);
			break;
		case GameState.Shoot:
			ShowState (objStates.transform.GetChild (3), 0.5f);
			break;
		case GameState.ViewResult:
		case GameState.None:
			HideTimer ();
			break;
		}
	}

}

