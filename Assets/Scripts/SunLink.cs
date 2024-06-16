using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLink : MonoBehaviour
{
    public LineRenderer lr;
    public ParticleSystem particles;

    private Vector3 m_fromPosition;
    private Vector3 m_toPosition;
    private float m_moveAnimTimer;
    private float m_moveAnimTime = 1f;

    public bool InMoveAnimation { get; private set; }

    private void Update()
    {
        if (InMoveAnimation)
        {
            Vector3 pos = Vector3.Lerp(m_fromPosition, m_toPosition, m_moveAnimTimer / m_moveAnimTime);

            particles.transform.position = pos;
            lr.SetPosition(0, m_fromPosition);
            lr.SetPosition(1, pos);

            m_moveAnimTimer += Time.deltaTime;

            if (m_moveAnimTimer >= m_moveAnimTime)
            {
                particles.Stop();
                InMoveAnimation = true;
            }
        }
    }

    public void StartMoveAnimation(Vector3 from, Vector3 to)
    {
        m_moveAnimTimer = 0f;
        InMoveAnimation = true;
        m_fromPosition = from;
        m_toPosition = to;
    }
}
