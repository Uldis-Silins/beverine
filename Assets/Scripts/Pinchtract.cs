using UnityEngine;
using UnityEngine.UIElements;

public class PinchObjectAttractor : MonoBehaviour
{
    public OVRHand hand; // Assign the OVRHand component for your hand in the inspector
    public Transform objectToAttract; // Assign the transform of the object to attract
    public Transform pointer;
    public float attractionSpeed = 2.0f; // Speed of attraction
    private bool isAttracting = false;
    public LineRenderer pointerLine;
    private Vector3 prevPos;
    private Transform objectMan;


    void Update()
    {
        // Check if the index finger is pinching
        RaycastHit hits;
        
        pointerLine.SetPosition(0, pointer.position);
        pointerLine.SetPosition(1, pointer.position - pointer.right * 0.15F);
        if (Physics.Raycast(pointer.position, -pointer.right, out hits, 0.15F)) {
            if (objectMan == null){
                prevPos = pointer.position;
                objectMan = hits.collider.transform;
            }
            Vector3 delta = pointer.position - prevPos;
            delta *= 100F;
            hits.collider.transform.Rotate(Vector3.up * (delta.x + delta.y) );
            prevPos = pointer.position;
        }
        else {
            objectMan = null;
        }

        bool isPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        if (isPinching)
        {
            StartAttraction();
        }
        else
        {
            StopAttraction();
        }

        // If the object is being attracted, move it towards the hand
        if (isAttracting)
        {
            objectToAttract.position = Vector3.MoveTowards(objectToAttract.position, hand.transform.position, attractionSpeed * Time.deltaTime);
        }
    }

    private void StartAttraction()
    {
        isAttracting = true;
    }

    private void StopAttraction()
    {
        isAttracting = false;
    }
}
