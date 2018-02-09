using UnityEngine;
using System.Collections;
using GameSparks.Api.Messages;
using System.Collections.Generic;

public class RTSessionInfo
{
	private string 	_hostURL 		= "";
	private string 	_accessToken 	= "";
	private int 	_portID 		= 0;
	private string 	_matchID 		= "";
	private List<RTPlayer> _players = new List<RTPlayer> ();

	//=============================

	public string HostURL{
		get { return _hostURL;}
	}

	public string AccessToken{
		get { return _accessToken;}
	}

	public int PortID{
		get { return _portID;}
	}

	public string MatchID{
		get { return _matchID;}
	}

	public List<RTPlayer> players{
		get{ return _players;}
	}

	//=============================

	public RTSessionInfo(MatchFoundMessage message){
		_portID 		= (int)message.Port;
		_hostURL 		= message.Host;
		_accessToken 	= message.AccessToken;
		_matchID 		= message.MatchId;

		foreach (MatchFoundMessage._Participant p in message.Participants)
			_players.Add (new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
	}
}

