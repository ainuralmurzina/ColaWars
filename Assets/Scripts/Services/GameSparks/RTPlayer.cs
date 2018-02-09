using UnityEngine;
using System.Collections;

public class RTPlayer
{
	public string 	displayName;
	public string 	id;
	public int 		peerID;
	public bool 	isOnline;

	public RTPlayer(string displayName, string id, int peerID){
		this.displayName 	= displayName;
		this.id 			= id;
		this.peerID 		= peerID;
	}
}

