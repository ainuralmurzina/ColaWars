using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ManagersController : MonoBehaviour {

	public static void Message(Message msg){

		if (msg.id == MessageData.EVENT_LOGIN) {
			FacebookManager.Instance.OnFacebookProfileLoadResult += OnLoginResult;
			FacebookManager.Instance.FacebookLogin ();
		}
		if (msg.id == MessageData.EVENT_LOGIN2) {
			LoginToSaveResult ();
		} else if (msg.id == MessageData.EVENT_LOGOUT) {
			FacebookManager.Instance.FacebookLogout ();
		} else if (msg.id == MessageData.EVENT_SHOW_PROFILE) {
			CheckSocialAvailability ();
		} else if (msg.id == MessageData.EVENT_CONNECT_SERVER) {
			GameSparksManager.Instance.FacebookAuthentication ((string)msg.data);
		} else if (msg.id == MessageData.EVENT_FIND_MATCH) {
			CheckServerAvailability ((Team)msg.data);
		} else if (msg.id == MessageData.EVENT_SET_ONLINE_PLAYERS) {
			SetOnlineGame (msg.data as List<RTPlayer>);
		} else if (msg.id == MessageData.EVENT_TERMINATE_ONLINE_GAME) {
			GameSparksManager.Instance.EndSession ();
		} else if (msg.id == MessageData.EVENT_ONLINE_GAME_TERMINATED) {
			TerminateGame ();
		} else if (msg.id == MessageData.EVENT_SEND_PACKET) {
			SendPacket ((OnlinePacket)msg.data);
		} else if (msg.id == MessageData.EVENT_RECIEVE_PACKET) {
			RecievePacket ((OnlinePacket)msg.data);
		} else if (msg.id == MessageData.EVENT_START_GAME) {
			ModeData mode_data = msg.data as ModeData;
			GameManager.Instance.StartGame (mode_data);
		} else if (msg.id == MessageData.EVENT_LEADERBOARD_UPDATE) {
			GameSparksManager.Instance.LeaderboardRequest ();
		} else if (msg.id == MessageData.EVENT_LEADERBOARD_OVERALL) {
			LeaderboardsView.Instance.UpdateOverall ((List<GlobalScore>)msg.data);
		} else if (msg.id == MessageData.EVENT_LEADERBOARD_MONTH) {
			LeaderboardsView.Instance.UpdateMonth ((List<GlobalScore>)msg.data);
		} else if (msg.id == MessageData.EVENT_LEADERBOARD_DAY) {
			LeaderboardsView.Instance.UpdateDay ((List<GlobalScore>)msg.data);
		} else if (msg.id == MessageData.EVENT_SWITCH_LEADERBOARD) {
			LeaderboardsView.Instance.SwitchRating ();
		} else if (msg.id == MessageData.EVENT_SUBMIT_SCORE) {
			GameSparksManager.Instance.SubmitScore ((int)msg.data);
		} else if (msg.id == MessageData.EVENT_FINISH_GAME) {
			ResultView.Instance.SwitchResult ((bool)msg.data, PlayerManager.Instance.player, PlayerManager.Instance.enemy);
			GameManager.Instance.FinishGame ();
			GameSparksManager.Instance.EndSession ();
		} else if (msg.id == MessageData.EVENT_SHOW_MENU) {
			MenuManager.Instance.ShowMenu ();
		} else if (msg.id == MessageData.EVENT_PAUSE_GAME) {
			MenuManager.Instance.AnimatePopupOpen (MenuManager.Instance.pnlExitGame);
		} else if (msg.id == MessageData.EVENT_PLAY_AGAIN) {

			string _team = "Cola";
			Team team = PlayerManager.Instance.player.team;
			if (team == Team.Cola)
				_team = "Cola";
			if (team == Team.Pepsi)
				_team = "Pepsi";

			MenuManager.Instance.StartGame (_team);
		}
		else if (msg.id == MessageData.EVENT_SUBMIT_WINNED_TEAM) {
			GameSparksManager.Instance.SubmitWinnedTeam ();
		}
		else if (msg.id == MessageData.EVENT_GET_TEAMS_SCORE) {
			LeaderboardsView.Instance.UpdateTeamsScore (msg.data.ToString(), msg.data2.ToString());
		}
	}

	static void CheckSocialAvailability(){
		if (FacebookManager.Instance.IsLoggedIn ()) {
			string name = FacebookManager.Instance.UserInfo.FirstName + "\n" + FacebookManager.Instance.UserInfo.LastName;
			Texture2D picture = FacebookManager.Instance.UserInfo.Picture;
			MenuManager.Instance.SetProfile (name, picture);
			MenuManager.Instance.SwitchProfile ();
		}
		else
			MenuManager.Instance.SwitchRegistration ();
	}

	static void CheckServerAvailability(Team choosedTeam){
		if (GameSparksManager.Instance.IsAuthenticated () && FacebookManager.Instance.IsLoggedIn()) {

			//Notify user about match state
			string comments = "ИДЕТ ПОИСК ПРОТИВНИКА...";
			MenuManager.Instance.SwitchLoading (comments);

			//Start matching
			GameSparksManager.Instance.OnMatchResult += OnMatchResult;
			GameSparksManager.Instance.FindMatch (choosedTeam);
		} 
		else {
			string notification = "ПОЖАЛУЙСТА СНАЧАЛА АВТОРИЗУЙТЕСЬ";
			MenuManager.Instance.SwitchNotification (notification);
		}
	}

	static void SendPacket(OnlinePacket packet){
		if (packet.id == 1) {
			GameSparksManager.Instance.SendPacket_Ring (packet);
		}
		if (packet.id == 2) {
			GameSparksManager.Instance.SendPacket_Field (packet);
		}
	}

	static void RecievePacket(OnlinePacket packet){
		if (packet.id == 1) {
			GameManager.Instance.SetOnlineShoot (
				packet.position,
				packet.rotation,
				packet.velocity
			);
		}
		if (packet.id == 2) {

			List<string> temp;

			List<bool> cola_bottles = new List<bool> ();
			temp = packet.cola_bottles.Split (',').ToList ();
			foreach (string s in temp) {
				if (s == "1")
					cola_bottles.Add (true);
				else if (s == "0")
					cola_bottles.Add (false);
			}

			List<bool> pepsi_bottles = new List<bool> ();
			temp = packet.pepsi_bottles.Split (',').ToList ();
			foreach (string s in temp) {
				if (s == "1")
					pepsi_bottles.Add (true);
				else if (s == "0")
					pepsi_bottles.Add (false);
			}

			GameManager.Instance.SetOnlineScore (new Score(packet.cola_score, packet.pepsi_score), cola_bottles, pepsi_bottles);
		}
	}

	static void SetOnlineGame(List<RTPlayer> players){

		string playerName, enemyName;

		if (players [0].id == GameSparksManager.Instance.playerID) {
			playerName = players [0].displayName;
			enemyName = players [1].displayName;
		}
		else{
			playerName = players [1].displayName;
			enemyName = players [0].displayName;
		}

		//MenuManager.Instance.SetGamePlayers (playerName, enemyName);
	}

	static void LoginToSaveResult(){
		if (MenuManager.Instance.IsInternetAccessible()) {
			MenuManager.Instance.SwitchLoading ("ИДЕТ ЗАГРУЗКА ДАННЫХ...");
			FacebookManager.Instance.OnFacebookProfileLoadResult += OnLoginResult2;
			FacebookManager.Instance.FacebookLogin ();
		}
	}

	static void TerminateGame () {
		if (GameManager.Instance.isOnlineGameActive) {
			GameManager.Instance.FinishGame ();
			MenuManager.Instance.ShowMenu ();

			string notification = "ПРОТИВНИК ПРЕЖДЕВРЕМЕННО ПОКИНУЛ ИГРУ";
			MenuManager.Instance.SwitchNotification (notification);
		}
	}

	//CALLBACKS
	static void OnLoginResult(bool isSucceeded){
		FacebookManager.Instance.OnFacebookProfileLoadResult -= OnLoginResult;
		MenuManager.Instance.OnLoginResult (isSucceeded);
	}

	static void OnLoginResult2(bool isSucceeded){
		FacebookManager.Instance.OnFacebookProfileLoadResult -= OnLoginResult2;
		MenuManager.Instance.OnLoginResult2 (isSucceeded);
		ResultView.Instance.OnRegistrationResult (isSucceeded);
	}

	static void OnMatchResult(bool isMatched){
		GameSparksManager.Instance.OnMatchResult -= OnMatchResult;
		MenuManager.Instance.OnMatchResult (isMatched);
	}
}
