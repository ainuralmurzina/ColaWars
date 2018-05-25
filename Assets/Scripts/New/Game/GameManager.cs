using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Test{
	public class GameManager : MonoBehaviour
	{

		public int rows = 6;
		public int bottlesInRows = 10;

		public FieldManager mngrField;
		public CameraManager mngrCamera;
		public MenuManager mngrMenu;

		private List<Team> playingTeams = new List<Team> ();
		private int currentRingNum = 0;
		private Team currentTeam = Team.None;
		private Transform currentRing = null;

		public void CreateAndStartGame(List<Team> playingTeams){
			this.playingTeams = playingTeams;

			mngrField.Reset ();
			mngrField.SetFieldsParameters (playingTeams, rows, bottlesInRows);
			mngrField.CreateField ();

			StartCoroutine (StartGame ());
		}

		IEnumerator StartGame(){
			yield return new WaitForSeconds (4f);
			StartRound ();
		}

		public void StartRound(){
			CalculateRingQueue ();
			StartCoroutine (ActivateRing ());
		}

		public void TimeIsOut(){
			currentRing.GetComponent<Ring> ().ForceReset ();
			RingStateChanged(GameState.None);
		}

		public void RingStateChanged(GameState state){
			if (state == GameState.None) {
				mngrCamera.ShowField ();
				bool finishGame = CheckRound ();

				if(!finishGame)
					StartCoroutine (NextRound ());
				else
					StartCoroutine (FinishGame ());
			} 
			else if (state == GameState.ObserveField) {
				mngrCamera.ShowFieldFromRing (currentRing);
			} 
			else if (state == GameState.ViewResult) {
				mngrCamera.FollowRing (currentRing);
			} 
			else {
				mngrCamera.ShowRing (currentRing);
			}

			mngrMenu.ShowGameState (state);

		}

		bool CheckRound(){

			bool finishGame = false;

			foreach (Team team in playingTeams) {
				int round_score = mngrField.GetRoundResult (team);
				int overall_score = mngrMenu.AddScore (round_score, team);

				if (overall_score >= rows * bottlesInRows && currentRingNum % 2 == 0)
					finishGame = true;
			}

			return finishGame;
		}

		IEnumerator NextRound(){
			yield return new WaitForSeconds (2.5f);
			mngrField.UpdateField ();
			yield return new WaitForSeconds (1f);
			StartRound ();
		}

		IEnumerator FinishGame(){
			yield return new WaitForSeconds (2.5f);
			mngrMenu.ShowResults ();
			mngrField.Reset ();
		}

		IEnumerator ActivateRing(){
			yield return new WaitForSeconds (2f);
			currentRing.GetComponent<Ring> ().ActivateRing ();
		}

		void CalculateRingQueue(){
			
			if (currentRingNum >= rows * 2)
				currentRingNum = 0;
			
			currentTeam = playingTeams [currentRingNum % 2];
			currentRing = mngrField.GetRing (currentTeam, currentRingNum / 2);
			currentRingNum++;
			
		}
	}
}

