using UnityEngine;

public class RotatePart : MonoBehaviour
{
    public float rotationSpeed = 100.0f;

    void Update()
    {
        // Rotate the object around its local Y axis at the specified speed
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}