using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Puzzle : MonoBehaviour
{
    public UnityEvent onSolved;

    public enum PuzzleStateType { None, Solve, Done }

    public Transform innerRing;
    public Transform outerRing;
    public AnimationCurve moveAnimationCurve;

    public int solvedRotation = 45;

    private Vector3 m_startPosition;
    private Vector3 m_targetPosition;
    private float m_moveAnimTime;
    private float m_moveAnimTimer;

    private Vector3 m_prevMousePosition;
    private bool m_inDrag;
    private bool m_inDoneRotation;
    private float m_fromDoneRotation;
    private float m_doneTimer = 3f;

    public PuzzleStateType CurrentState { get; private set; }
    public bool InAnimation { get; private set; }

    private void Start()
    {
        CurrentState = PuzzleStateType.Solve;
    }

    private void Update()
    {
        if (InAnimation)
        {
            float t = moveAnimationCurve.Evaluate(m_moveAnimTimer / m_moveAnimTime);
            transform.position = Vector3.Lerp(m_startPosition, m_targetPosition, t);

            if (m_moveAnimTimer >= m_moveAnimTime)
            {
                InAnimation = false;
                CurrentState = PuzzleStateType.Solve;
            }

            m_moveAnimTimer += Time.deltaTime;
        }
        else
        {
#if UNITY_EDITOR
            if (CurrentState == PuzzleStateType.Solve)
            {
                if (m_inDoneRotation)
                {
                    if (outerRing.eulerAngles.z - solvedRotation > 0.5f)
                    {
                        float rotation = Mathf.Lerp(m_fromDoneRotation, solvedRotation, Time.deltaTime);
                        outerRing.Rotate(Vector3.forward * rotation);
                        innerRing.Rotate(Vector3.forward * -rotation);
                    }

                    m_doneTimer -= Time.deltaTime;

                    if (m_doneTimer < 0f)
                    {
                        m_inDoneRotation = false;
                        CurrentState = PuzzleStateType.Done;
                        onSolved.Invoke();
                    }
                }
                else
                {
                    if (!m_inDrag && Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, 1 << gameObject.layer))
                        {
                            m_inDrag = true;
                            m_prevMousePosition = Input.mousePosition;
                        }
                    }

                    if (m_inDrag && Input.GetMouseButton(0))
                    {
                        Vector2 delta = Input.mousePosition - m_prevMousePosition;

                        float rotation = delta.x + delta.y;
                        outerRing.Rotate(Vector3.forward * rotation);
                        innerRing.Rotate(Vector3.forward * -rotation);

                        m_prevMousePosition = Input.mousePosition;
                    }

                    if (m_inDrag && Input.GetMouseButtonUp(0))
                    {
                        if (Mathf.Abs(outerRing.eulerAngles.z - solvedRotation) < 5f)
                        {
                            m_inDoneRotation = true;
                            m_fromDoneRotation = outerRing.eulerAngles.x;
                        }

                        m_inDrag = false;
                    }
                }
            }
#endif
        }
    }

    public void Rotate(float drag)
    {
        if (CurrentState == PuzzleStateType.Solve)
        {
            outerRing.Rotate(Vector3.up * drag * Time.deltaTime);
            innerRing.Rotate(Vector3.up * -drag * Time.deltaTime);
        }

        if (Mathf.Abs(outerRing.eulerAngles.y) < 2f)
        {
            CurrentState = PuzzleStateType.Done;
            onSolved.Invoke();
        }
    }

    public void StartMoveAnimation(Vector3 pos, float t)
    {
        m_startPosition = transform.position;
        m_targetPosition = pos;
        m_moveAnimTime = t;
        m_moveAnimTimer = 0f;
        InAnimation = true;
    }
}
