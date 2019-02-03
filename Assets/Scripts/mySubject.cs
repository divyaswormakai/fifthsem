using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mySubject : MonoBehaviour {

	database db;
	void Start () {
		 db = GameObject.FindObjectOfType(typeof(database))as database;
		 db.mySubject();
  	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
