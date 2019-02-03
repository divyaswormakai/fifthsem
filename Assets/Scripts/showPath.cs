using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class showPath : MonoBehaviour
{
     //find your dropdown object
    public Dropdown Source,Destination;
    
    public Camera cam;
    public GameObject capsule;    
    public NavMeshAgent agent;
    public TrailRenderer path;
    
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            sceneChanger sch=GameObject.FindObjectOfType(typeof(sceneChanger)) as sceneChanger;
            sch.MenuScene();
        }
    }

    public void SetPath(){
        path.Clear();
        string sourceName = Source.options[Source.value].text;
        string destinationName = Destination.options[Destination.value].text;
        GameObject source = GameObject.Find(sourceName);
        GameObject destination = GameObject.Find(destinationName);
       // Debug.Log(destination);
        Vector3 sourcePos = source.transform.position;
        Vector3 destinationPos = destination.transform.position;
        
        // sourcePos = cam.WorldToScreenPoint(sourcePos);
        // Ray rayS = cam.ScreenPointToRay(sourcePos);
        // RaycastHit hitS;
        // if (Physics.Raycast(rayS, out hitS))
        // {
        //     print(hitS.point);
        
        // }
        print(sourcePos);
        Vector3 temp = new Vector3(sourcePos.x, 0.59f, sourcePos.z);
        agent.transform.position = temp;
        Vector3 newPos;
        newPos = cam.WorldToScreenPoint(destinationPos);
        Ray ray = cam.ScreenPointToRay(newPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Path")))
        {
            path.Clear();
            //print(hit.point);
            Vector3 dest = new Vector3 (hit.point.x, 0.59f, hit.point.z);
            agent.SetDestination(dest);
        }
    }
}
