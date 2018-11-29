using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneChanger:MonoBehaviour{

    public Button btn1;

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

    public void BlockClick()
    {
        SceneManager.LoadScene("database");
    }


}
