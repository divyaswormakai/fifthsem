using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class database : MonoBehaviour {
    string blockname;
	// Use this for initialization
	void Start () {
    /*    string conn = "URI=file:" + Application.dataPath + "/plugins/test.s3db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string query = "SELECT * FROM test";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            Debug.Log("ID=" + id + "   Name" + name);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;*/
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void block9()
    {
        blockname = "block9";
    }

    public void block10()
    {
        blockname = "block10";
    }
}
