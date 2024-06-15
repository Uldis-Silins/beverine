using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Microsystem : MonoBehaviour
{
    public GameObject selectable;
    public ParticleSystem moveParticles;
    public AnimationCurve spawnAnimCurve;
    public AnimationCurve hoverScaleAnimationCurve;
    [SerializeField] private Renderer m_selectableRenderer;

    private Vector3 m_startPosition;
    private Vector3 m_targetPosition;
    private float m_spawnAnimTime;
    private float m_spawnAnimTimer;
    private float m_hoverScaleTimer;
    private Vector3 m_startScale;
    private readonly float m_hoverScaleTime = 0.25f;

    public bool InAnimation { get; private set; }
    public bool IsHovered { get; private set; }

    private void Start()
    {
        m_startScale = selectable.transform.localScale;
    }

    private void Update()
    {
        if(m_spawnAnimTimer > 0f)
        {
            float t = spawnAnimCurve.Evaluate(m_spawnAnimTimer / m_spawnAnimTime);
            t = 1f - t;
            transform.position = Vector3.Lerp(m_startPosition, m_targetPosition, t);

            m_spawnAnimTimer -= Time.deltaTime;

            if(m_spawnAnimTimer <= 0f)
            {
                transform.position = m_targetPosition;
                moveParticles.Stop();
                InAnimation = false;
            }
        }

        if(m_hoverScaleTimer > 0f)
        {
            float t = m_hoverScaleTimer / m_hoverScaleTime;
            if(IsHovered) t = 1f - t;
            Vector3 hoveredScale = m_startScale + Vector3.one * 0.25f;
            selectable.transform.localScale = Vector3.Lerp(m_startScale, hoveredScale, hoverScaleAnimationCurve.Evaluate(t));
            m_hoverScaleTimer -= Time.deltaTime;
        }
    }

    public void Spawn(Vector3 startPos, Vector3 targetPos, float t)
    {
        m_startPosition = startPos;
        m_targetPosition = targetPos;
        m_spawnAnimTime = t;
        m_spawnAnimTimer = t;
        moveParticles.Play();
        InAnimation = true;
    }

    public void SetHovered(bool isHovered)
    {
        if(IsHovered == isHovered) return;

        m_selectableRenderer.material.SetColor("_BaseColor", isHovered ? Color.green : Color.white);
        IsHovered = isHovered;
        m_hoverScaleTimer = m_hoverScaleTime;
    }
}
