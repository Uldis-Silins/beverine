using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotateChilds : MonoBehaviour
{
    [ExecuteInEditMode]
    public void RandomRotate()
    {
        transform.GetChild(0).localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
        transform.GetChild(1).localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
    }
}
