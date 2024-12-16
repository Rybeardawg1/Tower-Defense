using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float rot_sensitivity = 2f;     // Mouse sensitivity for rotation (not needed here for fixed top-down)
    public float move_sensitivity = 0.1f; // Sensitivity for mouse movement
    public float zoomSpeed = 2f;          // Speed of zooming with the scroll wheel
    //public float minZoom = 1f;            // Minimum zoom height
    //public float maxZoom = 40f;           // Maximum zoom height
    public float moveSpeed = 5f;          // Movement speed for WASD keys

    private Vector3 init_pos;             // Initial position of the camera
    private float angle = 75f;            // Top-down angle

    void Start()
    {
        // Save the initial position of the camera
        init_pos = transform.position;

        // Set the initial rotation for a top-down view
        transform.rotation = Quaternion.Euler(angle, 0, 0);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Zoom with the scroll wheel (changes Y position)
        //if (scroll != 0)
        //{
        //    float newY = Mathf.Clamp(transform.position.y - scroll * zoomSpeed, minZoom, maxZoom);
        //    transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        //}

        // Move the camera with the mouse (X-Z plane only)
        if (Input.GetMouseButton(1)) // Only move when the right mouse button is pressed
        {
            Vector3 mouseMovement = new Vector3(mouseX, 0, mouseY) * move_sensitivity;
            transform.position += mouseMovement;
        }

        // Move the camera with WASD keys (X-Z plane only)
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            moveDirection += Vector3.right;

        // Normalize and apply movement (X-Z plane)
        moveDirection = moveDirection.normalized * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.z);
    }
}
