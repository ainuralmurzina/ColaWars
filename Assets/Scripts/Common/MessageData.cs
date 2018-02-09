using UnityEngine;
using System.Collections;

public class MessageData
{
	//SOCIAL
	public const int EVENT_LOGIN = 101;
	public const int EVENT_LOGOUT = 102;
	public const int EVENT_SHOW_PROFILE = 103;
	public const int EVENT_LOGIN2 = 104;

	//ONLINE GAME
	public const int EVENT_CONNECT_SERVER = 201;
	public const int EVENT_FIND_MATCH = 202;
	public const int EVENT_SET_ONLINE_PLAYERS = 203;
	public const int EVENT_TERMINATE_ONLINE_GAME = 204;
	public const int EVENT_SEND_PACKET = 205; 
	public const int EVENT_RECIEVE_PACKET = 206;
	public const int EVENT_ONLINE_GAME_TERMINATED = 207;

	//LEADERBOARDS
	public const int EVENT_LEADERBOARD_OVERALL = 401;
	public const int EVENT_LEADERBOARD_MONTH = 402;
	public const int EVENT_LEADERBOARD_DAY = 403;
	public const int EVENT_LEADERBOARD_UPDATE = 404;
	public const int EVENT_SUBMIT_SCORE = 405;
	public const int EVENT_SUBMIT_WINNED_TEAM = 406;
	public const int EVENT_GET_TEAMS_SCORE = 407;

	//GAME
	public const int EVENT_START_GAME = 301;
	public const int EVENT_PAUSE_GAME = 302;
	public const int EVENT_FINISH_GAME = 303;
	public const int EVENT_PLAY_AGAIN = 304;

	//MENU
	public const int EVENT_SHOW_MENU = 501;
	public const int EVENT_SWITCH_LEADERBOARD = 502;
	public const int EVENT_SAVE_PLAYER_TEAM = 503;
}

