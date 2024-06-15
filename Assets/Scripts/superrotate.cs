using UnityEngine;

public class SuperRotatePart : MonoBehaviour
{
    public float rotationSpeed = -50.0f;

    void Update()
    {
        // Rotate the object around its local Y axis at the specified speed
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}