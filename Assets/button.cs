using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class button : MonoBehaviour {

    Button select, zoom, pan, rotate, orbit;
    int selectindex =0, zoomindex = 0,panindex= 0,rotateindex=0, orbitindex=0;
	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void btnPress(Button btn)
    {
        string btnName = btn.name;
        if (btnName == "select")
        {
            selectindex = 1;
            zoomindex = panindex = orbitindex = rotateindex = 0;
        } 
        else if(btnName == "pan")
        {
            panindex = 1;
            zoomindex = selectindex = orbitindex = rotateindex = 0;
        }
        else if(btnName == "zoom")
        {
            zoomindex = 1;
            panindex = selectindex = orbitindex = rotateindex = 0;
        }
        else if (btnName == "rotate")
        {
            rotateindex = 1;
            panindex = selectindex = orbitindex = zoomindex = 0;
        }
        else if (btnName == "orbit")
        {
            orbitindex = 1;
            panindex = selectindex = zoomindex = rotateindex = 0;
        }
    }

    
}
