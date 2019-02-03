using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneChanger:MonoBehaviour{

    //public Button MapBtn;
    //public Button MySubject;

    // Use this for initialization
    void Start () {
   
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MapScene()
    {
        SceneManager.LoadScene("Map");
    }

    public void DatabaseScene()
    {
        SceneManager.LoadScene("database");
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void MySubjectScene()
    {
        SceneManager.LoadScene("MySubject");
    }

    public void AddRoutineScene(){
        SceneManager.LoadScene("AddRoutine");
    }


}
