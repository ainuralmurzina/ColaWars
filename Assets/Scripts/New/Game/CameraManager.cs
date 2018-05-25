using UnityEngine;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace Test{
	public class CameraManager : MonoBehaviour
	{

		public CinemachineVirtualCamera vcam_modify_ring;
		public CinemachineVirtualCamera vcam_follow_ring;
		public CinemachineVirtualCamera vcam_observe_field;
		public CinemachineVirtualCamera vcam_field;

		private List<CinemachineVirtualCamera> vcams = new List<CinemachineVirtualCamera> ();

		void Start(){
			vcams.Add (vcam_modify_ring);
			vcams.Add (vcam_follow_ring);
			vcams.Add (vcam_observe_field);
			vcams.Add (vcam_field);
		}

		void DisableAllCamerasExcept(CinemachineVirtualCamera vcam){
			foreach (CinemachineVirtualCamera cam in vcams)
				if (cam != vcam)
					cam.gameObject.SetActive (false);
		}

		public void ShowRing(Transform obj){
			vcam_modify_ring.Follow = obj;
			vcam_modify_ring.LookAt = obj;

			vcam_modify_ring.gameObject.SetActive (true);
			DisableAllCamerasExcept (vcam_modify_ring);
		}

		public void FollowRing(Transform obj){
			vcam_follow_ring.Follow = obj;
			vcam_follow_ring.LookAt = obj;

			CinemachineTransposer transposer = vcam_follow_ring.GetCinemachineComponent<CinemachineTransposer> ();
			Vector3 offset = transposer.m_FollowOffset;
			offset.z = Mathf.Sign(obj.transform.position.z) * Mathf.Abs(offset.z);
			transposer.m_FollowOffset = offset;

			vcam_follow_ring.gameObject.SetActive (true);
			DisableAllCamerasExcept (vcam_follow_ring);
		}

		public void ShowOpponentRing(){

		}

		public void ShowField(){
			vcam_field.gameObject.SetActive (true);
			DisableAllCamerasExcept (vcam_field);
		}

		public void ShowFieldFromRing(Transform obj){
			vcam_observe_field.Follow = obj;
			vcam_observe_field.LookAt = obj;

			vcam_observe_field.gameObject.SetActive (true);
			DisableAllCamerasExcept (vcam_observe_field);
		}
			
	}
}

