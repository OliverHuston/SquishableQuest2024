using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int targetFrameRate = 20;
    public float camera_movement_sensitivity = 10f;

    [Space]
    [Tooltip("Minimum zoom size (ortho)")]  public float zoom_min = 3f;
    [Tooltip("Maximum zoom size (ortho)")]  public float zoom_max = 20f;
    [Tooltip("Zoom sensitivity")]  public float zoom_sensitivity = 60f;

    private float zoom_min_perspect;
    private float zoom_max_perspect;


    private bool orthographic = true;

    //
    private Vector3 dragClickOrigin;
    private Vector3 dragOldPosition;
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
        float fov = Camera.main.fieldOfView;
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



    void LateUpdate()
    {
        //CAMERA ZOOM
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


        // CAMERA MOVEMENT
        // WSAD move
        if (Input.GetKey(KeyCode.W)) { this.transform.position += new Vector3(0, 0, camera_movement_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.S)) { this.transform.position += new Vector3(0, 0, -camera_movement_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.A)) { this.transform.position += new Vector3(-camera_movement_sensitivity * Time.deltaTime, 0, 0); }
        else if (Input.GetKey(KeyCode.D)) { this.transform.position += new Vector3(camera_movement_sensitivity * Time.deltaTime, 0, 0); }

        // Drag
        if (Input.GetMouseButtonDown(0))
        {
            dragActive = true;
            dragOldPosition = this.transform.position;
            dragClickOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - dragClickOrigin;
            this.transform.position = dragOldPosition - new Vector3(pos.x, 0, pos.y) * camera_movement_sensitivity;
        }
        else
        {
            dragActive = false;
        }
    }
}
