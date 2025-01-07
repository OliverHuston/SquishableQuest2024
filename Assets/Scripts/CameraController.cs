using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int targetFrameRate = 20;
    public float camera_sensitivity = 10f;

    [Space]
    [Tooltip("Minimum zoom size (ortho)")]  public float zoom_min = 3f;
    [Tooltip("Maximum zoom size (ortho)")]  public float zoom_max = 20f;
    [Tooltip("Zoom sensitivity")]  public float zoom_sensitivity = 60f;

    private float zoom_min_perspect;
    private float zoom_max_perspect;



    [Space]
    public float drag_sensitivity = 2;


    private bool orthographic = true;


    private Vector3 dragOrigin;
    private bool dragActive = false;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        if (Camera.main.orthographic) { orthographic = true; }
        else { orthographic = false; }
        CalculatePerspectiveZoomMinMax();
    }

    private void CalculatePerspectiveZoomMinMax()
    {
        float fov = 60;
        zoom_min_perspect = Orthographic2Perspective(zoom_min, fov);
        zoom_max_perspect = Orthographic2Perspective(zoom_max, fov);
    }

    private float Orthographic2Perspective(float orthoSize, float fov)
    {   
        return orthoSize / Mathf.Tan(Mathf.Deg2Rad * fov / 2);
    }
    private float Perspective2Orthographic(float perspectiveDistance, float fov)
    {
        return Mathf.Tan(Mathf.Deg2Rad * fov / 2) / perspectiveDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // WSAD move
        if (Input.GetKey(KeyCode.W)) { this.transform.position += new Vector3(0, 0, camera_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.S)) { this.transform.position += new Vector3(0, 0, -camera_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.A)) { this.transform.position += new Vector3(-camera_sensitivity * Time.deltaTime, 0, 0); }
        else if (Input.GetKey(KeyCode.D)) { this.transform.position += new Vector3(camera_sensitivity * Time.deltaTime, 0, 0); }

        // Mouse scroll zoom
        float zoom_amount = 0;
        if(Input.mouseScrollDelta.y != 0) zoom_amount = Time.deltaTime * -zoom_sensitivity * Input.mouseScrollDelta.y / Mathf.Abs(Input.mouseScrollDelta.y);


        if (orthographic)
        {
            zoom_amount = Mathf.Clamp(Camera.main.orthographicSize + zoom_amount, zoom_min, zoom_max);
            Camera.main.orthographicSize = zoom_amount;
        }
        else {
            zoom_amount = Mathf.Clamp(this.transform.position.y + zoom_amount, zoom_min_perspect, zoom_max_perspect);
            this.transform.position = new Vector3(this.transform.position.x, zoom_amount, this.transform.position.z);
        }



        //------------------
        // Drag
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            dragActive = true;
            return;
        }
        if (!Input.GetMouseButton(0))
        {
            dragActive = false;
            return;
        }
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x, 0, -pos.y);
        this.transform.position += move;
    }
}
