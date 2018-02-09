using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnlinePacket
{
	public int id;
	//1 - ring
	//2 - field

	//Ring
	public Vector3 position;
	public Vector3 rotation;
	public Vector3 velocity;

	//Field
	public int cola_score;
	public int pepsi_score;
	public string cola_bottles;
	public string pepsi_bottles;

	public OnlinePacket(int id, Vector3 position, Vector3 rotation, Vector3 velocity){
		this.id = id;
		this.position = position;
		this.rotation = rotation;
		this.velocity = velocity;
	}

	public OnlinePacket(int id, Score score, string cola_bottles, string pepsi_bottles){
		this.id = id;
		this.cola_score = score.score_cola;
		this.pepsi_score = score.score_pepsi;
		this.cola_bottles = cola_bottles;
		this.pepsi_bottles = pepsi_bottles;
	}
}

