using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleArrangement : MonoBehaviour
{
    public GameObject circlePrefab; // The prefab for the circles
    public float circleRadius = 5f; // Radius of the circle around which the 8 circles will be placed
    public int numberOfCircles = 8; // Number of circles to place

    void Start()
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            // Calculate the angle in radians
            float angle = i * Mathf.PI * 2f / numberOfCircles;
            
            // Determine the position of each circle
            float x = Mathf.Cos(angle) * circleRadius;
            float z = Mathf.Sin(angle) * circleRadius;
            
            // Instantiate the circle prefab at the calculated position
            Vector3 position = new Vector3(x, 0, z); // Assuming y = 0 for simplicity
            Instantiate(circlePrefab, position, Quaternion.identity);
        }
    }
}
