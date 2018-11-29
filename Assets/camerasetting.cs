using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class camerasetting : MonoBehaviour
{
    //
    // VARIABLES
    //
    public Camera cam;
    public float turnSpeed =0.5f;      // Speed of camera turning when mouse moves in along an axis
    public float panSpeed = 0.5f;       // Speed of the camera when being panned
    public float zoomSpeed = 0.5f;      // Speed of the camera going back and forth
    public float RotateAmount = 0.5f;

    private bool selectindex;
    private bool zoomindex;
    private bool panindex;   
    private bool orbitindex;     
    private bool rotateindex;

    TextMeshProUGUI txt;
    Button tempButton;
    //
    // UPDATE
    //
    private void Start()
    {
        txt = GameObject.Find("text").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //---------------------------------------------
        //BUTTON
        if (zoomindex)
        {
            if (Input.touchCount == 2)
            {
                Touch touchone = Input.GetTouch(0);
                Touch touchtwo = Input.GetTouch(1);

                Vector2 oneprevpos = touchone.position - touchone.deltaPosition;
                Vector2 twoprevpos = touchtwo.position - touchtwo.deltaPosition;

                float prevTouchMag = (twoprevpos - oneprevpos).magnitude;
                float newTouchMag = (touchone.position - touchtwo.position).magnitude;

                float magDiff = prevTouchMag - newTouchMag;

                cam.fieldOfView += magDiff * zoomSpeed;
            }
        }

        foreach (Touch touch in Input.touches)
        {
            // Rotate camera along X and Y axis
            if (rotateindex)
            {
                Vector3 pos = touch.deltaPosition;

                cam.transform.RotateAround(cam.transform.position, cam.transform.right, -pos.y * turnSpeed);
                cam.transform.RotateAround(cam.transform.position, Vector3.up, pos.x * turnSpeed);
            }

            // Move the camera on it's XY plane
            if (panindex)
            {
                Vector3 pos = touch.deltaPosition;

                Vector3 move = new Vector3(-pos.x * panSpeed, -pos.y * panSpeed, 0);
                cam.transform.Translate(move, Space.Self);
            }

            // Move the camera linearly along Z axis


            if (orbitindex)
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

    public void BtnPress(Button btn)
    {
        if (tempButton)
        {
            tempButton.image.color = Color.white;
        }
        tempButton = btn;
        btn.image.color = Color.red;
        string btnName = btn.name;
        txt.text = "CURRENTLY: ";
        if (btnName == "select")
        {
            selectindex = true;
            zoomindex = panindex = orbitindex = rotateindex = false;
            txt.text += "Select Building";
        }
        else if (btnName == "pan")
        {
            panindex = true;
            zoomindex = selectindex = orbitindex = rotateindex = false;
            txt.text += "Panning";
        }
        else if (btnName == "zoom")
        {
            zoomindex = true;
            panindex = selectindex = orbitindex = rotateindex = false;
            txt.text += "Zooming";
        }
        else if (btnName == "rotate")
        {
            rotateindex = true;
            panindex = selectindex = orbitindex = zoomindex = false;
            txt.text += "Rotating";
        }
        else if (btnName == "orbit")
        {
            orbitindex = true;
            panindex = selectindex = zoomindex = rotateindex = false;
            txt.text += "Orbiting";
        }
        
    }


}
