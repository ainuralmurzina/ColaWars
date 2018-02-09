using System;

public class PracticeMode :  Mode
{

	//Methods
	public override void CreateGame(){

		//Create Player
		player = new Player ();

		//Set Player according to its team
		PlayerManager.Instance.SetPlayerAttributs(player);

		//Create bottles
		FieldManager.Instance.ActivateBottles_OneRow(player);

		//Set Player Position
		PlayerManager.Instance.SetPlayerPosition(player, "middle", false);

		//Activate players
		ActivatePlayers ();

		//Set Camera
		CameraManager.Instance.ShowPlayer(3f, player);

		//Set current player
		PlayerManager.Instance.currentPlayer = player.team;
		PlayerManager.Instance.isBot = false;
		PlayerManager.Instance.isEnemy = false;
		GameManager.Instance.SetCurrentPlayer(player);
	}

	public override void DestroyGame(){
		
		//Reset current player
		PlayerManager.Instance.currentPlayer = Team.None;
		PlayerManager.Instance.isBot = false;

		//Disable player
		PlayerManager.Instance.SwitchPlayer (player, false, false);

		//Disable bottles
		FieldManager.Instance.DisactivateAllBottles ();

		//Reset ring
		player.ring.GetComponent<Ring> ().ResetRing ();
	}

	public override void ActivatePlayers ()
	{
		PlayerManager.Instance.SwitchPlayer (player, false, true);
	}

	public override Player CheckTurn(){
		PlayerManager.Instance.SetPlayerPosition (player, player.currentRow, false);
		CameraManager.Instance.ShowPlayer (1f, player);

		//Info
		PlayerManager.Instance.currentPlayer = player.team;
		PlayerManager.Instance.isBot = false;
		PlayerManager.Instance.isEnemy = false;

		return player;
	}
}

