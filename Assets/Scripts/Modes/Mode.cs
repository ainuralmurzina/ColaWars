using System;
using UnityEngine;

public class Mode
{
	public Player player;

	public virtual void CreateGame (){}
	public virtual void DestroyGame (){}
	public virtual Player CheckTurn (){

		return null;
	}
	public virtual void ActivatePlayers (){}


}

