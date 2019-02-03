using UnityEngine;

public class Click2Glow : MonoBehaviour
{
    // Toggle between Diffuse and Transparent/Diffuse shaders
    // when space key is pressed

    public Shader shader1;
    public Shader shader2;
    public Camera cam;
    Renderer rend;
    Material mat; 

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                
                GameObject hitObject = hit.transform.root.gameObject;
                rend = hitObject.GetComponent<Renderer> ();
                if (rend.material.shader == shader1)
                {
                    rend.material.shader = shader2;
                }
                else if (rend.material.shader == shader2)
                {
                    rend.material.shader = shader1;
                }
            }
            
        }
    }
}