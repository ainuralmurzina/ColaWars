using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
	#region Authentication
	public static ResultView Instance {
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

	public GameObject pnlResult;
	public Text txtResult;

	public GameObject objColaPlayer;
	public GameObject objPepsiPlayer;

	public Button btnPlayAgain;
	public Button btnHomeResult;

	[Header ("Save Result Popup")]
	public GameObject pnlSaveResult;
	public Button btnRegistration;
	public Button btnCloseSaveResult;

	#endregion

	Player registration_required_player = null;

	void OnEnable(){
		btnPlayAgain.onClick.AddListener (() => HideResult (false));
		btnHomeResult.onClick.AddListener (() => HideResult (true));
		btnCloseSaveResult.onClick.AddListener (() => AnimatePopupClose (pnlSaveResult));
		btnRegistration.onClick.AddListener (() => RegistratePlayer ());
	}

	public void SwitchResult(bool isPlayerWon = false, Player player = null, Player enemy = null) {
		if (!pnlResult.activeSelf) {
			SetResult (isPlayerWon, player, enemy);
			SaveResult (player);
			ShowResult ();
		} 
		else {
			HideResult (true);
		}
	}

	void SetResult(bool isPlayerWon, Player player, Player enemy){

		if (isPlayerWon) {
			txtResult.text = "ВЫ ВЫЙГРАЛИ!";
			ManagersController.Message (Message.Create (this, MessageData.EVENT_SUBMIT_WINNED_TEAM));
		}
		else
			txtResult.text = "ВЫ ПРОИГРАЛИ!";

		Player colaPlayer = null;
		Player pepsiPlayer = null;

		if (player.team == Team.Cola) {
			colaPlayer = player;
			pepsiPlayer = enemy;
		} 
		else {
			colaPlayer = enemy;
			pepsiPlayer = player;
		}

		objColaPlayer.transform.Find ("imgCrown").gameObject.SetActive (isPlayerWon && player.team == Team.Cola);
		objPepsiPlayer.transform.Find ("imgCrown").gameObject.SetActive (isPlayerWon && player.team == Team.Pepsi);

		objColaPlayer.transform.Find ("txtScore").GetComponent<Text> ().text = colaPlayer.currentScore.ToString ();
		objPepsiPlayer.transform.Find ("txtScore").GetComponent<Text> ().text = pepsiPlayer.currentScore.ToString ();

		objColaPlayer.transform.Find ("txtName").GetComponent<Text> ().text = colaPlayer.displayName;
		objPepsiPlayer.transform.Find ("txtName").GetComponent<Text> ().text = pepsiPlayer.displayName;
	}

	void ShowResult(){

		Sequence seq = DOTween.Sequence ();
		RectTransform rt;

		pnlResult.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
		seq.Append (pnlResult.GetComponent<Image>().DOFade (1f, 0.5f));

		rt = objColaPlayer.GetComponent<RectTransform> ();
		rt.anchoredPosition = new Vector2 (-1000f, rt.anchoredPosition.y);
		seq.Append (rt.DOAnchorPosX(0f, 0.35f).SetEase(Ease.OutBack));

		rt = objPepsiPlayer.GetComponent<RectTransform> ();
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

	void HideResult(bool returnToMenu){

		Sequence seq = DOTween.Sequence ();
		RectTransform rt;

		rt = objPepsiPlayer.GetComponent<RectTransform> ();
		seq.Append (rt.DOAnchorPosX(1000f, 0.2f).SetEase(Ease.InBack));

		rt = objColaPlayer.GetComponent<RectTransform> ();
		seq.Join (rt.DOAnchorPosX(-1000f, 0.2f).SetEase(Ease.InBack));

		rt = txtResult.GetComponent<RectTransform> ();
		seq.Join (rt.DOScale(Vector2.zero, 0.2f).SetEase(Ease.InBack));

		rt = btnPlayAgain.GetComponent<RectTransform> ();
		seq.Join (rt.DOAnchorPosY(-1150, 0.2f).SetEase(Ease.InBack));

		rt = btnHomeResult.GetComponent<RectTransform> ();
		seq.Join (rt.DOAnchorPosY(-1150, 0.2f).SetEase(Ease.InBack));

		seq.Append (pnlResult.GetComponent<Image>().DOFade (0f, 0.5f));

		seq.OnComplete (() => {
			pnlResult.SetActive (false);

			if(returnToMenu)
				ManagersController.Message(Message.Create(this, MessageData.EVENT_SHOW_MENU));
			else
				ManagersController.Message(Message.Create(this, MessageData.EVENT_PLAY_AGAIN));
		});
	}

	void SaveResult(Player player){
		if (FacebookManager.Instance.IsLoggedIn ()) {
			ManagersController.Message (Message.Create (this, MessageData.EVENT_SUBMIT_SCORE, player.currentScore));
			PlayerPrefs.SetInt ("Score", PlayerPrefs.GetInt ("Score", 0) + player.currentScore);
		} 
		else {
			registration_required_player = player;
			AnimatePopupOpen (pnlSaveResult, 2.5f);
		}
	}

	void RegistratePlayer (){
		ManagersController.Message (Message.Create (this, MessageData.EVENT_LOGIN2));
		AnimatePopupClose (pnlSaveResult);
	}

	//CALLBACK
	public void OnRegistrationResult(bool isSucceeded){
		if (isSucceeded && registration_required_player != null) {
			SaveResult (registration_required_player);
			registration_required_player = null;
		}
	}

	//POPUPS
	public void AnimatePopupOpen(GameObject popup, float delay){
		popup.transform.GetChild (0).localScale = Vector2.zero;

		popup.transform.GetChild (0).DOScale (new Vector2(1f, 1f), 0.25f)
			.SetEase (Ease.OutBack)
			.SetDelay(delay)
			.OnPlay (() => popup.SetActive (true));
	}

	public void AnimatePopupClose(GameObject popup){
		popup.transform.GetChild (0).DOScale (Vector2.zero, 0.25f).SetEase (Ease.InBack)
			.OnComplete(() => popup.SetActive(false));
	}
}

