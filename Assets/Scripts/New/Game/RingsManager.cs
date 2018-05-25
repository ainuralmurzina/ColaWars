using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Test{
	public class RingsManager : MonoBehaviour
	{
		public GameObject objField;
		public GameObject pfbRedRing;
		public GameObject pfbBlueRing;

		private Team team = Team.None;
		private int rows = 0;
		private int bottlesInRow = 0;
		private GameObject ring;
		private int side = 1;

		public void SetRingsParameters(Team team, int rows, int bottlesInRow, int side){
			this.team = team;
			this.rows = rows;
			this.bottlesInRow = bottlesInRow;
			this.side = side;
			ring = GetRing (team);
		}

		GameObject GetRing(Team team){
			if(team == Team.Cola)
				return pfbRedRing;
			if(team == Team.Pepsi)
				return pfbBlueRing;
			return null;
		}

		public void CreateRings(){
			float sizeX = objField.transform.localScale.x - 4;
			float distance = sizeX / rows /2f;

			float pos_x = sizeX / 2 - distance;
			float pos_z = side * (bottlesInRow + 2);

			for (int i = 0; i < rows; i++){

				GameObject new_ring = Instantiate (ring, this.transform);				 
				new_ring.transform.localPosition = new Vector3 (pos_x, 1f, pos_z);
				new_ring.name = ring.name + i;
				pos_x -= distance * 2;
			}
		}

		public void ResetField(){

			foreach (Transform t in this.transform) {
				Destroy (t.gameObject);
			}

		}

		public Transform GetRing(Team team, int i){
			return this.transform.Find (GetRing (team).name + i);
		}

		public void MoveRings(Team team, List<int> leftBottlesInEachRow){

			int counter = 0;
			GameObject ring = GetRing (team);

			foreach (Transform t in this.transform)
				if (t.name.Contains (ring.name)) {
					Vector3 pos = t.transform.position;
					pos.z = Mathf.Sign (pos.z) * (leftBottlesInEachRow [counter] + 2);
					t.transform.position = pos;
					counter++;
				}
		}
	}
}

