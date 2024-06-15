using UnityEngine;

public class PinchRotate : MonoBehaviour
{
    public float rotationSpeed; // Make this public to set different speeds for each object
    private bool isRotating = false;

    void Update()
    {
        // Check if the user is pinching with either hand
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5f || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f)
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
        }

        // Rotate the object if isRotating is true
        if (isRotating)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
