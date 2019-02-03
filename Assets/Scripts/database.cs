using UnityEngine;
using UnityEngine.UI;
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

    string conn,temp;

    //FOR THE DYANMIC UI
    public GameObject itemPrefab;
    public TextMeshProUGUI textPrefab;
    public Button btn;

    IDbConnection dbconn;
    IDbCommand dbcmd,dbcmd2;

    string sceneName;
    void Start () {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName=="database")
        {
            StartSearch();
            Debug.Log("In database");
        }
        else if(sceneName == "Main" && openCount ==0){
            if (Application.platform == RuntimePlatform.Android)
            {
                openCount = 1;
                Debug.Log("---------The scene is main and database is to be checked here------------------");
                DatabaseCheck();
            }
        }
	}
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ASDFASDF");
            sceneChanger sch = GameObject.FindObjectOfType(typeof(sceneChanger)) as sceneChanger;
            if (sceneName=="database")
            {
                sch.MapScene();
            }
            else if(sceneName == "MySubject" ||sceneName == "AddRoutine"){
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
        if(dbcmd !=null){
            dbcmd.Dispose();
            dbcmd = null;
        }
        if(dbconn!=null){
            dbconn.Close();
            dbconn = null;
        }
            Debug.Log("-------DATABASE CLOSED------------");
    }

    public ArrayList GetSubjects(){
        ArrayList subjects = new ArrayList();
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string query = "SELECT * FROM subject";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {  
            subjects.Add(reader.GetValue(0).ToString());
        }
        dbconn.Close();
        closeConnection();
        return subjects;
    }

    public int CheckRoutine(int day,string block, string class_name, string start_time,string end_time){
        DateTime start_time_formatted = DateTime.Parse(start_time, System.Globalization.CultureInfo.CurrentCulture);
        DateTime end_time_formatted = DateTime.Parse(end_time, System.Globalization.CultureInfo.CurrentCulture);
        
        startConnection();
        int index=0;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string query = "SELECT * FROM class_routine WHERE day_id="+day;
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
        while(reader.Read()){
            string classdb = reader.GetValue(1).ToString().Trim();
            string blockdb = reader.GetValue(4).ToString().Trim();
            string start_timedb = reader.GetValue(6).ToString().Trim();
            string end_timedb = reader.GetValue(7).ToString().Trim();

            DateTime start_time_formatted_db = DateTime.Parse(start_timedb, System.Globalization.CultureInfo.CurrentCulture);
            DateTime end_time_formatted_db = DateTime.Parse(end_timedb, System.Globalization.CultureInfo.CurrentCulture);

            if(classdb ==class_name && block ==blockdb){
                if(start_time_formatted==start_time_formatted_db || end_time_formatted==end_time_formatted_db){               
                    index++;
                }
                else if(start_time_formatted>start_time_formatted_db && start_time_formatted<end_time_formatted_db){             
                    index++;
                }
                else if(end_time_formatted>start_time_formatted_db && start_time_formatted<end_time_formatted_db){
                    index++;
                }   
            }
        }
        if(index==0){
            return 1;
        }
        else{
            return 0;
        }

    }

    void StartSearch()
    {
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        string dayname = DateTime.Now.DayOfWeek.ToString();
        Debug.Log(dayname);
        string now = DateTime.Now.ToLongTimeString();
        DateTime currentTime= DateTime.Parse(now, System.Globalization.CultureInfo.CurrentCulture);
        dbcmd = dbconn.CreateCommand();

        if (blockname.Length > 0)
        {
            Debug.Log(blockname+"IN LINE 84-----------");
        }
        else
        {
            Debug.Log("Error in pass of value");
            blockname = "Block 9";
            Debug.Log(blockname);
        }
        string query = "SELECT * FROM class_routine INNER JOIN day ON class_routine.day_id = day.day_id WHERE block_name = '" + blockname + "' AND day_name = '"+ dayname+"'";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
        Debug.Log(query);
        while (reader.Read())
        {   
            Debug.Log("ASDFASDF");
            string class_name = reader.GetValue(1).ToString();
            string sub_code = reader.GetString(3);
            string start_time = reader.GetString(6);
            string end_time = reader.GetString(7);

            DateTime start_time_formatted = DateTime.Parse(start_time, System.Globalization.CultureInfo.CurrentCulture);
            DateTime end_time_formatted = DateTime.Parse(end_time, System.Globalization.CultureInfo.CurrentCulture);

            start_time = start_time_formatted.ToString("HH:mm:ss tt");
            end_time = end_time_formatted.ToString("HH:mm:ss tt");

            textPrefab.text = class_name +"\t\t"+ sub_code + "\t\t" + start_time +"\t\t" + end_time;
            GameObject newitems = Instantiate<GameObject>(itemPrefab, transform);
            if(currentTime>=start_time_formatted && currentTime<=end_time_formatted){
                Image testimage = newitems.transform.GetComponent<Image>();
                testimage.color=Color.green;
            }   
        }
        reader.Close();
        reader = null;
        dbconn.Close();
        closeConnection();
    }

    public void mySubject()
    {
        Color orange=new Color(93, 209, 87);

        startConnection();
        string query;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string dayname = DateTime.Now.DayOfWeek.ToString();
        string now = DateTime.Now.ToLongTimeString();
        DateTime currentTime= DateTime.Parse(now, System.Globalization.CultureInfo.CurrentCulture);
        //FOR THE SUBJECTS IN MY SUBJECT
        query = "SELECT * FROM mysubject";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            //Debug.Log(reader.GetString(0));
            temp = reader.GetString(0);
            textPrefab.text = temp;
            textPrefab.fontSize = 36;
            GameObject newitems = Instantiate<GameObject>(itemPrefab, transform);
            Image testimage = newitems.transform.GetComponent<Image>();
            testimage.color = Color.white;
            Button btn = newitems.GetComponentInChildren<Button>() as Button;
            btn.GetComponentInChildren<Text>().text = "Delete";
            //set temp as button id and get button id to use as medium to get the button
            btn.onClick.RemoveAllListeners(); 
            btn.onClick.AddListener(()=>{DeleteMySubject(newitems.GetComponentInChildren<TextMeshProUGUI>().text);});
        
            dbcmd2 = dbconn.CreateCommand();
            string query2 = "SELECT * FROM class_routine WHERE subject_code = '"+temp+"'";
            dbcmd2.CommandText = query2;
            IDataReader reader3 = dbcmd2.ExecuteReader();
            while (reader3.Read())
            {   
                string dayint = reader3.GetValue(2).ToString();
                string day = GetDayName(dayint);
                string block = reader3.GetValue(4).ToString();
                string classname = reader3.GetValue(1).ToString();
                string start_time = reader3.GetString(6);
                string end_time = reader3.GetString(7);

                DateTime start_time_formatted = DateTime.Parse(start_time, System.Globalization.CultureInfo.CurrentCulture);
                DateTime end_time_formatted = DateTime.Parse(end_time, System.Globalization.CultureInfo.CurrentCulture);

                start_time = start_time_formatted.ToString("HH:mm:ss tt");
                end_time = end_time_formatted.ToString("HH:mm:ss tt");

                textPrefab.text = day+"\t"+block+"\tClass: "+classname+"\t"+start_time+"  to  "+end_time;
                textPrefab.fontSize = 25;
                
                GameObject newitem = Instantiate<GameObject>(itemPrefab, transform);
                Image testimage2 = newitem.transform.GetComponent<Image>();
                if(dayname == day){
                    testimage2.color = Color.green; 
                }
                else{
                    testimage2.color = orange;
                }
                Button tempbtn = newitem.GetComponentInChildren<Button>();
                Destroy(tempbtn.gameObject);
            }
            reader3.Close();
            reader3 = null;
            dbcmd2.Dispose();
        }
        reader.Close();
        reader = null;


        dbcmd = dbconn.CreateCommand();
        query = "SELECT * FROM subject WHERE subject_code NOT IN (SELECT subject_code FROM mysubject)";
        dbcmd.CommandText = query;
        IDataReader reader2 = dbcmd.ExecuteReader();
        while (reader2.Read())
        {
            //Debug.Log(reader2.GetString(0));
            temp=reader2.GetString(0);
            textPrefab.text = temp;
            textPrefab.fontSize = 36;
            GameObject newitems = Instantiate<GameObject>(itemPrefab, transform);
            Image testimage = newitems.transform.GetComponent<Image>();
            testimage.color = Color.yellow;
            Button btn = newitems.GetComponentInChildren<Button>();
            btn.GetComponentInChildren<Text>().text = "Add";
            btn.onClick.AddListener(()=>{AddMySubject(newitems.GetComponentInChildren<TextMeshProUGUI>().text);});
        }
        reader2.Close();
        dbconn.Close();
        closeConnection();
        Debug.Log("Closed connection");
    }

    void AddMySubject(string subcode){
        closeConnection();
        Debug.Log("add function started");
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string query = "INSERT INTO mysubject(subject_code) VALUES('"+subcode+"')";
        dbcmd.CommandText = query;
        Debug.Log(query);
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
        closeConnection();
        //refresh the scene
        Debug.Log(subcode);
        SceneManager.LoadScene("MySubject");
    }

    void DeleteMySubject(string subcode){
        closeConnection();
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string query = "DELETE FROM mysubject WHERE subject_code='"+subcode+"'";
        Debug.Log(query);
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
        closeConnection();
        //refresh the scene
        Debug.Log(subcode);
        SceneManager.LoadScene("MySubject");
    }

    public void ViewRoutines()
    {
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string query = "SELECT * FROM class_routine INNER JOIN day ON class_routine.day_id = day.day_id";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
        Debug.Log(blockname);
        Debug.Log("----------------Viewing Routines-----------------");
        while (reader.Read())
        {   
            int routine_id = reader.GetInt32(0);
            string class_name = reader.GetValue(1).ToString();
            string day = GetDayName(reader.GetInt32(2).ToString());
            string block = reader.GetString(4);
            string sub_code = reader.GetString(3);
            string start_time = reader.GetString(6);
            string end_time = reader.GetString(7);

            textPrefab.text = block+"  "+class_name +"  "+day +"  "+sub_code + "  " + start_time +"  " + end_time;
            textPrefab.name=routine_id.ToString();
            Debug.Log(textPrefab.text +"   "+ routine_id+"--------------------------");
            GameObject newitems = Instantiate<GameObject>(itemPrefab, transform);
            Button btn = newitems.GetComponentInChildren<Button>();
            btn.GetComponentInChildren<Text>().text = "Delete";
            btn.onClick.AddListener(()=>{DeleteRoutine(newitems.GetComponentInChildren<TextMeshProUGUI>().name);});
        }
        reader.Close();
        reader = null;
        dbconn.Close();
        closeConnection();
        Debug.Log("----------------DONEEEEE-----------------");
    }

    void DeleteRoutine(string routine_id){
        int rout_id = Convert.ToInt32(routine_id);
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string query = "DELETE FROM class_routine WHERE routine_id='"+rout_id+"'";
        Debug.Log(query);
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
        closeConnection();
        Debug.Log(rout_id);
        //refresh the scene
        SceneManager.LoadScene("AddRoutine");
    }

    public void AddRoutine(string class_number,int day_id, string sub_code,string block_name, string block_id,string start_time,string end_time){
        //get UI values
        Debug.Log(sub_code+blockname+class_number+start_time+end_time);
        startConnection();
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string query = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('"+class_number+"',"+day_id+",'"+sub_code+"','"+block_name+"','"+block_id+"','"+start_time+"','"+end_time+"')";
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
        closeConnection();
        SceneManager.LoadScene("AddRoutine");
    }
    
    string GetDayName(string day){
        if(day=="1"){return "Sunday";}
        else if(day=="2"){return "Monday";}
        else if(day=="3"){return "Tuesday";}
        else if(day=="4"){return "Wednesday";}
        else if(day=="5"){return "Thursday";}
        else if(day=="6"){return "Friday";}
        else {return "Saturday";}
    }

    public void SetBlockName(string temp)
    {
        blockname = temp;
        Debug.Log(blockname);
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
        temp[7] = "CREATE TABLE IF NOT EXISTS `class_routine`(`routine_id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,`class_name` text, `day_id` int,`subject_code` text,`block_name` text,`block_id` text,`start_time` text,`end_time` text)";
        temp[8] = "CREATE TABLE IF NOT EXISTS `mysubject`(`subject_code` text)";

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
        Debug.Log("--------------------Inside the Insert Function----------------------");

        IDataReader reader = dbcmd.ExecuteReader();
        //IF READER CANT FIND ANYTHING THEN POPLUATE TABLE
        if (!reader.Read()) 
        {
            Debug.Log("-------------------The block  is empty so data is to be inserted-----------------");
            string[] query = new string[100];
            query[0] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('Block 9','blk9',3)";
            query[1] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('Block 10','blk10',2)";
            query[2] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('Block 8','blk8',3)";
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
            query[65] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('103',1,'COMP 307','Block 10','blk10','9:00:00','11:00:00')";
            query[66] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('103',1,'COEG 304','Block 10','blk10','12:00:00','14:00:00')";
            query[67] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',1,'COMP 301','Block 9','blk9','14:00:00','16:00:00')";
            query[68] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('103',2,'COMP 307','Block 9','blk9','12:00:00','13:00:00')";
            query[69] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',2,'COMP 315','Block 9','blk9','13:00:00','15:00:00')";
            query[70] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',2,'COMP 301','Block 9','blk9','15:00:00','16:00:00')";
            query[71] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',3,'COMP 315','Block 9','blk9','9:00:00','11:00:00')";
            query[72] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',3,'MGTS 301','Block 9','blk9','14:00:00','16:00:00')";
            query[73] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',4,'COMP 301','Block 9','blk9','9:00:00','11:00:00')";
            query[74] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',4,'MGTS 301','Block 10','blk9','12:00:00','14:00:00')";
            query[75] = "INSERT INTO class_routine(class_name,day_id,subject_code,block_name,block_id,start_time,end_time) VALUES('304',4,'COEG 304','Block 10','blk9','14:00:00','16:00:00')";
            query[76] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('Block 7','blk7',3)";
            query[77] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('Block 6','blk6',3)";
            query[78] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('Block 11','blk11',3)";
            query[79] = "INSERT INTO block(block_name,block_id,no_of_floors) VALUES('Block 12','blk12',3)";
    
            for(int i = 0; i <= 79; i++)
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
