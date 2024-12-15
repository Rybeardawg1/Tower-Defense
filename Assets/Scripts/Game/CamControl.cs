using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float rot_sensitivity = 2f;  // Mouse sensitivity for rotation
    public float maxXAngle = 10f;       // Max vertical angle for rotation
    public float minXAngle = -10f;      // Min vertical angle for rotation
    public float maxYAngle = 10f;       // Max horizontal angle for rotation
    public float minYAngle = -10f;      // Min horizontal angle for rotation

    public float xy_sensitivity = 0.1f; // Sensitivity for mouse-based movement
    public float max_move = 2.0f;       // Maximum mouse-based move range

    public float zoom_speed = 2.0f;     // Scroll wheel zoom speed
    public float min_zoom = 2.0f;       // Minimum zoom distance
    public float max_zoom = 10.0f;      // Maximum zoom distance

    public float move_speed = 5.0f;     // Speed for WSAD movement

    private float rotationX = 0f;       // Tracks vertical rotation
    private float rotationY = 0f;       // Tracks horizontal rotation
    private Vector3 init_pos;           // Initial position of the camera
    private float current_zoom = 5.0f;  // Current zoom level

    void Start()
    {
        init_pos = transform.position;      // Save initial position
        rotationX = transform.eulerAngles.x;
        rotationY = transform.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Handle right mouse button for rotation and movement
        if (Input.GetMouseButton(1))
        {
            // Adjust rotation
            rotationY += mouseX * rot_sensitivity;
            rotationX -= mouseY * rot_sensitivity;

            // Clamp rotation
            rotationX = Mathf.Clamp(rotationX, minXAngle, maxXAngle);
            rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

            // Apply rotation
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
        else
        {
            // Handle mouse-based movement
            Vector3 mouseMovement = new Vector3(mouseX, mouseY, 0) * xy_sensitivity;
            Vector3 newPosition = transform.position + mouseMovement;

            // Clamp position to stay within max_move range
            newPosition.x = Mathf.Clamp(newPosition.x, init_pos.x - max_move, init_pos.x + max_move);
            newPosition.y = Mathf.Clamp(newPosition.y, init_pos.y - max_move, init_pos.y + max_move);

            // Apply new position
            transform.position = newPosition;
        }

        // Handle scroll wheel zoom
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        current_zoom -= scrollInput * zoom_speed;
        current_zoom = Mathf.Clamp(current_zoom, min_zoom, max_zoom);

        // Apply zoom (move the camera along its forward vector)
        transform.position = init_pos + transform.forward * -current_zoom;

        // Handle WSAD movement
        Vector3 movement = new Vector3(
            Input.GetAxis("Horizontal"),  // A/D for left/right
            0,                            // No vertical input here
            Input.GetAxis("Vertical")     // W/S for forward/backward
        );

        // Move the camera relative to its orientation
        transform.Translate(movement * move_speed * Time.deltaTime, Space.Self);
    }
}
