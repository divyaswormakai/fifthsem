using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class btn : MonoBehaviour {

    public Button btn1;
    Vector2 place;

    // Use this for initialization
    void Start () {
   
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Btn1Click()
    {
        SceneManager.LoadScene("Map");
    }


}
