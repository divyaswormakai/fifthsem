using UnityEngine;
using System.Collections;

public class MyCamera : MonoBehaviour
{
    //
    // VARIABLES
    //
    public Camera cam;
    public float turnSpeed = 0.5f;      // Speed of camera turning when mouse moves in along an axis
    public float panSpeed = -0.5f;       // Speed of the camera when being panned
    public float zoomSpeed = 0.5f;      // Speed of the camera going back and forth
    public float RotateAmount = 0.25f;
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    private bool isPanning = false;     // Is the camera being panned?
    private bool isRotating = true;    // Is the camera being rotated?
    private bool isZooming = false;     // Is the camera zooming?
    private bool isOrbiting = false;


    //
    // UPDATE
    //
    void Update(){
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Ray raycast = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit raycasthit;
                if (Physics.Raycast(raycast, out raycasthit))
                {
                    if(raycasthit.collider.name.ToString() != "Ground"){
                        database db = GameObject.FindObjectOfType(typeof(database)) as database;
                        db.SetBlockName(raycasthit.collider.name.ToString());
                        Debug.Log(raycasthit.collider.name.ToString());
                        sceneChanger sch=GameObject.FindObjectOfType(typeof(sceneChanger)) as sceneChanger;
                        sch.DatabaseScene();
                    }

                } 
            } 
        }
    }

    void FixedUpdate()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (cam.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 30f, 110f);
            }
        }
        else if(Input.touchCount==1)
        {
            Touch touch = Input.GetTouch(0);
            // Rotate camera along X and Y axis
            if (isRotating)
            {
                Vector3 pos = touch.deltaPosition;

                cam.transform.RotateAround(cam.transform.position, cam.transform.right, -pos.y * turnSpeed);
                cam.transform.RotateAround(cam.transform.position, Vector3.up, pos.x * turnSpeed);
            }

            // Move the camera on it's XY plane
            if (isPanning)
            {
                Vector3 pos = touch.deltaPosition;

                Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
                cam.transform.Translate(move, Space.Self);
            }

            // Move the camera linearly along Z axis
            if (isZooming)
            {
                Vector3 pos = touch.deltaPosition;

                Vector3 move = pos.y * zoomSpeed * cam.transform.forward;
                cam.transform.Translate(move, Space.World);
            }

            if (isOrbiting)
            {
                Vector3 target = Vector3.zero; //this is the center of the scene, you can use any point here
                float y_rotate = touch.deltaPosition.x * RotateAmount;
                float x_rotate = touch.deltaPosition.y * RotateAmount;

                Vector3 angles = transform.eulerAngles;
                angles.z = 0;
                cam.transform.eulerAngles = angles;
                cam.transform.RotateAround(target, Vector3.up, y_rotate);
                cam.transform.RotateAround(target, Vector3.left, x_rotate);

                cam.transform.LookAt(target);
            }
        }
    }

    public void Chooser(int Index)
    {
        if(Index == 0)
        {
            isRotating = true;
            isOrbiting = isPanning = isZooming = false;
        }
        else if(Index == 1)
        {
            isZooming = true;
            isOrbiting = isPanning = isRotating = false;
        }
        else if (Index == 2)
        {
            isPanning = true;
            isOrbiting = isRotating = isZooming = false;
        }
        else if (Index == 3)
        {
            isOrbiting = true;
            isRotating = isPanning = isZooming = false;
        }
    }
}
