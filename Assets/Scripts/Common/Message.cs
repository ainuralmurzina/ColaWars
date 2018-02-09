using UnityEngine;
using System.Collections;

public class Message
{
	public MonoBehaviour sender {get; private set;}
	public int id {get; private set;}
	public System.Object data {get; private set;}
	public System.Object data2 {get; private set;}

	public Message (MonoBehaviour sender, int id, System.Object data = null, System.Object data2 = null) {
		this.sender = sender;
		this.id = id;
		this.data = data;
		this.data2 = data2;
	}

	public static Message Create (MonoBehaviour sender, int id, System.Object data = null, System.Object data2 = null) {
		return new Message (sender, id, data, data2);
	}
}

