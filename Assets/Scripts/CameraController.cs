using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int targetFrameRate = 20;
    public float camera_sensitivity = .01f;
    public float zoom_sensitivity = 80f;
    public float drag_sensitivity = 2;


    private Vector3 dragOrigin;
    private bool dragActive = false;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        // WSAD move
        if (Input.GetKey(KeyCode.W)) { this.transform.position += new Vector3(0, 0, camera_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.S)) { this.transform.position += new Vector3(0, 0, -camera_sensitivity * Time.deltaTime); }
        else if (Input.GetKey(KeyCode.A)) { this.transform.position += new Vector3(-camera_sensitivity * Time.deltaTime, 0, 0); }
        else if (Input.GetKey(KeyCode.D)) { this.transform.position += new Vector3(camera_sensitivity * Time.deltaTime, 0, 0); }

/*        // Mouse scroll zoom
        if (Input.mouseScrollDelta.y > 0)
        {
            this.transform.position += new Vector3(0, -zoom_sensitivity * Time.deltaTime, 0);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            this.transform.position += new Vector3(0, zoom_sensitivity * Time.deltaTime, 0);
        }
        if (transform.position.y > 40) transform.position = new Vector3(transform.position.x, 40, transform.position.z);
        else if (transform.position.y < 4) transform.position = new Vector3(transform.position.x, 4, transform.position.z);*/

/*        // Drag
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
        Vector3 move = new Vector3(-pos.x * dragSpeed * Time.deltaTime, 0, -pos.y * dragSpeed * Time.deltaTime);
        transform.Translate(move, Space.World);*/
    }
}
