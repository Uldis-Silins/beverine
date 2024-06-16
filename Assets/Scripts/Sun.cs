using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public Microsystem microsystemPrefab;
    public float spawnMoveSpeed = 15f;
    public float microsystemSpawnDistance = 10f;

    public List<Microsystem> microsystems;

    public Transform sunPosition;

    private Vector3 m_startAnimationPosition;

    private const int MICROSYSTEM_COUNT = 8;
    private float m_moveAnimTimer;
    private float m_moveAnimTime = 2f;

    public bool InAnimation { get; private set; }

    private void Update()
    {
        if(InAnimation)
        {
            transform.position = Vector3.Lerp(m_startAnimationPosition, sunPosition.position, m_moveAnimTimer / m_moveAnimTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, sunPosition.rotation, 3f * Time.deltaTime);
            transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.775911f, m_moveAnimTimer / m_moveAnimTime);

            if(m_moveAnimTimer >= m_moveAnimTime)
            {
                InAnimation = false;
                SpawnMicrosystems();
            }

            m_moveAnimTimer += Time.deltaTime;
        }
    }

    public void SpawnMicrosystems()
    {
        for(int i = 0; i < microsystems.Count; i++)
        {
            microsystems[i].gameObject.SetActive(true);
            microsystems[i].Spawn(transform.position, microsystems[i].transform.position, 2f);
        }
    }

    public void StartSpawnAnimation()
    {
        m_startAnimationPosition = transform.position;
        InAnimation = true;
    }
}
