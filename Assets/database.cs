using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections;
using System;
using System.Data;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class database : MonoBehaviour {

    int openCount = 0;
    public static string blockname = "";

    string conn;

    //FOR THE DYANMIC UI
    public GameObject itemPrefab;
    public TextMeshProUGUI textPrefab;

    IDbConnection dbconn;
    IDbCommand dbcmd;

    string sceneName;
    void Start () {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName=="database")
        {
            StartSearch();
        }
        else if(sceneName == "MySubject")
        {
            mySubject();
        }
        else if(sceneName == "Main" && openCount ==0){
            openCount = 1;
            Debug.Log("---------The scene is main and database is to be checked here------------------");
            DatabaseCheck();
        }
	}
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneChanger sch = GameObject.FindObjectOfType(typeof(sceneChanger)) as sceneChanger;
            if (sceneName=="database")
            {
                sch.MapScene();
            }
            else if(sceneName == "MySubject"){
                sch.MenuScene();
            }
        }
    }

    void startConnection()
    {
        string connection;
        if (Application.platform != RuntimePlatform.Android)
        {

            connection = Application.dataPath + "/StreamingAssets/db.s3db";
        }
        else
        {
            connection = Application.persistentDataPath + "/routine";
        }

        conn = "URI=file:" + connection;
    }

    void closeConnection()
    {
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            Debug.Log("-------DATABASE CLOSED------------");
    }
    void StartSearch()
    {
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        //hgdjgcv
        dbcmd = dbconn.CreateCommand();

        if (blockname.Length > 0)
        {
            Debug.Log(blockname+"IN LINE 84-----------");
        }
        else
        {
            Debug.Log("Error in pass of value");
            blockname = "block09";
            Debug.Log(blockname);
        }
        string query = "SELECT * FROM class_routine WHERE block_name = '" + blockname + "'";
        Debug.Log(query+"IN LINE 92-----------");
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
        Debug.Log(query+"IN LINE 96-----------");
        while (reader.Read())
        {   
            Debug.Log("-----------------------DATA HAS BEEN FOUND-------------------------");
            string class_name = reader.GetValue(0).ToString();
            string sub_code = reader.GetString(2);
            string start_time = reader.GetDateTime(5).ToLongTimeString();
            //DateTime end_time = reader.GetDateTime(6);
            //string end_time = reader.GetDateTime(6).ToString();
            Debug.Log("---------------Class = " + class_name + " Subject: " + sub_code + " Start: " + start_time);
            textPrefab.text = class_name +"\t\t\t"+ sub_code + "\t\t\t" + start_time;
            GameObject newitems = Instantiate<GameObject>(itemPrefab, transform);
        }
        reader.Close();
        reader = null;
    }

    void mySubject()
    {
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string query = "SELECT * FROM mysubject";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log(reader.GetString(0));
            textPrefab.text = reader.GetString(0);
            GameObject newitems = Instantiate<GameObject>(itemPrefab, transform);
        }

        closeConnection();
    }

    public void SetBlockName(string temp)
    {
        blockname = temp;
    }

    void DatabaseCheck(){
        Debug.Log("--------------LINE 136---------------------");
        CreateTables();
        InsertData();
        Debug.Log("--------------LINE 138---------------------");
    }

    void CreateTables()
    {
        startConnection();   
        dbconn = (IDbConnection)new SqliteConnection(conn);
        Debug.Log("-----------------------CREATINGGGGGGGG TABLESSSSS-------------------------");

        dbconn.Open();
        string[] temp = new string[10];
        temp[0] = "CREATE TABLE IF NOT EXISTS `block`(`block_name` text NULL,`block_id` text NULL,`no_of_floors` int NULL,PRIMARY KEY(`block_name`,`block_id`))";
        temp[1] = "CREATE TABLE IF NOT EXISTS `class` (`class_name` text  NULL,primary key(`class_name`))";
        temp[2] = "CREATE TABLE IF NOT EXISTS `day` (`day_id` int  NULL,`day_name` text  NULL,primary key(`day_id`))";
        temp[3] = "CREATE TABLE IF NOT EXISTS `department` (`department_code` text  NULL,`department_name` text  NULL,primary key(`department_code`))";
        temp[4] = "CREATE TABLE IF NOT EXISTS `subject` (`subject_code` text  NULL,`subject_name` text  NULL,primary key(`subject_code`))";
        temp[5] = "CREATE TABLE IF NOT EXISTS `teacher` (`teacher_id` int  NULL,`teacher_name` text  NULL,primary key(`teacher_id`,`teacher_name`))";
        temp[6] = "CREATE TABLE IF NOT EXISTS `teaches`(`teacher_id` int,`teacher_name` text,`subject_code` text,foreign key(`teacher_id`,`teacher_name`) references `teacher`(`teacher_id`,`teacher_name`),foreign key(`subject_code`) references `subject`(`subject_code`))";
        temp[7] = "CREATE TABLE IF NOT EXISTS `class_routine`(`class_name` text, `day_id` int,`subject_code` text,`block_name` text,`block_id` text,`start_time` time,`end_time` time,foreign key (`class_name`) references `class`(`class_name`),foreign key (`day_id`) references `day`(`day_id`), foreign key (`subject_code`) references `subject`(`subject_code`), foreign key(`block_name`,`block_id`) references `block`(`block_name`,`block_id`))";
        temp[8] = "CREATE TABLE IF NOT EXISTS `mysubject`(`subject_code` text,foreign key (`subject_code`) references `subject`(`subject_code`))";

        for(int i = 0; i <= 8; i++)
        {
            dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = temp[i];
            dbcmd.ExecuteReader();
             Debug.Log("-----------------CREATING TABLE NUMBERED"+i.ToString()+"------------------");
        }
        Debug.Log("-----------------------CREATED TABLESSSSS-------------------------");
        closeConnection();

    }

    void InsertData()
    {
        startConnection();   
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        //CHECK IF THE TABLES ARE EMPTY TO POPULATE
        dbcmd = dbconn.CreateCommand();
        string temp = "SELECT * FROM block";
        dbcmd.CommandText = temp;
        Debug.Log("--------------------Inside the Insert Fcuntion---------------");

        IDataReader reader = dbcmd.ExecuteReader();
        //IF READER CANT FIND ANYTHING THEN POPLUATE TABLE
        if (!reader.Read()) 
        {
            Debug.Log("-------------------The block  is empty so data is to be inserted-----------------");
            string[] query = new string[100];
            query[0] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('block09','blk9',3)";
            query[1] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('block10','blk10',2)";
            query[2] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('block08','blk8',3)";
            query[3] = "INSERT INTO class(class_name) VALUES('100')";
            query[4] = "INSERT INTO class(class_name) VALUES('101')";
            query[5] = "INSERT INTO class(class_name) VALUES('102')";
            query[6] = "INSERT INTO class(class_name) VALUES('103')";
            query[7] = "INSERT INTO class(class_name) VALUES('104')";
            query[8] = "INSERT INTO class(class_name) VALUES('105')";
            query[9] = "INSERT INTO class(class_name) VALUES('106')";
            query[10] = "INSERT INTO class(class_name) VALUES('107')";
            query[11] = "INSERT INTO class(class_name) VALUES('108')";
            query[12] = "INSERT INTO class(class_name) VALUES('109')";
            query[13] = "INSERT INTO class(class_name) VALUES('110')";
            query[14] = "INSERT INTO class(class_name) VALUES('200')";
            query[15] = "INSERT INTO class(class_name) VALUES('201')";
            query[16] = "INSERT INTO class(class_name) VALUES('202')";
            query[17] = "INSERT INTO class(class_name) VALUES('203')";
            query[18] = "INSERT INTO class(class_name) VALUES('204')";
            query[19] = "INSERT INTO class(class_name) VALUES('205')";
            query[20] = "INSERT INTO class(class_name) VALUES('206')";
            query[21] = "INSERT INTO class(class_name) VALUES('207')";
            query[22] = "INSERT INTO class(class_name) VALUES('208')";
            query[23] = "INSERT INTO class(class_name) VALUES('209')";
            query[24] = "INSERT INTO class(class_name) VALUES('210')";
            query[25] = "INSERT INTO class(class_name) VALUES('300')";
            query[26] = "INSERT INTO class(class_name) VALUES('301')";
            query[27] = "INSERT INTO class(class_name) VALUES('302')";
            query[28] = "INSERT INTO class(class_name) VALUES('303')";
            query[29] = "INSERT INTO class(class_name) VALUES('304')";
            query[30] = "INSERT INTO class(class_name) VALUES('305')";
            query[31] = "INSERT INTO class(class_name) VALUES('306')";
            query[32] = "INSERT INTO class(class_name) VALUES('307')";
            query[33] = "INSERT INTO class(class_name) VALUES('308')";
            query[34] = "INSERT INTO class(class_name) VALUES('309')";
            query[35] = "INSERT INTO class(class_name) VALUES('310')";
            query[36] = "INSERT INTO class(class_name) VALUES('400')";
            query[37] = "INSERT INTO class(class_name) VALUES('401')";
            query[38] = "INSERT INTO class(class_name) VALUES('402')";
            query[39] = "INSERT INTO class(class_name) VALUES('403')";
            query[40] = "INSERT INTO class(class_name) VALUES('404')";
            query[41] = "INSERT INTO day(day_id,day_name) VALUES(1,'Sunday')";
            query[42] = "INSERT INTO day(day_id,day_name) VALUES(2,'Monday')";
            query[43] = "INSERT INTO day(day_id,day_name) VALUES(3,'Tuesday')";
            query[44] = "INSERT INTO day(day_id,day_name) VALUES(4,'Wednesday')";
            query[45] = "INSERT INTO day(day_id,day_name) VALUES(5,'Thursday')";
            query[46] = "INSERT INTO day(day_id,day_name) VALUES(6,'Friday')";
            query[47] = "INSERT INTO day(day_id,day_name) VALUES(7,'Saturday')";
            query[48] = "INSERT INTO department(department_code,department_name) VALUES('DOCSE','Department of Computer Science and Engineering')";
            query[49] = "INSERT INTO department(department_code,department_name) VALUES('DOEE','Department of Electical Engineering')";
            query[50] = "INSERT INTO subject(subject_code,subject_name) VALUES('COMP 301','Principles of Programming Language')";
            query[51] = "INSERT INTO subject(subject_code,subject_name) VALUES('COMP 307', 'Operating Systems')";
            query[52] = "INSERT INTO subject(subject_code,subject_name) VALUES('COMP 315', 'Computer Design and Architecture')";
            query[53] = "INSERT INTO subject(subject_code,subject_name) VALUES('MGTS 301', 'Engineering Economics')";
            query[54] = "INSERT INTO subject(subject_code,subject_name) VALUES('COEG 304', 'Instrumentaion and Control')";
            query[55] = "INSERT INTO teacher(teacher_id,teacher_name) VALUES(1,'Dr. Rajani Chulyadyo')";
            query[56] = "INSERT INTO teacher(teacher_id,teacher_name) VALUES(2,'Namrata Tusuju Shrestha')";
            query[57] = "INSERT INTO teacher(teacher_id,teacher_name) VALUES(3,'Pankaj Raj Duwadi')";
            query[58] = "INSERT INTO teacher(teacher_id,teacher_name) VALUES(4,'Nabin Ghimire')";
            query[59] = "INSERT INTO teacher(teacher_id,teacher_name) VALUES(5,'Bibhu Ratna Tulhadhar')";
            query[60] = "INSERT INTO teaches(teacher_id,teacher_name,subject_code) VALUES(1,'Dr. Rajani Chulyadyo','COMP 307')";
            query[61] = "INSERT INTO teaches(teacher_id,teacher_name,subject_code) VALUES(4,'Nabin Ghimire','COMP 301')";
            query[62] = "INSERT INTO teaches(teacher_id,teacher_name,subject_code) VALUES(2,'Namrata Tusuju Shrestha','COEG 304')";
            query[63] = "INSERT INTO teaches(teacher_id,teacher_name,subject_code) VALUES(3,'Pankaj Raj Duwadi','COMP 315')";
            query[64] = "INSERT INTO teaches(teacher_id,teacher_name,subject_code) VALUES(5,'Bibhu Ratna Tulhadhar','MGTS 301')";
            query[65] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('103',1,'COMP 307','block10','blk10','9:00:00','11:00:00')";
            query[66] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('103',1,'COEG 304','block10','blk10','12:00:00','14:00:00')";
            query[67] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',1,'COMP 301','block09','blk9','14:00:00','16:00:00')";
            query[68] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('103',2,'COMP 307','block09','blk9','12:00:00','13:00:00')";
            query[69] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',2,'COMP 315','block09','blk9','13:00:00','15:00:00')";
            query[70] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',2,'COMP 301','block09','blk9','15:00:00','16:00:00')";
            query[71] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',3,'COMP 315','block09','blk9','9:00:00','11:00:00')";
            query[72] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',3,'MGTS 301','block09','blk9','14:00:00','16:00:00')";
            query[73] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',4,'COMP 301','block09','blk9','9:00:00','11:00:00')";
            query[74] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',4,'MGTS 301','block09','blk9','12:00:00','14:00:00')";
            query[75] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',4,'COEG 304','block09','blk9','14:00:00','16:00:00')";
        
            for(int i = 0; i <= 75; i++)
            {
                dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = query[i];
                dbcmd.ExecuteNonQuery();
                 Debug.Log("-------------Running query number"+i+"----------------------");
            }
            Debug.Log("ALL Values inserted heree-----------------------------------------------------------");
            
        }
        closeConnection();
    }
}
