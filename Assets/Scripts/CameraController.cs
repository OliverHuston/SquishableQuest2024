using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 20;
    [SerializeField] private float camera_movement_sensitivity = 10f;

    [Space]
    [Tooltip("Minimum zoom size (ortho)")][SerializeField] private float zoom_min = 3f;
    [Tooltip("Maximum zoom size (ortho)")][SerializeField] private float zoom_max = 20f;
    [Tooltip("Zoom sensitivity")][SerializeField]  private float zoom_sensitivity = 60f;
    [SerializeField] private bool correct_sensitivity_for_zoom = false;

    [Space]
    [SerializeField] private bool view_clamped = false;
    [SerializeField] private float x_min;
    [SerializeField] private float x_max;
    [SerializeField] private float y_min;
    [SerializeField] private float y_max;


    //-----------------------------//
    private float zoom_min_perspect;
    private float zoom_max_perspect;
    private bool orthographic = true;

    private Vector3 dragClickOrigin;
    private Vector3 dragOldPosition;

    private float zoom_amount;
    private float zoom_percentage;

    public static CameraController instance { get; private set; }


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one CameraManager in the scene.");
        }
        instance = this;

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
        float zoom_change = 0;
        if(Input.mouseScrollDelta.y != 0) zoom_change = Time.deltaTime * -zoom_sensitivity * Input.mouseScrollDelta.y / Mathf.Abs(Input.mouseScrollDelta.y);

        if (orthographic)
        {
            zoom_change = Mathf.Clamp(Camera.main.orthographicSize + zoom_change, zoom_min, zoom_max);
            Camera.main.orthographicSize = zoom_change;

            zoom_amount = zoom_change;
            zoom_percentage = (zoom_amount - zoom_min)/(zoom_max - zoom_min);


            // Zoom to point
/*            if (Input.mouseScrollDelta.y != 0)
            {
                Vector3 cursorPos = Input.mousePosition;
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(cursorPos.x, 0, cursorPos.z));
                
                float step = zoom_sensitivity * 1000f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, cursorWorldPos, step);
            }*/
        }
        else {
            zoom_change = Mathf.Clamp(this.transform.position.y + zoom_change, zoom_min_perspect, zoom_max_perspect);
            this.transform.position = new Vector3(this.transform.position.x, zoom_change, this.transform.position.z);

            zoom_amount = zoom_change;
            zoom_percentage = (zoom_amount - zoom_min_perspect) / (zoom_max_perspect - zoom_min_perspect);
        }

        // CAMERA MOVEMENT
        float corrected_cam_movement_sensitivity = camera_movement_sensitivity;
        if (correct_sensitivity_for_zoom) corrected_cam_movement_sensitivity = camera_movement_sensitivity * zoom_percentage;

        // WSAD move
        if (Input.GetKey(KeyCode.W)) { this.transform.position += new Vector3(0, 0, corrected_cam_movement_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.S)) { this.transform.position += new Vector3(0, 0, -corrected_cam_movement_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.A)) { this.transform.position += new Vector3(-corrected_cam_movement_sensitivity * Time.deltaTime, 0, 0); }
        else if (Input.GetKey(KeyCode.D)) { this.transform.position += new Vector3(corrected_cam_movement_sensitivity * Time.deltaTime, 0, 0); }

        // Drag
        if (Input.GetMouseButtonDown(0))
        {
            dragOldPosition = this.transform.position;
            dragClickOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - dragClickOrigin;
            this.transform.position = dragOldPosition - new Vector3(pos.x, 0, pos.y) * corrected_cam_movement_sensitivity;
        }

        // Clamp viewport movement
        if(view_clamped)
        {
            float vertExtent = Camera.main.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;
            float clamped_x = Mathf.Clamp(this.transform.position.x, x_min + horzExtent, x_max - horzExtent);
            float clamped_z = Mathf.Clamp(this.transform.position.z, y_min + vertExtent, y_max - vertExtent);
            this.transform.position = new Vector3(clamped_x, this.transform.position.y, clamped_z);
        }
    }

}
