using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;

public class FacebookManager : MonoBehaviour {

	public static FacebookManager Instance {
		get;
		private set;
	}

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		} else {
			Instance = this;
		}
	}

	//=============================================

	void Start(){
		if (!FB.IsInitialized) {
			FB.Init(InitCallback, OnHideUnity);
		} else {
			FB.ActivateApp();
		}
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			FB.ActivateApp();
			if (FB.IsLoggedIn) {
				Init ();
			}
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	//=======================================================

	private FB_UserInfo _userInfo = null;

	public FB_UserInfo UserInfo{
		get{ return _userInfo;}
	}

	//=======================================================

	public event Action<bool> OnFacebookProfileLoadResult = delegate {};

	//=======================================================

	public void Init(){
		LoadUserInfo ();
	}

	//USER INFO

	private void LoadUserInfo(){
		FB.API("/me?fields=id,name,first_name,last_name,picture&type=square", HttpMethod.GET, UserDataCallBack);
	}

	private void UserDataCallBack(IGraphResult result){
		if (result.Error != null) {
			Debug.Log ("Facebook Manager: " + result.Error);
			OnFacebookProfileLoadResult (false);
		}
		else
			ParseUserData (result.ResultDictionary);
	}

	private void ParseUserData(IDictionary<string,object> data){
		try{
			_userInfo = new FB_UserInfo(data);
			StartCoroutine("LoadProfileTexture");
		}
		catch(Exception e){
			Debug.LogWarning("Parceing User Data failed");
			Debug.LogWarning(e.Message);
			OnFacebookProfileLoadResult (false);
		}
	}

	IEnumerator LoadProfileTexture(){
		// Start a download of the given URL
		WWW www = new WWW(_userInfo.PictureURL);

		// Wait for download to complete
		yield return www;

		UserInfo.Picture = www.texture;

		ManagersController.Message (Message.Create (this, MessageData.EVENT_CONNECT_SERVER, GetAccessToken()));
		OnFacebookProfileLoadResult (true);
	}

	//LOGIN

	public void FacebookLogin(){
		var perms = new List<string>(){"public_profile"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			Init ();
		} 
		else {
			Debug.Log("Facebook Manager: " + result.RawResult);
			OnFacebookProfileLoadResult (false);
		}
	}

	//LOGOUT

	public void FacebookLogout(){
		Debug.Log ("Facebook Manager: Player is logged out!");
		FB.LogOut();
	}

	//PUBLIC METHODS

	public bool IsLoggedIn(){
		return FB.IsLoggedIn;
	}

	public string GetAccessToken(){
		return Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
	}

}
