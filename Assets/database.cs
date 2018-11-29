using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class database{
    public static string blockname ="";
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startsearch()
    {
        string conn = "URI=file:" + Application.dataPath + "/db.s3db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        if (blockname.Length > 0)
        {
            Debug.Log(blockname);
        }
        else
        {
            Debug.Log("Error in pass of value");
        }

        string query = "SELECT * FROM class_routine WHERE block_name = '"+blockname+"'";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string class_name = reader.GetValue(0).ToString();
            string sub_code = reader.GetString(2);
            DateTime start_time = reader.GetDateTime(5);
            string s_time = start_time.ToString();
            //string end_time = reader.GetDateTime(6).ToString();
            Debug.Log("Class = " + class_name + " Subject: " + sub_code + " Start: " + s_time);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public void SetBlockName(string temp)
    {
        blockname = temp;
    }
}
