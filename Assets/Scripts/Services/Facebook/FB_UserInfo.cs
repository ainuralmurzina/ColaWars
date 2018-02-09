using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FB_UserInfo 
{
	private string _id 			= string.Empty;
	private string _name 		= string.Empty;
	private string _first_name 	= string.Empty;
	private string _last_name 	= string.Empty;
	private string _picUrl 		= string.Empty;
	private Texture2D _picture	= null;

	//==========================================

	public string ID { 
		get{ return _id;}
	}

	public string Name{ 
		get{ return _name;}
	}

	public string FirstName{ 
		get{ return _first_name;}
	}

	public string LastName{ 
		get{ return _last_name;}
	}

	public string PictureURL{ 
		get{ return _picUrl;}
	}

	public Texture2D Picture{ 
		get{ return _picture;}
		set{ _picture = value;}
	}

	public event Action<FB_UserInfo> OnProfileImageLoaded = delegate {};

	//==========================================

	public FB_UserInfo(IDictionary<string,object> JSON) {
		InitializeData(JSON);
	}

	private void InitializeData(IDictionary<string,object> JSON){
		
		if(JSON.ContainsKey("id")) {
			_id 	= JSON["id"].ToString();
		}

		if(JSON.ContainsKey("name")) {
			_name 	= JSON["name"].ToString();
		}

		if(JSON.ContainsKey("first_name")) {
			_first_name 	= JSON["first_name"].ToString();
		}

		if(JSON.ContainsKey("last_name")) {
			_last_name 	= JSON["last_name"].ToString();
		}

		if(JSON.ContainsKey("picture")) {
			IDictionary picDict = JSON["picture"] as IDictionary;
			if (picDict != null && picDict.Contains("data")) {
				IDictionary data = picDict["data"] as IDictionary;
				if (data != null && data.Contains("url")) {
					_picUrl =  data["url"].ToString();
				}
			}
		}
	}

	private void OnPictureLoaded(Texture2D image) {

		if(this == null) {return;}

		_picture = image;

		OnProfileImageLoaded(this);
	}
}

