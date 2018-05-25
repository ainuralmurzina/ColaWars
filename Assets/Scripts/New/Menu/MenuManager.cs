using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace Test{
	public class MenuManager : MonoBehaviour {

		public GameManager mngrGame;

		[Space (10)]
		public Image imgBG;

		[Header ("Panels")]
		public GameObject pnlMenu;
		public GameObject pnlBottomMenu;
		public GameObject pnlGame;
		public GameObject pnlResult;
		public GameObject pnlLoading;

		[Header ("Menu")]
		public Button btnSinglePlayer;
		public Button btnMultiPlayerOn;
		public Button btnMultiPlayerOff;
		public Button btnPractice;

		[Header ("Bottom Menu")]
		public Button btnProfile;
		public Button btnRatings;
		public Button btnSettings;

		[Header ("Loading")]
		public Slider slrLoading;

		[Header ("Game")]
		public Button btnPause;
		public GameObject objStates;

		[Header ("Score")]
		public Text txtScore1;
		public Text txtScore2;
		public GameObject objMulitpleScore;

		[Header ("Timer")]
		public GameObject objTimer;
		public Image timerGreen;
		public Image timerYellow;
		public Image timerRed;
		public Text txtTimer;
		public float timeMax = 31f;

		[Header ("Result")]
		public Text txtResult;
		public GameObject objPlayer1;
		public GameObject objPlayer2;
		public Button btnPlayAgain;
		public Button btnHomeResult;

		[Header ("Exit Game Popup")]
		public GameObject pnlExitGame;
		public Button btnExitGame;
		public Button btnResume;


		//Game Settings
		private List<Team> playingTeams = new List<Team> ();

		//Timer
		private bool isTimerActive = false;
		private float currentTime = 31f;

		void Start(){
			playingTeams.Add (Team.Pepsi);
			playingTeams.Add (Team.Cola);
		}

		public void StartGame_MultiOff(){
			mngrGame.CreateAndStartGame (playingTeams);

			HideMainMenu ();
			HideBottomMenu ();
			StartCoroutine(ShowLoading ());
			StartCoroutine(HideBackground ());
			StartCoroutine(ShowGameMenu ());
		}

		public void PlayAgain(){
			mngrGame.CreateAndStartGame (playingTeams);

			HideResult ();
			StartCoroutine(ShowLoading ());
			StartCoroutine(HideBackground ());
			StartCoroutine(ShowGameMenu ());
		}

		public void BackToMenu_FromeResults(){
			HideResult ();
			StartCoroutine(ShowMainMenu (0.8f));
			StartCoroutine(ShowBottomMenu (0.8f));
		}

		public void Pause(){
			Time.timeScale = 0f;
			AnimatePopupOpen (pnlExitGame);
		}

		public void Resume(){
			AnimatePopupClose (pnlExitGame);
			Time.timeScale = 1f;
		}

		public void ExitGame(){
			AnimatePopupClose (pnlExitGame);
			HideGameMenu ();
			ShowBackground ();
			StartCoroutine(ShowMainMenu (0.5f));
			StartCoroutine(ShowBottomMenu (0.5f));
		}

		IEnumerator ShowMainMenu(float waitTime){
			
			yield return new WaitForSeconds (waitTime);

			btnPractice.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, -330f);
			btnSinglePlayer.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, 330f);
			btnMultiPlayerOff.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, -115f);
			btnMultiPlayerOn.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, 115f);

			pnlMenu.SetActive (true);

			btnSinglePlayer.GetComponent<RectTransform> ().DOAnchorPosX (0f, 1f).SetEase (Ease.OutElastic, 0.1f);
			btnMultiPlayerOn.GetComponent<RectTransform> ().DOAnchorPosX (0f, 1.25f).SetEase (Ease.OutElastic, 0.1f);
			btnMultiPlayerOff.GetComponent<RectTransform> ().DOAnchorPosX (0f, 1.5f).SetEase (Ease.OutElastic, 0.1f);
			btnPractice.GetComponent<RectTransform> ().DOAnchorPosX (0f, 1.75f).SetEase (Ease.OutElastic, 0.1f);
		}

		void HideMainMenu(){
			btnSinglePlayer.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.2f).SetEase (Ease.InBack);
			btnMultiPlayerOn.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.25f).SetEase (Ease.InBack);
			btnMultiPlayerOff.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.3f).SetEase (Ease.InBack);
			btnPractice.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.35f).SetEase (Ease.InBack)
				.OnComplete( () => pnlMenu.SetActive (false));
		}

		IEnumerator ShowBottomMenu(float waitTime){

			yield return new WaitForSeconds (0.5f);

			btnSettings.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-285f, -240f);
			btnProfile.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (285f, -240f);
			btnRatings.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, -150f);

			pnlBottomMenu.SetActive (true);

			btnSettings.GetComponent<RectTransform> ().DOAnchorPosY (178f, 1f).SetEase (Ease.OutElastic, 0.1f);
			btnRatings.GetComponent<RectTransform> ().DOAnchorPosY (267f, 1.25f).SetEase (Ease.OutElastic, 0.1f);
			btnProfile.GetComponent<RectTransform> ().DOAnchorPosY (178f, 1.5f).SetEase (Ease.OutElastic, 0.1f);
		}

		void HideBottomMenu(){
			btnSettings.GetComponent<RectTransform> ().DOAnchorPosY (-240f, 0.2f).SetEase (Ease.InBack);
			btnRatings.GetComponent<RectTransform> ().DOAnchorPosY (-150f, 0.25f).SetEase (Ease.InBack);
			btnProfile.GetComponent<RectTransform> ().DOAnchorPosY (-240f, 0.3f).SetEase (Ease.InBack)
				.OnComplete(() => pnlBottomMenu.SetActive (false));
		}

		IEnumerator ShowLoading(){

			yield return new WaitForSeconds (0.5f);

			slrLoading.value = 0f;
			slrLoading.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, -1000f);

			Sequence animSeq = DOTween.Sequence ();

			animSeq.Append (slrLoading.GetComponent<RectTransform>().DOAnchorPosY(-460f, 0.5f).SetEase(Ease.OutBack));
			animSeq.Append (slrLoading.DOValue (1f, 3f));
			animSeq.Append (slrLoading.GetComponent<RectTransform>().DOAnchorPosY(-1000f, 0.5f).SetEase(Ease.InBack));

			animSeq.OnPlay (() => pnlLoading.SetActive (true));
			animSeq.OnComplete (() => pnlLoading.SetActive (false));
		}

		IEnumerator HideBackground(){
			yield return new WaitForSeconds (5f);
			imgBG.DOFade (0f, 0.5f).OnComplete(() => imgBG.gameObject.SetActive(false));
		}

		void ShowBackground(){
			imgBG.DOFade (1f, 0.5f).OnPlay(() => imgBG.gameObject.SetActive(true));
		}

		IEnumerator ShowGameMenu(){

			txtScore1.text = "0";
			txtScore2.text = "0";

			yield return new WaitForSeconds (5.5f);

			btnPause.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-105.85f, 105.85f);
			txtScore1.transform.parent.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-885f, -19.9f);
			txtScore2.transform.parent.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (885f, -19.9f);

			objMulitpleScore.SetActive (true);
			pnlGame.SetActive (true);

			btnPause.GetComponent<RectTransform> ().DOAnchorPosX (105.85f, 1f).SetEase (Ease.OutElastic, 0.1f);
			txtScore1.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (-325f, 0.5f).SetEase (Ease.OutBack);
			txtScore2.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (325f, 0.5f).SetEase (Ease.OutBack);
		}

		void HideGameMenu (){

			//Hide Score
			txtScore1.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (-885f, 0.25f).SetEase (Ease.InBack);
			txtScore2.transform.parent.GetComponent<RectTransform> ().DOAnchorPosX (885f, 0.25f).SetEase (Ease.InBack);

			//Hide Timer and Timer
			HideTimer();
			HideState ();

			//Hide Pause Button
			btnPause.GetComponent<RectTransform> ().DOAnchorPosX (-105.85f, 0.25f).SetEase (Ease.InBack)
				.OnComplete (() => {pnlGame.SetActive (false);});
		}

		void ShowTimer(float waitTime = 0f){

			currentTime = timeMax;
			txtTimer.text = currentTime.ToString ();

			timerGreen.DOFade (1f, 0f);
			timerYellow.DOFade (1f, 0f);

			objTimer.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (360f, 99f);
			objTimer.GetComponent<RectTransform> ().DOAnchorPosX (-360f, 0.5f).SetEase (Ease.OutBack)
				.SetDelay(waitTime)
				.OnComplete(() => isTimerActive = true);
		}

		void HideTimer(){
			objTimer.GetComponent<RectTransform> ().DOAnchorPosX (360f, 0.25f).SetEase (Ease.InBack)
				.OnPlay(() => isTimerActive = false);
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

					mngrGame.TimeIsOut ();
				}
			}

		}

		public void ShowGameState(GameState currentState){
			HideState ();

			float waitTime = 0f;

			switch(currentState){
			case GameState.ObserveField:
				waitTime = 2f;
				ShowTimer (waitTime);
				ShowState (objStates.transform.GetChild (0), waitTime);
				break;
			case GameState.RotateDisk:
				waitTime = 0.5f;
				ShowState (objStates.transform.GetChild (1), waitTime);
				break;
			case GameState.MoveDisk:
				waitTime = 0.5f;
				ShowState (objStates.transform.GetChild (2), waitTime);
				break;
			case GameState.Shoot:
				waitTime = 0.5f;
				ShowState (objStates.transform.GetChild (3), waitTime);
				break;
			case GameState.ViewResult:
			case GameState.None:
				HideTimer ();
				break;
			}
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

		public int AddScore(int score, Team team){

			int curScore = 0;

			if(team == playingTeams[1]){
				curScore = Int32.Parse (txtScore1.text);
				curScore += score;
				txtScore1.text = curScore.ToString ();
			}

			if(team == playingTeams[0]){
				curScore = Int32.Parse (txtScore2.text);
				curScore += score;
				txtScore2.text = curScore.ToString ();
			}

			return curScore;
		}

		public void ShowResults(){

			SetResult ();
			HideGameMenu ();
			ShowBackground ();

			Sequence seq = DOTween.Sequence ();
			RectTransform rt;

			pnlResult.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
			seq.Append (pnlResult.GetComponent<Image>().DOFade (1f, 0.5f));

			rt = objPlayer1.GetComponent<RectTransform> ();
			rt.anchoredPosition = new Vector2 (-1000f, rt.anchoredPosition.y);
			seq.Append (rt.DOAnchorPosX(0f, 0.35f).SetEase(Ease.OutBack));

			rt = objPlayer2.GetComponent<RectTransform> ();
			rt.anchoredPosition = new Vector2 (1000f, rt.anchoredPosition.y);
			seq.Append (rt.DOAnchorPosX(0f, 0.35f).SetEase(Ease.OutBack));

			rt = txtResult.GetComponent<RectTransform> ();
			rt.localScale = new Vector2 (0f, 0f);
			seq.Append (rt.DOScale(new Vector2 (1f, 1f), 0.5f).SetEase(Ease.OutBack, 4f)).AppendInterval(0.25f);

			rt = btnPlayAgain.GetComponent<RectTransform> ();
			rt.anchoredPosition = new Vector2 (0f, -1150f);
			seq.Append (rt.DOAnchorPosY(-320f, 0.35f).SetEase(Ease.OutBack));

			rt = btnHomeResult.GetComponent<RectTransform> ();
			rt.anchoredPosition = new Vector2 (0f, -1150f);
			seq.Join (rt.DOAnchorPosY(-600f, 0.55f).SetEase(Ease.OutBack));

			//Hide and Finish Game
			seq.OnPlay (() => {
				pnlResult.SetActive (true);
			});

		}

		void HideResult(){

			Sequence seq = DOTween.Sequence ();
			RectTransform rt;

			rt = objPlayer1.GetComponent<RectTransform> ();
			seq.Append (rt.DOAnchorPosX(1000f, 0.2f).SetEase(Ease.InBack));

			rt = objPlayer2.GetComponent<RectTransform> ();
			seq.Join (rt.DOAnchorPosX(-1000f, 0.2f).SetEase(Ease.InBack));

			rt = txtResult.GetComponent<RectTransform> ();
			seq.Join (rt.DOScale(Vector2.zero, 0.2f).SetEase(Ease.InBack));

			rt = btnPlayAgain.GetComponent<RectTransform> ();
			seq.Join (rt.DOAnchorPosY(-1150, 0.2f).SetEase(Ease.InBack));

			rt = btnHomeResult.GetComponent<RectTransform> ();
			seq.Join (rt.DOAnchorPosY(-1150, 0.2f).SetEase(Ease.InBack));

			seq.Append (pnlResult.GetComponent<Image>().DOFade (0f, 0.5f));

			seq.OnComplete (() => {pnlResult.SetActive (false);});
		}

		void SetResult(){

			txtResult.text = "ВЫ ВЫЙГРАЛИ!";

			objPlayer1.transform.Find ("imgCrown").gameObject.SetActive (Int32.Parse(txtScore1.text) < Int32.Parse(txtScore2.text));
			objPlayer2.transform.Find ("imgCrown").gameObject.SetActive (Int32.Parse(txtScore1.text) > Int32.Parse(txtScore2.text));

			objPlayer1.transform.Find ("txtScore").GetComponent<Text> ().text = txtScore1.text;
			objPlayer2.transform.Find ("txtScore").GetComponent<Text> ().text = txtScore2.text;

			objPlayer1.transform.Find ("txtName").GetComponent<Text> ().text = "ИГРОК 1";
			objPlayer2.transform.Find ("txtName").GetComponent<Text> ().text = "ИГРОК 2";
		}

		public void AnimatePopupOpen(GameObject popup){
			popup.transform.GetChild (0).localScale = Vector2.zero;
			popup.SetActive (true);
			popup.transform.GetChild (0).DOScale (new Vector2(1f, 1f), 0.25f).SetEase (Ease.OutBack);
		}

		public void AnimatePopupClose(GameObject popup){
			popup.transform.GetChild (0).DOScale (Vector2.zero, 0.25f).SetEase (Ease.InBack)
				.OnComplete(() => popup.SetActive(false));
		}
	}
}
