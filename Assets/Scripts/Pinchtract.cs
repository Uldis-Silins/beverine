using UnityEngine;
using UnityEngine.Events;

public class Pinchtract : MonoBehaviour
{
    public enum GameStateType { None, Start, Select, Puzzle, GameOver }

    public UnityEvent<GameObject> onStartHit;
    public UnityEvent<Microsystem> onMicrosystemSelect;
    public UnityEvent onMicrosystemDeselect;
    public Transform pointer;
    public float attractionSpeed = 2.0f; // Speed of attraction
    public LayerMask startHitLayers;
    public LayerMask selectHitLayers;
    public LayerMask puzzleHitLayers;

    private bool isAttracting = false;
    public LineRenderer pointerLine;
    private Vector3 m_prevDragPosition;
    public Microsystem CurrentHitMicrosystem { get; private set; }
    public Puzzle CurrentHitPuzzle { get; private set; }
    public float DragDelta { get; private set; }

    public GameStateType CurrentState { get; private set; }

    void Update()
    {
        switch (CurrentState)
        {
            case GameStateType.Start:
                Collider[] hits = Physics.OverlapSphere(pointer.position, 0.15f, startHitLayers);

                if(hits.Length > 0)
                {
                    onStartHit.Invoke(hits[0].gameObject);
                }
                break;
            case GameStateType.Select:
                RaycastHit selectHit;

                //pointerLine.enabled = true;

                pointerLine.SetPosition(0, pointer.position);
                pointerLine.SetPosition(1, pointer.position - pointer.right * 30f);
                if (Physics.Raycast(pointer.position, -pointer.right, out selectHit, 30f, selectHitLayers))
                {
                    pointerLine.SetPosition(1, selectHit.point);
                    CurrentHitMicrosystem = selectHit.collider.transform.parent.GetComponent<Microsystem>();
                    pointerLine.material.SetColor("_BaseColor", Color.red);
                }
                else
                {
                    onMicrosystemDeselect.Invoke();
                    CurrentHitMicrosystem = null;
                    pointerLine.material.SetColor("_BaseColor", Color.white);
                }
                break;
            case GameStateType.Puzzle:
                Collider[] puzzleHits = Physics.OverlapSphere(pointer.position, 0.05f, puzzleHitLayers);

                if (puzzleHits.Length > 0)
                {
                    if(CurrentHitPuzzle == null) 
                    {
                        m_prevDragPosition = pointer.position;

                        CurrentHitPuzzle = puzzleHits[0].transform.parent.GetComponent<Puzzle>();
                    }

                    if(CurrentHitPuzzle != null)
                    {
                        Vector3 drag = pointer.position - m_prevDragPosition;
                        DragDelta = drag.x + drag.z;
                        DragDelta *= 2500f;
                        m_prevDragPosition = pointer.position;
                        pointerLine.material.SetColor("_BaseColor", Color.blue);
                    }
                }
                else
                {
                    DragDelta = 0f;
                    CurrentHitPuzzle = null;
                    pointerLine.material.SetColor("_BaseColor", Color.white);
                }
                break;
        }
    }

    public void SetState(GameStateType type)
    {
        CurrentState = type;
    }
}
