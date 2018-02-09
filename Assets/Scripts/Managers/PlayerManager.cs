using UnityEngine;
using System.Collections;

//============================================
using DG.Tweening;

public class Player
{
	public string displayName;
	public int currentScore;
	public int currentID; // 1 or 2
	public Team team;
	public GameObject ring;
	public GameObject santa;
	public string currentRow;

	//Contructors
	public Player(){
		Team _team = Team.Cola;
		string team = PlayerPrefs.GetString ("Team", "Cola");
		if (team == "Cola")
			_team = Team.Cola;
		if (team == "Pepsi")
			_team = Team.Pepsi;
		this.team = _team;
	}

	public Player(Team team, string displayName){
		this.team = team;
		this.displayName = displayName;
		currentScore = 0;
	}
}

//============================================

public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Instance {
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

	//==============================================

	[Header ("Current Player Info")]
	public Team currentPlayer;
	public bool isBot;
	public bool isEnemy;

	[Space (10)]
	public Player player;
	public Player enemy;

	[Header ("Ring")]
	public GameObject objRingRed;
	public GameObject objRingBlue;

	public float ringPosY = 1f;
	public float ringPosZ = 12f;

	[Header ("Santa")]
	public GameObject objSantaRed;
	public GameObject objSantaBlue;

	public float santaPosZ = 13f;

	[HideInInspector]
	public float animationTime = 0f;

	public void SetPlayerAttributs(Player player){
	
		switch (player.team) {
		case Team.Cola:
			player.ring = objRingRed;
			player.santa = objSantaRed;
			break;
		case Team.Pepsi:
			player.ring = objRingBlue;
			player.santa = objSantaBlue;
			break;
		}
	}

	public void SetPlayerPosition(Player player,string row, bool animate){
		
		player.currentRow = row;

		if (!player.santa.activeSelf)
			animate = false;

		switch (player.team) {
		case Team.Cola:
			
			objRingRed.transform.position = new Vector3 (GameManager.Instance.rows [row], ringPosY, -ringPosZ);

			if (!animate) { 
				animationTime = 0f;
				objSantaRed.transform.position = new Vector3 (GameManager.Instance.rows [row], 0f, -ringPosZ);
			}
			else {
				Transform santa = objSantaRed.transform;

				//Set rotation vectors
				Vector3 targetPosition = new Vector3 (GameManager.Instance.rows [row], 0f, -ringPosZ);
				Vector3 initialRotation = santa.rotation.eulerAngles;
				Vector3 targetRotation = initialRotation;
				float difference = santa.position.x - GameManager.Instance.rows [row];
				targetRotation.y = difference < 0 ? targetRotation.y + 90 : targetRotation.y - 90;

				//Animate
				animationTime = Mathf.Abs (difference);
				if (difference != 0f)
					AnimateSanta (santa, initialRotation, targetRotation, targetPosition);
			}
			break;
		case Team.Pepsi:
			
			objRingBlue.transform.position = new Vector3 (GameManager.Instance.rows [row], ringPosY, ringPosZ);

			if (!animate) {
				animationTime = 0f;
				objSantaBlue.transform.position = new Vector3 (GameManager.Instance.rows [row], 0f, ringPosZ);
			}
			else {
				Transform santa = objSantaBlue.transform;

				//Set rotation vectors
				Vector3 targetPosition = new Vector3 (GameManager.Instance.rows [row], 0f, santaPosZ);
				Vector3 initialRotation = santa.rotation.eulerAngles;
				Vector3 targetRotation = initialRotation;
				float difference = santa.position.x - GameManager.Instance.rows [row];
				targetRotation.y = difference < 0 ? targetRotation.y - 90 : targetRotation.y + 90;

				//Animate
				animationTime = Mathf.Abs (difference);
				if (difference != 0f)
					AnimateSanta (santa, initialRotation, targetRotation, targetPosition);
			}
			break;
		}
	}

	private void AnimateSanta(Transform santa, Vector3 initialRotation, Vector3 targetRotation, Vector3 targetPosition){
		Sequence seq = DOTween.Sequence ();

		seq.Append (santa.DORotate (targetRotation, 0.25f));
		seq.Append (santa.DOMove (targetPosition, animationTime));
		seq.Append (santa.DORotate (initialRotation, 0.25f));

		seq
			.OnPlay (() => {
				if(santa.GetChild (0).GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
					santa.GetChild (0).GetComponent<Animator> ().SetTrigger ("Walk");
			})
			.OnComplete (() => {
				if(santa.GetChild (0).GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).IsName("Walk"))
					santa.GetChild (0).GetComponent<Animator> ().SetTrigger ("Walk");
			});
	}

	public void SwitchPlayer(Player player, bool showRing = true, bool showSanta = false, float waitTime = 0f){
		if (waitTime == 0f) {
			player.ring.GetComponent<MeshRenderer> ().enabled = showRing;
			player.santa.SetActive (showSanta);
		}
		else
			StartCoroutine (SwitchPlayerCoroutine(player, showRing, showSanta, waitTime));
	}

	IEnumerator SwitchPlayerCoroutine(Player player, bool showRing, bool showSanta, float waitTime){
		yield return new WaitForSeconds (waitTime);

		player.ring.GetComponent<MeshRenderer> ().enabled = showRing;
		player.santa.SetActive (showSanta);
	}

	public void SetPlayersScore(Score score){
		player.currentScore = player.team == Team.Cola ? score.score_cola : score.score_pepsi;
		enemy.currentScore = enemy.team == Team.Cola ? score.score_cola : score.score_pepsi;
	}
}