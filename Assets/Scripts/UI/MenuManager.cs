using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MenuManager : MonoBehaviour
{

	#region Instance Initialization
	public static MenuManager Instance {
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

	#region Public Variabels
	public Image imgBG;

	[Header ("Panels")]
	public GameObject pnlBottomMenu;
	public GameObject pnlMenu;
	public GameObject pnlSingleMode;
	public GameObject pnlGame;

	[Header ("Loading Popup")]
	public GameObject pnlWait;
	public Text txtWaitDescription;

	[Header ("Notification Popup")]
	public GameObject pnlNotification;
	public Button btnCloseNotification;
	public Text txtNotification;

	[Header ("Menu")]
	public Button btnPractice;
	public Button btnSinglePlayer;
	public Button btnMultiPlayerOff;
	public Button btnMultiPlayerOn;

	[Header ("Bottom Menu")]
	public Button btnProfile;
	public Button btnRatings;
	public Button btnSettings;

	[Header ("Single Mode")]
	public Button btnBack;
	public Button btnSingleEasy;
	public Button btnSingleMedium;
	public Button btnSingleHard;

	[Header ("Exit Game Popup")]
	public GameObject pnlExitGame;
	public Button btnExitGame;
	public Button btnResume;

	[Header ("Choose Team Popup")]
	public GameObject pnlChooseTeam;
	public Button btnPepsiTeam;
	public Button btnColaTeam;
	public Button btnCloseChooseTeam;

	[Header ("Profile Popup")]
	public GameObject pnlProfile;
	public Button btnCloseProfile;
	public Text txtName;
	public Image imgProfilePicture;
	public Text txtScore;
	public Button btnLogout;

	[Header ("Registration Popup")]
	public GameObject pnlRegistration;
	public Button btnLogin;
	public Button btnCloseRegistration;

	#endregion

	#region Private Variables

	//Save Result
	private Button btnPrevClicked = null;

	//Game Properties
	ModeData mode_data = new ModeData();

	#endregion

	void Start (){
		if (!PlayerPrefs.HasKey ("Team"))
			SwitchTeam ();
	}

	void OnEnable(){
		//Single Mode
		btnSinglePlayer.onClick.AddListener (() => OnClick(btnSinglePlayer.GetInstanceID()));
		btnSingleEasy.onClick.AddListener (() => OnClick(btnSingleEasy.GetInstanceID()));
		btnSingleMedium.onClick.AddListener (() => OnClick(btnSingleMedium.GetInstanceID()));
		btnSingleHard.onClick.AddListener (() => OnClick(btnSingleHard.GetInstanceID()));
		btnBack.onClick.AddListener (() => OnClick(btnBack.GetInstanceID()));

		//Practice
		btnPractice.onClick.AddListener (() => OnClick(btnPractice.GetInstanceID()));

		//MultiOff Mode
		btnMultiPlayerOff.onClick.AddListener (() => OnClick(btnMultiPlayerOff.GetInstanceID()));

		//MultiOn Mode
		btnMultiPlayerOn.onClick.AddListener (() => OnClick(btnMultiPlayerOn.GetInstanceID()));

		//Exit Game Popup
		btnExitGame.onClick.AddListener (() => OnClick(btnExitGame.GetInstanceID()));
		btnResume.onClick.AddListener (() => OnClick(btnResume.GetInstanceID()));

		//Team
		btnColaTeam.onClick.AddListener (() =>SetPlayerTeam("Cola"));
		btnPepsiTeam.onClick.AddListener (() => SetPlayerTeam("Pepsi"));
		btnCloseChooseTeam.onClick.AddListener (() => SwitchTeam ());

		//Profile and Registration
		btnProfile.onClick.AddListener(() => ManagersController.Message(Message.Create(this, MessageData.EVENT_SHOW_PROFILE)));
		btnCloseProfile.onClick.AddListener(() => SwitchProfile ());
		btnCloseRegistration.onClick.AddListener(() => SwitchRegistration ());

		//Social
		btnLogin.onClick.AddListener(() => OnClick(btnLogin.GetInstanceID()));
		btnLogout.onClick.AddListener(() => OnClick(btnLogout.GetInstanceID()));

		//Notification
		btnCloseNotification.onClick.AddListener(() => SwitchNotification(""));

		//Leaderboards
		btnRatings.onClick.AddListener(() => OnClick(btnRatings.GetInstanceID()));
	}

	void OnClick(int instanceID){
		
		if (instanceID == btnPractice.GetInstanceID ()) {
			mode_data.mode = GameMode.Practice;
			mode_data.sub_mode = GameSubMode.None;
			StartGame (PlayerPrefs.GetString("Team", "Cola"));
		} 
		else if (instanceID == btnSinglePlayer.GetInstanceID ()) {
			mode_data.mode = GameMode.SinglePlayer;
			Invoke("HideMainMenu", 0.35f);
			Invoke("ShowSingleModeMenu", 0.75f);
		}
		else if (instanceID == btnMultiPlayerOff.GetInstanceID ()) {
			mode_data.mode = GameMode.MultiPlayer;
			mode_data.sub_mode = GameSubMode.None;
			StartGame(PlayerPrefs.GetString("Team", "Cola"));
		}
		else if (instanceID == btnMultiPlayerOn.GetInstanceID ()) {
			if (IsInternetAccessible ()) {
				mode_data.mode = GameMode.MultiplayerOnline;
				mode_data.sub_mode = GameSubMode.None;
				StartGame(PlayerPrefs.GetString("Team", "Cola"));
			}
		}
		else if (instanceID == btnSingleEasy.GetInstanceID ()) {
			mode_data.sub_mode = GameSubMode.Easy;
			StartGame(PlayerPrefs.GetString("Team", "Cola"));
		}
		else if (instanceID == btnSingleMedium.GetInstanceID ()) {
			mode_data.sub_mode = GameSubMode.Medium;
			StartGame(PlayerPrefs.GetString("Team", "Cola"));
		}
		else if (instanceID == btnSingleHard.GetInstanceID ()) {
			mode_data.sub_mode = GameSubMode.Hard;
			StartGame(PlayerPrefs.GetString("Team", "Cola"));
		}
		else if (instanceID == btnBack.GetInstanceID ()) {
			Invoke("HideSingleModeMenu", 0.35f);
			Invoke("ShowMainMenu", 0.75f);
		}
		else if (instanceID == btnExitGame.GetInstanceID ()) {

			if(GameManager.Instance.currentMode == GameMode.MultiplayerOnline)
				ManagersController.Message (Message.Create (this, MessageData.EVENT_TERMINATE_ONLINE_GAME));

			GameManager.Instance.FinishGame ();

			Time.timeScale = 1f;
			AnimatePopupClose (pnlExitGame);
			ShowMenu ();

		}
		else if (instanceID == btnResume.GetInstanceID ()) {
			Time.timeScale = 1f;
			AnimatePopupClose (pnlExitGame);
		}
		else if (instanceID == btnLogin.GetInstanceID ()) {
			if (IsInternetAccessible()) {
				SwitchLoading ("ИДЕТ ЗАГРУЗКА ДАННЫХ...");
				ManagersController.Message (Message.Create (this, MessageData.EVENT_LOGIN));
			}
		}
		else if (instanceID == btnLogout.GetInstanceID ()) {
			if (IsInternetAccessible()) {
				SwitchProfile ();
				ManagersController.Message (Message.Create (this, MessageData.EVENT_LOGOUT));
			}
		}
		else if (instanceID == btnRatings.GetInstanceID ()) {
			if (IsInternetAccessible()) {
				ManagersController.Message (Message.Create(this, MessageData.EVENT_SWITCH_LEADERBOARD));
			}
		}
	}

	#region Public Methods

	//SWITCHERS

	public void ShowMenu(){
		ShowBackground ();
		ShowMainMenu ();
		ShowBottomMenu ();
	}

	public void HideMenu(){
		if (pnlMenu.activeSelf)
			HideMainMenu ();
		if(pnlSingleMode.activeSelf)
			HideSingleModeMenu ();

		HideBottomMenu ();
		HideBackground ();

	}

	public void ShowBackground(){
		imgBG.DOFade (1f, 0.5f).OnPlay(() => imgBG.gameObject.SetActive(true));
	}

	public void HideBackground(){
		imgBG.DOFade (0f, 0.5f).OnComplete(() => imgBG.gameObject.SetActive(false));
	}

	public void ShowBottomMenu(){
		btnSettings.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-285f, -240f);
		btnProfile.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (285f, -240f);
		btnRatings.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, -150f);
		
		pnlBottomMenu.SetActive (true);

		btnSettings.GetComponent<RectTransform> ().DOAnchorPosY (178f, 1f).SetEase (Ease.OutElastic, 0.1f);
		btnRatings.GetComponent<RectTransform> ().DOAnchorPosY (267f, 1.25f).SetEase (Ease.OutElastic, 0.1f);
		btnProfile.GetComponent<RectTransform> ().DOAnchorPosY (178f, 1.5f).SetEase (Ease.OutElastic, 0.1f);
	}

	public void HideBottomMenu(){
		btnSettings.GetComponent<RectTransform> ().DOAnchorPosY (-240f, 0.2f).SetEase (Ease.InBack);
		btnRatings.GetComponent<RectTransform> ().DOAnchorPosY (-150f, 0.25f).SetEase (Ease.InBack);
		btnProfile.GetComponent<RectTransform> ().DOAnchorPosY (-240f, 0.3f).SetEase (Ease.InBack)
			.OnComplete(() => pnlBottomMenu.SetActive (false));
	}

	public void ShowMainMenu(){
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

	public void HideMainMenu(){
		btnSinglePlayer.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.2f).SetEase (Ease.InBack);
		btnMultiPlayerOn.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.25f).SetEase (Ease.InBack);
		btnMultiPlayerOff.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.3f).SetEase (Ease.InBack);
		btnPractice.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.35f).SetEase (Ease.InBack)
			.OnComplete( () => pnlMenu.SetActive (false));
	}

	public void ShowSingleModeMenu(){
		btnBack.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, 330f);
		btnSingleEasy.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, 115f);
		btnSingleMedium.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, -115f);
		btnSingleHard.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-900f, -330f);

		pnlSingleMode.SetActive (true);

		btnBack.GetComponent<RectTransform> ().DOAnchorPosX (0f, 0.75f).SetEase (Ease.OutElastic, 0.1f);
		btnSingleEasy.GetComponent<RectTransform> ().DOAnchorPosX (0f, 1f).SetEase (Ease.OutElastic, 0.1f);
		btnSingleMedium.GetComponent<RectTransform> ().DOAnchorPosX (0f, 1.25f).SetEase (Ease.OutElastic, 0.1f);
		btnSingleHard.GetComponent<RectTransform> ().DOAnchorPosX (0f, 1.5f).SetEase (Ease.OutElastic, 0.1f);
	}

	public void HideSingleModeMenu(){
		btnBack.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.15f).SetEase (Ease.InBack);
		btnSingleEasy.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.2f).SetEase (Ease.InBack);
		btnSingleMedium.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.25f).SetEase (Ease.InBack);
		btnSingleHard.GetComponent<RectTransform> ().DOAnchorPosX (900f, 0.3f).SetEase (Ease.InBack)
			.OnComplete(() => pnlSingleMode.SetActive (false));
	}

	public void SwitchNotification(string notification){
		if (!pnlNotification.activeSelf) {
			txtNotification.text = notification;
			AnimatePopupOpen (pnlNotification);
		}
		else
			AnimatePopupClose (pnlNotification);
	}

	public void SwitchLoading(string description = ""){
		if (!pnlWait.activeSelf) {
			txtWaitDescription.text = description;
			pnlWait.SetActive (true);
		}
		else
			pnlWait.SetActive (false);
	}

	public void SwitchProfile(){
		if(!pnlProfile.activeSelf)
			AnimatePopupOpen (pnlProfile);
		else
			AnimatePopupClose (pnlProfile);
	}

	public void SwitchRegistration(){
		if(!pnlRegistration.activeSelf)
			AnimatePopupOpen (pnlRegistration);
		else
			AnimatePopupClose (pnlRegistration);
	}

	public void SwitchTeam(){
		if(!pnlChooseTeam.activeSelf)
			AnimatePopupOpen (pnlChooseTeam);
		else
			AnimatePopupClose (pnlChooseTeam);
	}

	//CALLBACKS

	public void OnLoginResult(bool isSucceeded){
		SwitchLoading ();
		SwitchRegistration ();

		if (isSucceeded)
			ManagersController.Message (Message.Create (this, MessageData.EVENT_SHOW_PROFILE));
		else {
			string notification = "РЕГИСТРАЦИЯ НЕ УДАЛАСЬ";
			SwitchNotification (notification);
		}
	}

	public void OnLoginResult2(bool isSucceeded){
		SwitchLoading ();

		if (!isSucceeded){
			string notification = "РЕГИСТРАЦИЯ НЕ УДАЛАСЬ";
			SwitchNotification (notification);
		}
	}

	public void OnMatchResult(bool isMathced){
		SwitchLoading ();

		if (isMathced) {
			ManagersController.Message (Message.Create (this, MessageData.EVENT_START_GAME,mode_data));
			if(imgBG.color.a > 0f)
				HideMenu ();
		} 
		else {
			string notification = "ПРОТИВНИК НЕ НАЙДЕН\nПОПЫТАЙТЕСЬ ПОЗЖЕ";
			SwitchNotification (notification);
		}
	}

	//EVENTS

	public void SetProfile(string name, Texture2D picture){
		txtName.text = name;

		Sprite photo = picture == null ? null : Sprite.Create (picture, new Rect (0f, 0f, picture.width, picture.height), new Vector2 (0.5f, 0.5f));
		if(photo != null) imgProfilePicture.sprite = photo;

		txtScore.text = PlayerPrefs.GetInt ("Score", 0).ToString ();
	}

	//POPUPS
	public void AnimatePopupOpen(GameObject popup){
		popup.transform.GetChild (0).localScale = Vector2.zero;
		popup.SetActive (true);
		popup.transform.GetChild (0).DOScale (new Vector2(1f, 1f), 0.25f).SetEase (Ease.OutBack);
	}

	public void AnimatePopupClose(GameObject popup){
		popup.transform.GetChild (0).DOScale (Vector2.zero, 0.25f).SetEase (Ease.InBack)
			.OnComplete(() => popup.SetActive(false));
	}

	public void StartGame(String team){

		Team _team = Team.Cola;
		if (team == "Cola")
			_team = Team.Cola;
		if (team == "Pepsi")
			_team = Team.Pepsi;

		mode_data.teamPlayer = _team;
		mode_data.teamEnemy = _team == Team.Cola ? Team.Pepsi : Team.Cola;

		switch (mode_data.mode) {
		case GameMode.Practice:
			ManagersController.Message (Message.Create (this, MessageData.EVENT_START_GAME, mode_data));
			if(imgBG.color.a > 0f)
				HideMenu ();
			break;
		case GameMode.MultiplayerOnline:
			ManagersController.Message (Message.Create (this, MessageData.EVENT_FIND_MATCH, _team));
			break;
		case GameMode.MultiPlayer:
			ManagersController.Message (Message.Create (this, MessageData.EVENT_START_GAME, mode_data));
			if(imgBG.color.a > 0f)
				HideMenu ();
			break;
		case GameMode.SinglePlayer:
			ManagersController.Message (Message.Create (this, MessageData.EVENT_START_GAME, mode_data));
			if(imgBG.color.a > 0f)
				HideMenu ();
			break;
		}

	}

	public bool IsInternetAccessible(){
		#if UNITY_EDITOR
		return true;
		#endif

		if (Application.internetReachability == NetworkReachability.NotReachable) {
			string notification = "ПРОВЕРЬТЕ ИНТЕРНЕТ ПОДКЛЮЧЕНИЕ";
			SwitchNotification (notification);
			return false;
		}
		else
			return true;
	}

	#endregion

	void SetPlayerTeam(String team){
		PlayerPrefs.SetString ("Team", team);

		if (pnlChooseTeam.activeSelf)
			SwitchTeam ();
	}
}

