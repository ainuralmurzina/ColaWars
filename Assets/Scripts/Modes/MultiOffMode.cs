using UnityEngine;
using System.Collections;

public class MultiOffMode : Mode
{
	public Player player2;

	private Team teamPlayer1;
	private Team teamPlayer2;

	//Constructor
	public MultiOffMode( Team teamPlayer1, Team teamPlayer2){
		this.teamPlayer1 = teamPlayer1;
		this.teamPlayer2 = teamPlayer2;
	}

	//Methods
	public override void CreateGame(){

		//Create Players
		player = new Player (teamPlayer1, "ВЫ");
		player2 = new Player (teamPlayer2, "СОПЕРНИК");

		player.currentID = 1;
		player2.currentID = 2;

		PlayerManager.Instance.player = player;
		PlayerManager.Instance.enemy = player2;

		//Set Player Attributes according to its team
		PlayerManager.Instance.SetPlayerAttributs(player);
		PlayerManager.Instance.SetPlayerAttributs(player2);

		//Create Bottles
		FieldManager.Instance.ActivateBottles_FiveRows(player);
		FieldManager.Instance.ActivateBottles_FiveRows(player2);

		//Set Player Position
		PlayerManager.Instance.SetPlayerPosition(player, "most_left", false);
		PlayerManager.Instance.SetPlayerPosition(player2, "most_left", false);

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

		//Disable players
		PlayerManager.Instance.SwitchPlayer (player, false, false);
		PlayerManager.Instance.SwitchPlayer (player2, false, false);

		//Disable bottles
		FieldManager.Instance.DisactivateAllBottles ();

		//Reset rings
		player.ring.GetComponent<Ring> ().ResetRing ();
		player2.ring.GetComponent<Ring> ().ResetRing ();
	}

	public override void ActivatePlayers ()
	{
		PlayerManager.Instance.SwitchPlayer (player, false, true);
		PlayerManager.Instance.SwitchPlayer (player2, false, true);
	}

	public override Player CheckTurn(){

		Player currentPlayer = GameManager.Instance.currentPlayer;

		while (true) {
			if (currentPlayer == player) {
				currentPlayer = player2;
			} else {
				currentPlayer = player;
			}

			bool bottlesExist = FieldManager.Instance.CheckBottleExistence (currentPlayer, currentPlayer.currentRow);

			if (bottlesExist)
				break;
			else 
				GameManager.Instance.SetPlayerToNextRow (currentPlayer);
		}

		PlayerManager.Instance.SetPlayerPosition (player, player.currentRow, true);
		PlayerManager.Instance.SetPlayerPosition (player2, player2.currentRow, true);

		//Current Player Info
		PlayerManager.Instance.currentPlayer = currentPlayer.team;
		PlayerManager.Instance.isBot = false;
		PlayerManager.Instance.isEnemy = false;

		//Set Camera
		CameraManager.Instance.ShowPlayer (1f, currentPlayer);

		return currentPlayer;
	}
}

