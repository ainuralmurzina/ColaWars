using UnityEngine;
using System.Collections;

public class SingleMode :  Mode
{
	public Player enemy;

	private Team teamPlayer;
	private Team teamEnemy;

	private GameSubMode subMode;

	//Constructor
	public SingleMode(GameSubMode subMode, Team teamPlayer, Team teamEnemy){
		this.subMode = subMode;
		this.teamPlayer = teamPlayer;
		this.teamEnemy = teamEnemy;
	}


	//Methods
	public override void CreateGame(){

		//Create Players
		player = new Player (teamPlayer, "ВЫ");
		enemy = new Player (teamEnemy, "СОПЕРНИК");

		player.currentID = 1;
		enemy.currentID = 2;

		PlayerManager.Instance.player = player;
		PlayerManager.Instance.enemy = enemy;

		//Set Player Attributes according to its team
		PlayerManager.Instance.SetPlayerAttributs(player);
		PlayerManager.Instance.SetPlayerAttributs(enemy);

		//Create Bottles
		FieldManager.Instance.ActivateBottles_FiveRows(player);
		FieldManager.Instance.ActivateBottles_FiveRows(enemy);

		//Set Player Position
		PlayerManager.Instance.SetPlayerPosition(player, "most_left", false);
		PlayerManager.Instance.SetPlayerPosition(enemy, "most_left", false);

		//Activate players
		ActivatePlayers ();

		//Set Cameras
		CameraManager.Instance.ResetBotCamera (enemy);
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
		PlayerManager.Instance.SwitchPlayer (enemy, false, false);

		//Disable bottles
		FieldManager.Instance.DisactivateAllBottles ();

		//Reset rings
		player.ring.GetComponent<Ring> ().ResetRing ();
		enemy.ring.GetComponent<Ring> ().ResetRing ();
	}

	public override void ActivatePlayers ()
	{
		PlayerManager.Instance.SwitchPlayer (player, false, true);
		PlayerManager.Instance.SwitchPlayer (enemy, false, true);
	}

	public override Player CheckTurn(){
		
		Player currentPlayer = GameManager.Instance.currentPlayer;

		while (true) {
			if (currentPlayer == player) {
				currentPlayer = enemy;
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
		PlayerManager.Instance.SetPlayerPosition (enemy, enemy.currentRow, true);

		PlayerManager.Instance.currentPlayer = currentPlayer.team;
		if (currentPlayer == player) {
			CameraManager.Instance.ShowPlayer (1f, player);
			PlayerManager.Instance.isBot = false;
			PlayerManager.Instance.isEnemy = false;
		} 
		else {
			CameraManager.Instance.ShowRing (1f, player, enemy);
			PlayerManager.Instance.isBot = true;
			PlayerManager.Instance.isEnemy = false;

			//Disactivate player during bot shoot
			PlayerManager.Instance.SwitchPlayer(player, false,false, 2f);
		}

		return currentPlayer;
	}

}

