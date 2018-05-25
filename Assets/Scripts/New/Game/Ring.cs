using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace Test{
	public class Ring : MonoBehaviour
	{
		private bool isTimerOn = true;
		private GameState currentState = GameState.None;
		private bool isMouseDown = false;

		private GameManager mngrMain;

		//Shoot Properties
		private float shootTimeStart = 0f;
		private float shootTimeEnd = 0f;
		private Vector2 shootPosStart = Vector2.zero;
		private Vector2 shootPosEnd = Vector2.zero;

		//Initial properties
		private Vector3 initPos = Vector3.zero;
		private Quaternion initRot = Quaternion.identity;
		private int factor = 1;

		//Components
		private Rigidbody myRigidbody;
		private Collider myCollider;

		void Start(){
			mngrMain = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();

			factor = -(int)Mathf.Sign(transform.position.z);

			initPos = transform.position;
			initRot = transform.rotation;

			myRigidbody = GetComponent<Rigidbody> ();
			myCollider = GetComponent<Collider> ();
		}

		public void ActivateRing(){
			myCollider.enabled = true;
			currentState = GameState.ObserveField;
			mngrMain.RingStateChanged (currentState);
		}

		void Update ()
		{
			if (IsMouseOverUI ())
				return;

			if (currentState == GameState.ViewResult) {
				if (!myRigidbody.isKinematic && myRigidbody.velocity.magnitude < 0.001f) {
					NextState ();
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
					NextState ();
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

		void NextState(){
			switch (currentState) {
			case GameState.ObserveField:
				currentState = GameState.RotateDisk;
				mngrMain.RingStateChanged (currentState);
				break;
			case GameState.RotateDisk:
				currentState = GameState.MoveDisk;
				mngrMain.RingStateChanged (currentState);
				break;
			case GameState.MoveDisk:
				currentState = GameState.Shoot;
				mngrMain.RingStateChanged (currentState);
				break;
			case GameState.Shoot:
				currentState = GameState.ViewResult;
				mngrMain.RingStateChanged (currentState);
				break;
			case GameState.ViewResult:
				currentState = GameState.None;
				StartCoroutine(ResetRing ());
				mngrMain.RingStateChanged (currentState);
				break;
			}
		}

		void RotateField(){
			int delta = factor == 1 ? 0 : 180;
			transform.rotation = Quaternion.Euler(new Vector3 (
					0f, 
					delta + Mathf.Clamp(((Input.mousePosition.x / Screen.width) - 0.5f) * 170f, -85f, 85f), //[-85, 85]
					0f
				));
		}

		void RotateRing(){
			float AngleRad = Mathf.Atan2(Input.mousePosition.y - Screen.height / 2, Input.mousePosition.x - Screen.width / 2);
			float AngleDeg = (180 / Mathf.PI) * AngleRad;

			Vector3 rotation = transform.rotation.eulerAngles;
			transform.rotation = Quaternion.Euler(rotation.x, rotation.y, AngleDeg);
		}

		void MoveRing (){

			transform.position = new Vector3(
				factor * (Mathf.Clamp(Input.mousePosition.x / Screen.width, 0.2f, 0.8f) - 0.5f) + initPos.x,
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
			NextState ();

			Vector3 ring_direction = this.transform.forward.normalized;
			float angle = (Vector2.SignedAngle (new Vector2 (0f, 1f), direction))/3f;
			Vector3 velocity_direction = Quaternion.AngleAxis (-angle, Vector3.up) * ring_direction;

			float speed = Mathf.Min(1000 * 2 * 2 / time, 25);
			Vector3 av = new Vector3(-Mathf.Sin(this.transform.rotation.eulerAngles.z / 180 * Mathf.PI), Mathf.Cos(this.transform.rotation.eulerAngles.z / 180 * Mathf.PI), 0);
			av.Scale(new Vector3(speed * 10, speed * 10, speed * 10));
			Vector3 velocity = velocity_direction * speed;

			myRigidbody.isKinematic = false;
			myRigidbody.angularVelocity = av;
			myRigidbody.velocity = velocity;
		}

		public void ForceReset(){
			currentState = GameState.None;
			StartCoroutine (ResetRing ());
		}

		IEnumerator ResetRing(){
			yield return new WaitForSeconds (2f);

			myRigidbody.isKinematic = true;
			myCollider.enabled = false;
			transform.rotation = initRot;
			transform.position = initPos;
		}
	}
}

