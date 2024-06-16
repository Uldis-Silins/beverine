using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class Microsystem : MonoBehaviour
{
    public UnityEvent onSelcted;
    public UnityEvent onDeselected;
    public UnityEvent onStartPuzzle;

    public ParticleSystem moveParticles;
    public AnimationCurve spawnAnimCurve;

    public Transform puzzleTransform;
    public SphereCollider selectionCollider;

    private Vector3 m_startPosition;
    private Vector3 m_targetPosition;
    private float m_spawnAnimTime;
    private float m_spawnAnimTimer;
    private float m_selectAnimationTimer;
    private float m_selectAnimationTime = 3f;
    private Vector3 m_selectTargetPosition;

    public bool InSpawnAnimation { get; private set; }
    public bool InSelectAnimation { get; private set; }
    public bool InDeselectAnimation { get; private set; }
    public bool IsHovered { get; private set; }
    public bool InPuzzlePosition { get; private set; }

    private void Update()
    {
        if (m_spawnAnimTimer > 0f)
        {
            float t = spawnAnimCurve.Evaluate(m_spawnAnimTimer / m_spawnAnimTime);
            t = 1f - t;
            transform.position = Vector3.Lerp(m_startPosition, m_targetPosition, t);

            m_spawnAnimTimer -= Time.deltaTime;

            if (m_spawnAnimTimer <= 0f)
            {
                transform.position = m_targetPosition;
                moveParticles.Stop();
                InSpawnAnimation = false;
            }
        }

        if (InSelectAnimation)
        {
            transform.position = Vector3.Lerp(transform.position, m_selectTargetPosition, 5f * Time.deltaTime);

            if (Vector3.Distance(transform.position, m_selectTargetPosition) < 0.25f)
            {
                InPuzzlePosition = true;
            }
        }

        if (InDeselectAnimation)
        {
            transform.position = Vector3.Lerp(transform.position, m_targetPosition, 10f * Time.deltaTime);

            if (Vector3.Distance(transform.position, m_targetPosition) < 0.25f)
            {
                InDeselectAnimation = false;
                onDeselected.Invoke();
            }
        }
    }

    public void Select(Vector3 moveTarget)
    {
        transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime);

        if (Vector3.Distance(transform.position, moveTarget) < 3f)
        {
            selectionCollider.enabled = false;
            m_selectTargetPosition = moveTarget;
            InSelectAnimation = true;
            onSelcted.Invoke();
        }
    }

    public void Deselect()
    {
        InDeselectAnimation = true;
    }

    public void Spawn(Vector3 startPos, Vector3 targetPos, float t)
    {
        m_startPosition = startPos;
        m_targetPosition = targetPos;
        m_spawnAnimTime = t;
        m_spawnAnimTimer = t;
        moveParticles.Play();
        InSpawnAnimation = true;
    }
}
