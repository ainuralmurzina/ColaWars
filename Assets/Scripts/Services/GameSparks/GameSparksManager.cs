using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.RT;
using System;

public class GameSparksManager : MonoBehaviour {

	#region Instance Intialization
	public static GameSparksManager Instance = null;
	void Awake() {
		if (Instance == null) 
		{
			Instance = this; 
			DontDestroyOnLoad(this.gameObject); 
		} else 
		{
			Destroy(this.gameObject);
		}
	}
	#endregion

	#region Player
	public string playerID { get; private set;}
	public string playerName { get; private set;}
	#endregion

	#region Session Variables
	public GameSparksRTUnity session { get; private set;}
	public RTSessionInfo session_info { get; private set;}
	#endregion

	#region Authentication
	public void FacebookAuthentication(string accessToken){
		new FacebookConnectRequest ()
			.SetAccessToken (accessToken)
			.Send (response => FacebookAuthenticationCallback (response));
	}

	void FacebookAuthenticationCallback(AuthenticationResponse response){
		if (!response.HasErrors) {
			playerID = response.UserId;
			playerName = response.DisplayName;
		} 
		else {
			Debug.Log ("GameSparksManager: Error: " + response.Errors.JSON);
		}
	}
	#endregion

	#region Matching

	public event Action<bool> OnMatchResult = delegate {};

	public void FindMatch(Team choosedTeam){

		//Send Request
		if (choosedTeam == Team.Cola) {
			new MatchmakingRequest ()
				.SetMatchShortCode ("DEFAULT_MCH")
				.SetSkill(1)
				.SetParticipantData( new GSRequestData ().AddString("team", "cola"))
				.SetCustomQuery( new GSRequestData ().AddString ("players.participantData.team", "pepsi"))
				.Send (response => FindMatchCallback (response));
		} 
		else {
			new MatchmakingRequest ()
				.SetMatchShortCode ("DEFAULT_MCH")
				.SetSkill(1)
				.SetParticipantData( new GSRequestData ().AddString("team", "pepsi"))
				.SetCustomQuery( new GSRequestData ().AddString ("players.participantData.team", "cola"))
				.Send (response => FindMatchCallback (response));
		}

		//Listeners
		MatchNotFoundMessage.Listener += MatchNotFound;
		MatchFoundMessage.Listener += MatchFound;

	}

	void FindMatchCallback(MatchmakingResponse response){
		if (response.HasErrors) {
			Debug.Log ("GameSparksManager: Matchmaking result: Error: " + response.Errors.JSON);
			OnMatchResult (false);
		}
	}

	void MatchNotFound(MatchNotFoundMessage message){
		OnMatchResult (false);
	}

	void MatchFound(MatchFoundMessage message){
		session_info = new RTSessionInfo (message);
		ManagersController.Message (Message.Create (this, MessageData.EVENT_SET_ONLINE_PLAYERS, session_info.players));

		#region Create Session

		session = this.gameObject.GetComponent<GameSparksRTUnity> ();

		GSRequestData mockedResponse = new GSRequestData ()
			.AddNumber ("port", session_info.PortID)
			.AddString ("host", session_info.HostURL)
			.AddString ("accessToken", session_info.AccessToken);
		FindMatchResponse response = new FindMatchResponse (mockedResponse);

		session.Configure (
			response,
			peerID => OnPlayerConnected(peerID),
			peerID => OnPlayerDisconnected(peerID),
			ready => OnRTReady(ready),
			packet => OnPacketRecieved(packet)
		);

		session.Connect ();

		#endregion
	}

	#endregion

	#region Session Methods

	void OnPlayerConnected(int peerID){
	}

	void OnPlayerDisconnected(int peerID){
		Invoke ("TerminateSession", 5f);
	}

	void TerminateSession(){
		EndSession ();
		ManagersController.Message (Message.Create (this, MessageData.EVENT_ONLINE_GAME_TERMINATED));
	}

	void OnRTReady(bool isReady){
	}

	#endregion

	#region Score

	private void ScoreSubmissionCallback(LogEventResponse response){
		if (response.HasErrors)
			Debug.Log ("GameSparksManager: Error: " + response.Errors.JSON);
		else
			//Update leaderboard
			LeaderboardRequest ();
	}

	private void WinnedTeamSubmissionCallback(LogEventResponse response){
		if (response.HasErrors)
			Debug.Log ("GameSparksManager: Error: " + response.Errors.JSON);
	}

	private void TeamScoreCallback(LogEventResponse response){
		if (response.HasErrors)
			Debug.Log ("GameSparksManager: Error: " + response.Errors.JSON);
		else {
			GSData dataCola = response.ScriptData.GetGSData ("ColaData");
			GSData dataPepsi = response.ScriptData.GetGSData ("PepsiData");

			int counterCola = 0;
			if (dataCola.ContainsKey ("count"))
				counterCola = dataCola.GetInt ("count").Value;

			int counterPepsi = 0;
			if (dataPepsi.ContainsKey ("count"))
				counterPepsi = dataPepsi.GetInt ("count").Value;

			ManagersController.Message (Message.Create (this, MessageData.EVENT_GET_TEAMS_SCORE, counterCola, counterPepsi));
		}
	}

	private void OverrallLeaderboardCallback(LeaderboardDataResponse response){
		if (response.HasErrors) 
			Debug.Log ("GameSparksManager: Error: " + response.Errors.JSON);
		else {
			List<GlobalScore> overall_leaderboard = new List<GlobalScore> ();
			foreach (LeaderboardDataResponse._LeaderboardData data in response.Data) {
				string displayName = data.UserName;
				string score = data.JSONData ["SCORE"].ToString ();
				string team = "Cola";
				if(data.JSONData.ContainsKey("TEAM"))
					team = data.JSONData ["TEAM"].ToString();
				overall_leaderboard.Add (new GlobalScore (displayName, score, team));
			}
			if (overall_leaderboard.Count != 0)
				ManagersController.Message (Message.Create (this, MessageData.EVENT_LEADERBOARD_OVERALL, overall_leaderboard));
		}
	}

	private void DailyLeaderboardCallback(LeaderboardDataResponse response){
		if (response.HasErrors) 
			Debug.Log ("GameSparksManager: Error: " + response.Errors.JSON);
		else {
			List<GlobalScore> monthly_leaderboard = new List<GlobalScore> ();
			foreach (LeaderboardDataResponse._LeaderboardData data in response.Data) {
				string displayName = data.UserName;
				string score = data.JSONData ["SCORE"].ToString ();
				string team = "Cola";
				if(data.JSONData.ContainsKey("TEAM"))
					team = data.JSONData ["TEAM"].ToString();
				monthly_leaderboard.Add (new GlobalScore (displayName, score, team));
			}
			if (monthly_leaderboard.Count != 0)
				ManagersController.Message (Message.Create (this, MessageData.EVENT_LEADERBOARD_MONTH, monthly_leaderboard));
		}
	}

	private void WeeklyLeaderboardCallback(LeaderboardDataResponse response){
		if (response.HasErrors) 
			Debug.Log ("GameSparksManager: Error: " + response.Errors.JSON);
		else {
			List<GlobalScore> weekly_leaderboard = new List<GlobalScore> ();
			foreach (LeaderboardDataResponse._LeaderboardData data in response.Data) {
				string displayName = data.UserName;
				string score = data.JSONData ["SCORE"].ToString ();
				string team = "Cola";
				if(data.JSONData.ContainsKey("TEAM"))
					team = data.JSONData ["TEAM"].ToString();
				weekly_leaderboard.Add (new GlobalScore (displayName, score, team));
			}
			if (weekly_leaderboard.Count != 0)
				ManagersController.Message (Message.Create (this, MessageData.EVENT_LEADERBOARD_DAY, weekly_leaderboard));
		}
	}

	#endregion

	#region Packet Methods

	void OnPacketRecieved(RTPacket packet){

		switch (packet.OpCode) {
		//100 => All users are connected to session
		case 100:
			OnMatchResult (true);
			break;
			//101 => Ring Info
		case 101:
			PacketRecieved_Ring (packet);
			break;
		case 102:
			PacketRecieved_Field (packet);
			break;
		}
	}

	void PacketRecieved_Ring(RTPacket rt_packet){
		OnlinePacket packet = new OnlinePacket (
			1,
			new Vector3(rt_packet.Data.GetVector3(1).Value.x, rt_packet.Data.GetVector3(1).Value.y, rt_packet.Data.GetVector3(1).Value.z),
			new Vector3(rt_packet.Data.GetVector3(2).Value.x, rt_packet.Data.GetVector3(2).Value.y, rt_packet.Data.GetVector3(2).Value.z),
			new Vector3(rt_packet.Data.GetVector3(3).Value.x, rt_packet.Data.GetVector3(3).Value.y, rt_packet.Data.GetVector3(3).Value.z)
		);
		ManagersController.Message (Message.Create (this, MessageData.EVENT_RECIEVE_PACKET,packet));
	}

	void PacketRecieved_Field(RTPacket rt_packet){
		OnlinePacket packet = new OnlinePacket (
			2,
			new Score((int) rt_packet.Data.GetInt(1), (int) rt_packet.Data.GetInt(2)),
			(string) rt_packet.Data.GetString (3),
			(string) rt_packet.Data.GetString (4)
		);
		ManagersController.Message (Message.Create (this, MessageData.EVENT_RECIEVE_PACKET,packet));
	}

	#endregion

	#region Public Methods

	public void EndSession(){
		if(session != null)
			session.Disconnect ();
	}

	public bool IsAuthenticated(){
		if (GS.Authenticated)
			return true;
		else
			return false;
	}

	public void SendPacket_Ring(OnlinePacket packet){
		using (RTData data = RTData.Get ()) {
			data.SetVector3 (1, packet.position);
			data.SetVector3 (2, packet.rotation);
			data.SetVector3 (3, packet.velocity);
			session.SendData (101, GameSparksRT.DeliveryIntent.RELIABLE, data);
		}
	}

	public void SendPacket_Field(OnlinePacket packet){
		using (RTData data = RTData.Get ()) {
			data.SetInt (1, packet.cola_score);
			data.SetInt (2, packet.pepsi_score);
			data.SetString (3, packet.cola_bottles);
			data.SetString (4, packet.pepsi_bottles);
			session.SendData (102, GameSparksRT.DeliveryIntent.RELIABLE, data);
		}
	}

	public void SubmitWinnedTeam(){

		string key = "COLA_WIN";
		string team = PlayerPrefs.GetString ("Team", "Cola");
		if(team == "Cola")
			key = "COLA_WIN";
		else
			key = "PEPSI_WIN";

		new LogEventRequest ()
			.SetEventKey ("SUBMIT_GAME_RESULT")
			.SetEventAttribute (key, 1)
			.Send (response => WinnedTeamSubmissionCallback(response));
	}

	public void SubmitScore(int score){
		new LogEventRequest ()
			.SetEventKey ("SUBMIT_SCORE")
			.SetEventAttribute ("SCORE", score)
			.SetEventAttribute ("TEAM", PlayerPrefs.GetString("Team", "Cola"))
			.Send (response => ScoreSubmissionCallback(response));
	}

	public void LeaderboardRequest(){
		new LeaderboardDataRequest ()
			.SetLeaderboardShortCode ("OVERALL_LEADERBOARD")
			.SetEntryCount (100)
			.Send (response => OverrallLeaderboardCallback(response));

		string month = System.DateTime.Now.Month.ToString ();
		if (month.Length < 2)
			month = "0" + month;
		
		new LeaderboardDataRequest ()
			.SetLeaderboardShortCode ("MONTHLY_LEADERBOARD.MONTH." + System.DateTime.Now.Year.ToString() + month)
			.SetEntryCount (100)
			.Send (response => DailyLeaderboardCallback(response));

		string day = System.DateTime.Now.Day.ToString ();
		if (day.Length < 2)
			day = "0" + day;
		new LeaderboardDataRequest ()
			.SetLeaderboardShortCode ("DAILY_LEADERBOARD.DAY." + System.DateTime.Now.Year.ToString () + month + day)
			.SetEntryCount (100)
			.Send (response => WeeklyLeaderboardCallback (response));

		new LeaderboardDataRequest ()
			.SetLeaderboardShortCode ("DAILY_LEADERBOARD.DAY." + System.DateTime.Now.Year.ToString () + month + day)
			.SetEntryCount (100)
			.Send (response => WeeklyLeaderboardCallback (response));

		new LogEventRequest ()
			.SetEventKey ("GET_TEAMS_SCORE")
			.Send (response => TeamScoreCallback(response));
	}

	#endregion

}
