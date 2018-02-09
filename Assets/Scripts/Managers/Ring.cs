using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class Ring : MonoBehaviour
{

	public Team team;

	private GameState currentState;
	private bool isMouseDown = false;

	//Shoot Properties
	private float shootTimeStart = 0f;
	private float shootTimeEnd = 0f;
	private Vector2 shootPosStart = Vector2.zero;
	private Vector2 shootPosEnd = Vector2.zero;

	void Update ()
	{
		if (IsMouseOverUI ())
			return;

		if (GameManager.Instance.currentPlayer != null && GameManager.Instance.currentPlayer.team != team)
			return;

		currentState = GameManager.Instance.currentState;

		if (currentState == GameState.ViewResult) {
			Rigidbody rb = GetComponent<Rigidbody> ();
			if (!rb.isKinematic && rb.velocity.magnitude < 0.001f) {
				ResetRing ();
				GameManager.Instance.SetPlayerToNextRow ();
				GameManager.Instance.NextRound ();
			}
		}

		if (Input.GetButtonUp ("Fire1")) {

			isMouseDown = false;

			if (currentState == GameState.Shoot) {
				shootTimeEnd = Time.time * 1000;
				shootPosEnd = Input.mousePosition;
				ShootRing ();	
			} 
			else 
			{
				GameManager.Instance.ChangeGameState ();
			}

		} 
		else if (Input.GetButtonDown ("Fire1")) {

			isMouseDown = true;

			if(currentState == GameState.Shoot){
				shootTimeStart = Time.time * 1000;
				shootPosStart = Input.mousePosition;
			}
		}

		if (isMouseDown) {
			switch (currentState) {
			case GameState.ObserveField:
				RotateField ();
				break;
			case GameState.RotateDisk:
				RotateRing ();
				break;
			case GameState.MoveDisk:
				MoveRing ();
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

	void RotateField(){
		//Turn ring

		//float angle = Vector3.SignedAngle (Vector3.forward, Camera.main.transform.forward, Vector3.up);
		//transform.rotation = Quaternion.AngleAxis (angle, Vector3.up);
		transform.rotation = Quaternion.Euler(new Vector3 (0f, Camera.main.transform.rotation.eulerAngles.y, 0f));
	}

	void RotateRing(){
		float AngleRad = Mathf.Atan2(Input.mousePosition.y - Screen.height / 2, Input.mousePosition.x - Screen.width / 2);
		float AngleDeg = (180 / Mathf.PI) * AngleRad;

		Vector3 rotation = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(rotation.x, rotation.y, AngleDeg);
	}

	void MoveRing (){

		if(GameManager.Instance.currentPlayer.team == Team.Cola)
			transform.position = new Vector3(
				Mathf.Clamp(Input.mousePosition.x / Screen.width, 0.3f, 0.6f) - 0.5f + GameManager.Instance.rows[GameManager.Instance.currentPlayer.currentRow],
				Mathf.Clamp(Input.mousePosition.y / Screen.height * 1.5f, 0.25f, 1.5f), 
				transform.position.z);
		else
			transform.position = new Vector3(
				-(Mathf.Clamp(Input.mousePosition.x / Screen.width, 0.3f, 0.6f) - 0.5f) + GameManager.Instance.rows[GameManager.Instance.currentPlayer.currentRow],
				Mathf.Clamp(Input.mousePosition.y / Screen.height * 1.5f, 0.25f, 1.5f), 
				transform.position.z);
	}

	void ShootRing(){

		float time = shootTimeEnd - shootTimeStart;
		float distance = Vector3.Distance (shootPosStart, shootPosEnd);
		Vector2 direction = (shootPosEnd - shootPosStart).normalized;

		if(time < 50 || time > 1000 || distance < 50)
			return;

		//Change state to view shoot result
		GameManager.Instance.ChangeGameState ();

		Rigidbody rb = GetComponent<Rigidbody>();

		//Set speed
		//float speed = Mathf.Clamp(1.5f * distance / time, 5f, 30f)

		Vector3 ring_direction = this.transform.forward.normalized;
		float angle = Vector2.SignedAngle (new Vector2 (0f, 1f), direction);
		Vector3 velocity_direction = Quaternion.AngleAxis (-angle, Vector3.up) * ring_direction;

		float speed = Mathf.Min(1000 * 2 * 2 / time, 25);
		Vector3 av = new Vector3(-Mathf.Sin(this.transform.rotation.eulerAngles.z / 180 * Mathf.PI), Mathf.Cos(this.transform.rotation.eulerAngles.z / 180 * Mathf.PI), 0);
		av.Scale(new Vector3(speed * 10, speed * 10, speed * 10));
		//Vector3 speed3D = new Vector3((shootPosEnd.x - shootPosStart.x) / Screen.width * speed / 25, (shootPosEnd.y - shootPosStart.y) / Screen.height * speed / 8, speed);

		Vector3 velocity = velocity_direction * speed;
		SendRingShootInfo (velocity);

		rb.isKinematic = false;
		rb.angularVelocity = av;
		rb.velocity = velocity;
	}

	public void SendRingShootInfo(Vector3 velocity){
		if(GameManager.Instance.currentMode == GameMode.MultiplayerOnline){
			OnlinePacket packet = new OnlinePacket (
				1,
				transform.position,
				transform.rotation.eulerAngles,
				velocity
			);
			ManagersController.Message (Message.Create (this, MessageData.EVENT_SEND_PACKET, packet));
		}
	}

	public void ResetRing(){

		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.isKinematic = true;

		if (team == Team.Cola) {
			transform.rotation = Quaternion.identity;
		} 
		else {
			transform.rotation = new Quaternion (0f, 1f, 0f, 0f);
		}
	}

	public void Bot_ShootRing(){
		
		Sequence seq = DOTween.Sequence ();

		Vector3 rotation1 = Vector3.zero;
		Vector3 rotation2 = Vector3.zero;
		Vector3 position = Vector3.zero;
		float time = 0f;

		if (GameManager.Instance.currentSubMode == GameSubMode.Easy) {
			rotation1 = new Vector3 (0f, UnityEngine.Random.Range (160f, 200f), 0f);
			rotation2 = new Vector3 (0f, rotation1.y, UnityEngine.Random.Range (0f, 90f));
			position = new Vector3 (transform.position.x, UnityEngine.Random.Range (0.95f, 1.25f), transform.position.z);
			time = UnityEngine.Random.Range(50f, 100f);
		} 
		else if (GameManager.Instance.currentSubMode == GameSubMode.Medium) {
			rotation1 = new Vector3 (0f, UnityEngine.Random.Range (175f, 185f), 0f);
			rotation2 = new Vector3 (0f, rotation1.y, UnityEngine.Random.Range (0f, 45f));
			position = new Vector3 (transform.position.x, UnityEngine.Random.Range (1.25f, 1.45f), transform.position.z);
			time = UnityEngine.Random.Range(50f, 75f);
		}
		else if (GameManager.Instance.currentSubMode == GameSubMode.Hard) {
			rotation1 = new Vector3 (0f, 180f, 0f);
			rotation2 = new Vector3 (0f, rotation1.y, UnityEngine.Random.Range (0f, 10f));
			position = new Vector3 (transform.position.x, UnityEngine.Random.Range (1.4f, 1.5f), transform.position.z);
			time = UnityEngine.Random.Range(50f, 60f);
		}

		transform.rotation = Quaternion.Euler (rotation1);
		CameraManager.Instance.SetBotCamera(Mathf.Clamp ((180 - rotation1.y) / 90f * 4f, -3f, 3f));

		seq.Append (transform.DORotate(rotation2, 0.5f)).AppendInterval(0.5f);
		seq.Append (transform.DOMove (position, 0.5f)
			.OnComplete(() => GameManager.Instance.currentPlayer.santa.transform.GetChild(0).GetComponent<Animator> ().SetTrigger ("Throw")))
			.AppendInterval (0.5f);


		seq.OnComplete(() => {
			Rigidbody rb = transform.GetComponent<Rigidbody>();

			//Speed
			float speed =  Mathf.Min(1000 * 2 * 2 / time, 25);
			Vector3 v = CameraManager.Instance.vcam_start_bot.transform.right * speed;

			if (GameManager.Instance.currentSubMode == GameSubMode.Hard)
				v.z = 0f;

			rb.velocity = new Vector3(-v.z, v.y, v.x);

			Vector3 av = new Vector3(-Mathf.Sin(this.transform.rotation.eulerAngles.z / 180 * Mathf.PI), Mathf.Cos(this.transform.rotation.eulerAngles.z / 180 * Mathf.PI), 0);
			av.Scale(new Vector3(speed * 10, speed * 10, speed * 10));
			rb.angularVelocity = av;

			rb.isKinematic = false;
		});

	}

	public void OnlinePlayer_ShootRing(Vector3 position, Vector3 rotation, Vector3 velocity){
		Sequence seq = DOTween.Sequence ();

		CameraManager.Instance.SetBotCamera(Mathf.Clamp ((180 - rotation.y) / 90f * 4f, -3f, 3f));

		seq.Append (transform.DORotate(rotation, 0.5f)).AppendInterval(0.5f);
		seq.Append (transform.DOMove (position, 0.5f));

		seq.OnComplete(() => {
			GameManager.Instance.currentPlayer.santa.transform.GetChild(0).GetComponent<Animator> ().SetTrigger ("Throw");
			Rigidbody rb = transform.GetComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.velocity = velocity;
		});
	}

	private void OnCollisionEnter(Collision collision) {
		AudioSource hit = collision.collider.GetComponentInParent<AudioSource>();
		if(hit && !hit.isPlaying) {
			hit.volume = collision.impulse.magnitude;
			hit.PlayOneShot(hit.clip);
		}
	}
}

