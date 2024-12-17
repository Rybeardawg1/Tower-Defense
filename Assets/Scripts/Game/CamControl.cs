using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float rot_sensitivity = 2f;   
    public float move_sensitivity = 0.1f; 
    private Vector3 init_pos;            
    public float zoomSpeed = 2f;          
    public float moveSpeed = 5f;
    private float angle = 75f;            

    void Start()
    {
        init_pos = transform.position;
        // Set the initial rotation
        transform.rotation = Quaternion.Euler(angle, 0, 0);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Zoom with the scroll wheel
        //if (scroll != 0)
        //{
        //    float newY = Mathf.Clamp(transform.position.y - scroll * zoomSpeed, minZoom, maxZoom);
        //    transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        //}

        // Cam movement
        if (Input.GetMouseButton(1)) // When the right mouse button is pressed
        {
            Vector3 mouseMovement = new Vector3(mouseX, 0, mouseY) * move_sensitivity;
            transform.position += mouseMovement;
        }

        // Move the camera with WASD keys
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            moveDirection += Vector3.right;

        // Normalize and apply movement
        moveDirection = moveDirection.normalized * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.z);
    }
}
