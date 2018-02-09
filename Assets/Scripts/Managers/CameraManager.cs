using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance {
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

	public CinemachineVirtualCamera vcam_santa;
	public CinemachineVirtualCamera vcam_start;
	public CinemachineVirtualCamera vcam_shoot;
	public CinemachineVirtualCamera vcam_field;
	public CinemachineVirtualCamera vcam_field2;
	public CinemachineVirtualCamera vcam_start_bot;
	public CinemachineVirtualCamera vcam_bottles1;
	public CinemachineVirtualCamera vcam_bottles2;

	private GameState currentState;
	private bool isMouseDown = false;

	private float waitTime = 0f;

	void Update(){

		if (IsMouseOverUI ())
			return;

		currentState = GameManager.Instance.currentState;

		if (currentState == GameState.ViewResult && !PlayerManager.Instance.isBot && !PlayerManager.Instance.isEnemy) {
			if (!vcam_shoot.gameObject.activeSelf && Mathf.Abs (Camera.main.transform.position.z - vcam_start.LookAt.position.z) > 5f) {
				ShowShoot ();
			}
		}

		if (Input.GetButtonUp ("Fire1")) {
			isMouseDown = false;
		} 
		else if (Input.GetButtonDown ("Fire1")) {
			isMouseDown = true;
		}

		if (isMouseDown) {
			switch (currentState) {
			case GameState.ObserveField:
				ObserveField ();
				break;
			case GameState.MoveDisk:
				if(vcam_start.Follow != null)
					vcam_start.Follow = null;
				break;
			}
		}

	}

	bool IsMouseOverUI() {
		try {
			if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
		} catch(Exception e) { e.ToString(); }
		if(EventSystem.current.IsPointerOverGameObject()) return true;
		return false;
	}

	void ObserveField(){
		//Turn camera
		Vector3 offset = vcam_santa.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset;
		Vector3 rotation;

		if(GameManager.Instance.currentPlayer.team == Team.Cola)
			rotation = new Vector3 (offset.x, Mathf.Clamp((Input.mousePosition.y / Screen.height) * 10f, 1f, 5f), Mathf.Clamp(((Input.mousePosition.x / Screen.width) - 0.5f) * 10f, -5f, 5f));
		else 
			rotation = new Vector3 (offset.x, Mathf.Clamp((Input.mousePosition.y / Screen.height) * 10f, 1f, 5f), Mathf.Clamp(-((Input.mousePosition.x / Screen.width) - 0.5f) * 10f, -5f, 5f));
		
		vcam_santa.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset = rotation;
	}

	void RotateField(){
		//Turn camera
		Vector3 offset = vcam_start.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset;
		Vector3 rotation;

		if(GameManager.Instance.currentPlayer.team == Team.Cola)
			rotation = new Vector3 (Mathf.Clamp(-((Input.mousePosition.x / Screen.width) - 0.5f) * 1.95f, -5f, 5f),	offset.y, offset.z);
		else 
			rotation = new Vector3 (Mathf.Clamp(((Input.mousePosition.x / Screen.width) - 0.5f) * 1.95f, -5f, 5f),	offset.y, offset.z);

		vcam_start.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset = rotation;
	}

	public void ShowField(){
		if(GameManager.Instance.currentMode != GameMode.SinglePlayer && GameManager.Instance.currentMode != GameMode.MultiplayerOnline)
			if (GameManager.Instance.currentPlayer == null || GameManager.Instance.currentPlayer.team == Team.Cola)
				ShowField_Cola ();
			else
				ShowField_Pepsi ();
		else
			if (PlayerManager.Instance.player == null || PlayerManager.Instance.player.team == Team.Cola)
				ShowField_Cola ();
			else
				ShowField_Pepsi ();
		
		vcam_start.gameObject.SetActive (true);
		vcam_shoot.gameObject.SetActive (true);
	}

	void ShowField_Cola(){
		vcam_field.gameObject.SetActive (true);
		vcam_field2.gameObject.SetActive (false);
	}

	void ShowField_Pepsi(){
		vcam_field.gameObject.SetActive (false);
		vcam_field2.gameObject.SetActive (true);
	}

	public void ShowShoot(){

		Vector3 offset = vcam_shoot.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset;
		if ((GameManager.Instance.currentPlayer.team == Team.Cola && offset.z > 0) || (GameManager.Instance.currentPlayer.team == Team.Pepsi && offset.z < 0))
			offset.z *= -1;
		vcam_shoot.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset = offset;

		vcam_shoot.Follow = vcam_start.LookAt;
		vcam_shoot.LookAt = vcam_start.LookAt;

		vcam_shoot.gameObject.SetActive (true);
	}

	public void ShowPlayer(float waitTime, Player player){

		//Reset camera
		Vector3 offset = vcam_santa.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset;
		vcam_santa.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset = new Vector3 (offset.x, 2f, 0f);

		StartCoroutine (ShowPlayerCoroutine(waitTime, player));
	}

	IEnumerator ShowPlayerCoroutine(float waitTime, Player player){

		yield return new WaitForSeconds (waitTime);

		if (player.team == Team.Cola && !vcam_field.gameObject.activeSelf) {
			vcam_field.gameObject.SetActive (true);
			yield return new WaitForSeconds (2f);
		}
		else if (player.team == Team.Pepsi && !vcam_field2.gameObject.activeSelf) {
			vcam_field2.gameObject.SetActive (true);
			yield return new WaitForSeconds (2f);
		}	

		vcam_santa.Follow = player.santa.transform;
		vcam_santa.LookAt = player.santa.transform;

		if(player.team == Team.Cola)
			player.santa.transform.position = new Vector3 (player.santa.transform.position.x, player.santa.transform.position.y, -PlayerManager.Instance.ringPosZ);
		else
			player.santa.transform.position = new Vector3 (player.santa.transform.position.x, player.santa.transform.position.y, PlayerManager.Instance.ringPosZ);
		
		vcam_santa.gameObject.SetActive (true);
		vcam_start.gameObject.SetActive (false);
		vcam_shoot.gameObject.SetActive (false);
		vcam_field.gameObject.SetActive (false);
		vcam_field2.gameObject.SetActive (false);
		vcam_bottles1.gameObject.SetActive (false);
		vcam_bottles2.gameObject.SetActive (false);

		float time = 2f;
		if(GameManager.Instance.currentPlayer != null && 
			GameManager.Instance.currentPlayer.santa.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Walk"))
			time = 2f >= PlayerManager.Instance.animationTime ? 2f : PlayerManager.Instance.animationTime;
		Invoke ("CameraIsReady", time);

		StopCoroutine ("ShowPlayerCoroutine");
	}

	public void ShowRing(float waitTime, Player player, Player enemy = null){
		//Reset camera
		Vector3 offset = vcam_start.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset;
		vcam_start.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset = new Vector3 (0f, offset.y, offset.z);

		StartCoroutine(ShowRingCoroutine(waitTime, player, enemy));
	}

	IEnumerator ShowRingCoroutine(float waitTime, Player player, Player enemy){

		yield return new WaitForSeconds (waitTime);

		if (enemy == null) {
			ShowRing (player);
		} 
		else {
			ShowEnemyRing (player, enemy);
		}

		StopCoroutine ("ShowRingCoroutine");
	}

	private void ShowRing(Player player){
		vcam_start.Follow = player.ring.transform;
		vcam_start.LookAt = player.ring.transform;

		vcam_start.gameObject.SetActive (true);
	}

	private void ShowEnemyRing(Player player, Player enemy){

		vcam_start.Follow = player.ring.transform;
		vcam_start.LookAt = enemy.ring.transform;

		if(player.team == Team.Cola)
			player.santa.transform.position = new Vector3 (player.santa.transform.position.x, player.santa.transform.position.y, -PlayerManager.Instance.santaPosZ);
		else
			player.santa.transform.position = new Vector3 (player.santa.transform.position.x, player.santa.transform.position.y, PlayerManager.Instance.santaPosZ);

		vcam_start.gameObject.SetActive (true);
		vcam_shoot.gameObject.SetActive (false);
		vcam_field.gameObject.SetActive (false);
		vcam_field2.gameObject.SetActive (false);
		vcam_bottles1.gameObject.SetActive (false);
		vcam_bottles2.gameObject.SetActive (false);

		float waitTime = 2f >= PlayerManager.Instance.animationTime ? 2f : PlayerManager.Instance.animationTime;
		Invoke ("CameraIsReady", waitTime);
	}

	public void ResetBotCamera(Player player){
		//Reset camera
		Vector3 offset = vcam_start_bot.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset;
		vcam_start_bot.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset = new Vector3 (0f, offset.y, offset.z);

		vcam_start_bot.Follow = player.ring.transform;
		vcam_start_bot.LookAt = player.ring.transform;
	}

	public void SetBotCamera(float pos_x){
		//Reset camera
		Vector3 offset = vcam_start_bot.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset;
		vcam_start_bot.GetCinemachineComponent<CinemachineTransposer> ().m_FollowOffset = new Vector3 (pos_x, offset.y, offset.z);
	}

	void CameraIsReady(){
		GameManager.Instance.GameIsReady();
	}
}

