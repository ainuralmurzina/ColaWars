using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Test{
	public class FieldManager : MonoBehaviour
	{
		public BottlesManager mngrBottles;
		public RingsManager mngrRings;

		private List<Team> playingTeams;
		private int rows = 0;
		private int bottlesInRow = 0;

		public void SetFieldsParameters(List<Team> playingTeams, int rows, int bottlesInRow){
			this.playingTeams = playingTeams;
			this.rows = rows;
			this.bottlesInRow = bottlesInRow;
		}

		public void CreateField(){

			//max num of playing teams = 2
			int[] sides = new int[]{ 1, -1 };
			int i = 0;

			foreach(Team team in playingTeams){

				int side = sides [i];
				i++;

				mngrBottles.SetLocalFieldParameters (team, rows, bottlesInRow,side);
				mngrBottles.CreateLocalField ();

				mngrRings.SetRingsParameters (team, rows, bottlesInRow, side);
				mngrRings.CreateRings ();

			}


		}

		public void Reset(){
			mngrBottles.ResetField ();
			mngrRings.ResetField ();
		}

		public Transform GetRing(Team team, int num){
			return mngrRings.GetRing (team, num);
		}

		public int GetRoundResult(Team team){
			int score = 0;
			score = mngrBottles.GetDownedBottlesNumber (team);
			return score;
		}

		public void UpdateField(){
			StartCoroutine (UpdateFieldCoroutine());
		}

		IEnumerator UpdateFieldCoroutine(){
			mngrBottles.RemoveDownedBottles ();
			yield return new WaitForSeconds (1f);

			List<int> leftBottlesInEachRow = new List<int> ();
			foreach (Team team in playingTeams) {
				leftBottlesInEachRow = mngrBottles.GetLeftBotllesNumberInRows (team);
				mngrRings.MoveRings (team, leftBottlesInEachRow);
				leftBottlesInEachRow.Clear ();
			}
		}
	}
}

