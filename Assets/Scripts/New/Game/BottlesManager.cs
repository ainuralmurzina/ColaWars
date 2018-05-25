using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Test{
	public class BottlesManager : MonoBehaviour {

		public GameObject objField;
		public GameObject pfbBottle_Cola;
		public GameObject pfbBottle_Pepsi;

		private Team team = Team.None;
		private int rows = 0;
		private int bottlesInRow = 0;
		private int side = 1;
		private GameObject bottle;

		public void SetLocalFieldParameters(Team team, int rows, int bottlesInRow, int side){
			this.team = team;
			this.rows = rows;
			this.bottlesInRow = bottlesInRow;
			this.side = side;
			bottle = GetBottle (team);
		}

		GameObject GetBottle(Team team){
			if(team == Team.Cola)
				return pfbBottle_Cola;
			if(team == Team.Pepsi)
				return pfbBottle_Pepsi;
			return null;
		}

		public void CreateLocalField(){
			float sizeX = objField.transform.localScale.x - 4;
			float distance = sizeX / rows /2f;

			float pos_x = sizeX / 2 - distance;
			float pos_z = side;
			float inc = side;

			for (int i = 0; i < rows; i++){
				
				for (int j = 0; j < bottlesInRow; j++) {
					GameObject new_bottle = Instantiate (bottle, this.transform);				 
					new_bottle.transform.localPosition = new Vector3 (pos_x, 0f, pos_z);
					pos_z+= inc;
				}

				pos_x -= distance * 2;
				pos_z = side;
			}
		}

		public void ResetField(){
			
			foreach (Transform t in this.transform) {
				Destroy (t.gameObject);
			}

		}

		public int GetDownedBottlesNumber(Team team){

			int counter = 0;
			GameObject bottle = GetBottle (team);

			foreach (Transform t in this.transform) 
				if(t.name.Contains(bottle.name) && t.rotation.eulerAngles.z > 1f)
					counter++;
			
			return counter;
		}

		public void RemoveDownedBottles(){
			
			foreach (Transform t in this.transform)
				if (t.rotation.eulerAngles.z > 1f)
					Destroy (t.gameObject);
		}

		public List<int> GetLeftBotllesNumberInRows(Team team){

			List<int> leftBottlesInEachRow = new List<int> ();

			List<float> rows_pos = new List<float> ();

			float sizeX = objField.transform.localScale.x - 4;
			float distance = sizeX / rows /2f;
			float pos_x = sizeX / 2 - distance;

			for (int i = 0; i < rows; i++) {
				rows_pos.Add (pos_x);
				pos_x -= distance * 2;
			}

			List<List<Transform>> rowsOfBottles = new List<List<Transform>> ();
			GameObject bottle = GetBottle (team);

			for (int i = 0; i < rows; i++) {
				List<Transform> bottles_list = new List<Transform> ();

				foreach (Transform t in this.transform) {
					float pos = Mathf.Round (t.transform.position.x * 10) / 10;
					if (t.name.Contains (bottle.name) && pos == rows_pos [i]) {
						bottles_list.Add (t);
					}
				}

				rowsOfBottles.Add (bottles_list);
			}

			foreach (List<Transform> list in rowsOfBottles){

				int pos = 0;

				if(list.Count != 0)
					pos = (int)Mathf.Abs(Mathf.Round (list [list.Count - 1].position.z));
				
				leftBottlesInEachRow.Add (Mathf.Max (list.Count, pos));
				Debug.Log (pos + " " + list.Count);;
			}

			return leftBottlesInEachRow;
		}
	}
}
