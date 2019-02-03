using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    public Camera cam;
    public NavMeshAgent agent;
    public TrailRenderer path;
    public Transform target;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            path.Clear();
            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 newPos;
            newPos = cam.WorldToScreenPoint(target.position);
            print(newPos);
            Ray ray = cam.ScreenPointToRay(newPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                print(hit.point);
                agent.SetDestination(hit.point);
                //Vector3 newPos = new Vector3(target.position.x,target.position.y - 1.4f,target.position.z);
                //print(newPos);
                //agent.SetDestination(newPos);
            }
            
        }
    }
}
