using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldManager : MonoBehaviour
{
	public static FieldManager Instance {
		get; 
		private set;
	}

	void Awake () {
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	//===============================================

	public int BottleStartPos_AxisZ = -3;

	[Header("One Row Bottles")]
	public GameObject objBottlesColaOne;
	public GameObject objBottlesPepsiOne;

	[Header("Five Rosw Bottles")]
	public GameObject objBottlesColaFive;
	public GameObject objBottlesPepsiFive;

	//Private
	private List<bool> cola_bottles_state = new List<bool> ();
	private List<bool> pepsi_bottles_state = new List<bool> ();

	public void ActivateBottles_OneRow(Player player){

		switch (player.team) {
		case Team.Cola:
			ResetBottles (objBottlesColaOne);
			objBottlesColaOne.SetActive (true);
			break;
		case Team.Pepsi:
			ResetBottles (objBottlesPepsiOne);
			objBottlesPepsiOne.SetActive (true);
			break;
		}
	}

	public void ActivateBottles_FiveRows(Player player){

		switch (player.team) {
		case Team.Cola:
			ResetBottles (objBottlesColaFive);
			objBottlesColaFive.SetActive (true);
			break;
		case Team.Pepsi:
			ResetBottles (objBottlesPepsiFive);
			objBottlesPepsiFive.SetActive (true);
			break;
		}
	}

	public void ResetBottles(GameObject bottles){
		
		int pos_z = BottleStartPos_AxisZ;

		foreach (Transform t in bottles.transform) {

			if (t.GetComponent<Rigidbody> () == null)
				continue;

			//Every 10 bottles z position is restarted
			if(pos_z - 10 == BottleStartPos_AxisZ) pos_z = BottleStartPos_AxisZ;				
			Vector3 pos = new Vector3 (GameManager.Instance.rows [t.gameObject.tag], 0f, pos_z);
			pos_z++;

			t.localPosition = pos;
			t.localRotation = Quaternion.identity;
			t.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			t.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

			t.gameObject.SetActive (true);
		}
	}

	public void DisactivateAllBottles(){
		objBottlesColaOne.SetActive (false);
		objBottlesPepsiOne.SetActive (false);
		objBottlesColaFive.SetActive (false);
		objBottlesPepsiFive.SetActive (false);
	}

	public Score CountBottles(){
		
		switch (GameManager.Instance.currentMode) {
		case GameMode.Practice:
			int score = 0;

			if (GameManager.Instance.currentPlayer.team == Team.Cola) {
				score = CountScores (objBottlesColaOne);
			} else {
				score = CountScores (objBottlesPepsiOne);
			}
	
			ActivateBottles_OneRow (GameManager.Instance.currentPlayer);
			return new Score (score);
			break;
		default:
			cola_bottles_state.Clear ();
			pepsi_bottles_state.Clear ();

			int score_pepsi = CountScores (objBottlesColaFive, cola_bottles_state);
			int score_cola = CountScores (objBottlesPepsiFive, pepsi_bottles_state);

			SendFieldStateInfo (new Score (score_cola, score_pepsi));

			return new Score (score_cola, score_pepsi);
			break;
		}
	}

	int CountScores(GameObject bottles, List<bool> bottle_state = null){
		int score = 0;
		foreach (Transform t in bottles.transform) {
			Vector3 rotation = t.rotation.eulerAngles;
			if (!t.gameObject.activeSelf || rotation.z > 1f) {
				score++;
				t.gameObject.SetActive (false);

				if(bottle_state != null)
					bottle_state.Add (false);
			} 
			else
				if(bottle_state != null)
					bottle_state.Add (true);
		}
		return score;
	}

	void SendFieldStateInfo (Score score_info){

		string cola_state = "";
		foreach (bool b in cola_bottles_state) {
			if (b)
				cola_state += "1";
			else
				cola_state += "0";
			cola_state += ",";
		}

		string pepsi_state = "";
		foreach (bool b in pepsi_bottles_state) {
			if (b)
				pepsi_state += "1";
			else
				pepsi_state += "0";
			pepsi_state += ",";
		}

		if(GameManager.Instance.currentMode == GameMode.MultiplayerOnline){
			OnlinePacket packet = new OnlinePacket (
				2,
				score_info,
				cola_state, 
				pepsi_state
			);
			ManagersController.Message (Message.Create (this, MessageData.EVENT_SEND_PACKET, packet));
		}
	}

	public bool CheckBottleExistence(Player player, string tag){
		
		bool exist = false;

		switch (GameManager.Instance.currentMode) {
		case GameMode.Practice:

			if (player.team == Team.Pepsi)
				exist = CheckBottles (objBottlesPepsiOne, tag);
			else
				exist = CheckBottles (objBottlesColaOne, tag);
			
			break;
		default:

			if (player.team == Team.Pepsi)
				exist = CheckBottles (objBottlesPepsiFive, tag);
			else
				exist = CheckBottles (objBottlesColaFive, tag);

			break;
		}

		return exist;
	}

	bool CheckBottles(GameObject bottles, string tag){
		foreach (Transform t in bottles.transform) 
			if (t.tag == tag && t.gameObject.activeSelf)
				return true;
		return false;
	}

	public void SetOnlineField (List<bool> cola_bottles, List<bool> pepsi_bottles){
		ResetBottles (objBottlesColaFive);
		ResetBottles (objBottlesPepsiFive);

		for (int i = 0; i < cola_bottles.Count; i++) {
			objBottlesColaFive.transform.GetChild (i).gameObject.SetActive (cola_bottles [i]);
			objBottlesPepsiFive.transform.GetChild (i).gameObject.SetActive (pepsi_bottles [i]);
		}
	}

	public List<GameObject> FallenBottlesList(){
		List<GameObject> fallen_bottles = new List<GameObject> ();

		switch (GameManager.Instance.currentMode) {
		case GameMode.Practice:
			foreach (Transform t in objBottlesColaOne.transform) {
				Vector3 rotation = t.rotation.eulerAngles;
				if (t.gameObject.activeSelf && rotation.z > 1f)
					fallen_bottles.Add (t.gameObject);
			}
			foreach (Transform t in objBottlesPepsiOne.transform) {
				Vector3 rotation = t.rotation.eulerAngles;
				if (t.gameObject.activeSelf && rotation.z > 1f)
					fallen_bottles.Add (t.gameObject);
			}
			break;
		default:
			foreach (Transform t in objBottlesColaFive.transform) {
				Vector3 rotation = t.rotation.eulerAngles;
				if (t.gameObject.activeSelf && rotation.z > 1f)
					fallen_bottles.Add (t.gameObject);
			}
			foreach (Transform t in objBottlesPepsiFive.transform) {
				Vector3 rotation = t.rotation.eulerAngles;
				if (t.gameObject.activeSelf && rotation.z > 1f)
					fallen_bottles.Add (t.gameObject);
			}
			break;
		}

		return fallen_bottles;
	}
}

