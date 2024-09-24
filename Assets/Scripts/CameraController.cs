using UnityEngine;

public class CameraControllerGlobal : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of camera movement
    public Vector3 minBounds; // Minimum bounds for camera movement
    public Vector3 maxBounds; // Maximum bounds for camera movement

    void Update()
    {
        Vector3 newPosition = transform.position;

        // Get input for movement (WASD or arrow keys)
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveZ = moveSpeed * Time.deltaTime; // Move forward on the global Z axis
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveZ = -moveSpeed * Time.deltaTime; // Move backward on the global Z axis
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -moveSpeed * Time.deltaTime; // Move left on the global X axis
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveX = moveSpeed * Time.deltaTime; // Move right on the global X axis
        }

        // Apply the movement on the global axis (X and Z) regardless of camera's rotation
        newPosition += new Vector3(moveX, 0, moveZ);

        // Clamp the camera movement within the defined bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.z = Mathf.Clamp(newPosition.z, minBounds.z, maxBounds.z);

        // Apply the new position to the camera
        transform.position = newPosition;
    }
}
