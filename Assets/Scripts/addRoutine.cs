using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class addRoutine : MonoBehaviour {
	database db;
	public Button addbtn;
	public TextMeshProUGUI sub_code,block,class_name,start_time,day,end_time,error;
	// Use this for initialization
	ArrayList subjects;
	void Start () {
		db = GameObject.FindObjectOfType(typeof(database))as database;
		subjects = db.GetSubjects();
		db.ViewRoutines();

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			sceneChanger scene = GameObject.FindObjectOfType(typeof(sceneChanger))as sceneChanger;
			scene.MenuScene();
		}	
	}

	public void CallAddRoutine(){
		string subject_code,block_string,block_id,class_string,start_string,day_string,end_string;
		int index=0;
		subject_code = sub_code.GetParsedText().Trim();
		class_string = class_name.GetParsedText().Trim();
		start_string = start_time.GetParsedText().Trim();
		end_string = end_time.GetParsedText().Trim();
		day_string = day.GetParsedText().Trim();
		block_string ="Test";
		block_id ="test";
		if(subject_code.Length==0 || class_string.Length==0 || start_string.Length==0||end_string.Length==0||day_string.Length==0){
			error.text ="The input field may be empty.";
		}
		else{
			
			//FOR BLOCK 
			int temp = Convert.ToInt32(block.GetParsedText().Trim());
			if(temp<=14 && temp>0){
				if(temp<10){
					block_string = "Block "+temp;
					block_id="blk0"+temp;
				}
				else{
					block_string = "Block "+temp;
					block_id="blk"+temp;
				}
				index++;
			}
			//FOR Subject Code
			for(int i=0;i<subjects.Count;i++){
				if(subject_code.Trim()==subjects[i].ToString().Trim()){
					index++;
					subject_code=subject_code.Trim();
					break;
				}
			}
			//DAY
			int day_id = GetDayId(day_string.ToUpper());
			//FOR duplicate routines

			if(index==2){
				//subject and block name passed
				if(day_id!=0){
					int checkRoutine = db.CheckRoutine(day_id,block_string,class_string,start_string,end_string);
					if(checkRoutine==1){
						Debug.Log("All correct No class in that time");
						db.AddRoutine(class_string,day_id,subject_code,block_string,block_id,start_string,end_string);
					}
					else{
						Debug.Log("Classes in that time");
						error.text = "Class "+class_string+" of "+block_string+"is not empty at that time";
					}
				}
				else{
					error.text="The day input is incorrect";
				}
			}
			else{
				error.text ="Either the Subject doesnt exist or the block";
			}
		}
		
			
	}

	int GetDayId(string dayname){
		Debug.Log("IN THE GET DAY ID day="+dayname);
		dayname = dayname.Trim();
		if(dayname=="SUNDAY"){
			return 1;
		}
		else if(dayname == "MONDAY"){
			return 2;
		}
		else if(dayname=="TUESDAY"){
			return 3;
		}
		else if(dayname=="WEDNESDAY"){
			return 4;
		}
		else if(dayname == "THURSDAY"){
			return 5;
		}
		else if(dayname== "FRIDAY"){
			return 6;
		}
		else{
			return 0;
		}

	}
}
