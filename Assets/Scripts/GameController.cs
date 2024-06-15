using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public enum GameStateType { None, Start, Select, Puzzle }

    private delegate void StateHandler();

    public GameStartButton startButton;
    public Sun sunPrefab;
    public LayerMask microsystemLayers;

    public Transform puzzlePosition;

    public Puzzle puzzlePrefab;

    [HideInInspector] public UnityEvent onGameStart;

    private StateHandler m_stateHandler;

    private Dictionary<GameStateType, StateHandler> m_states;

    public GameStateType CurrentGameState { get; private set; }
    public Sun CurrentSun { get; private set; }
    public Microsystem CurrentHoveredMicrosystem { get; private set; }
    public Puzzle CurrentPuzzle { get; private set; }

#if UNITY_EDITOR
    [SerializeField] private Camera m_mainCam;
#endif  // UNITY_EDITOR

    private void Awake()
    {
        m_states = new Dictionary<GameStateType, StateHandler>()
        {
            { GameStateType.None, EnterState_None },
            { GameStateType.Start, EnterState_Start },
            { GameStateType.Select, EnterState_Select },
            { GameStateType.Puzzle, EnterState_Puzzle }
        };
    }

    private void Start()
    {
        CurrentGameState = GameStateType.Start;
        m_stateHandler = EnterState_Start;
    }

    private void Update()
    {
        if (m_stateHandler != null)
        {
            m_stateHandler();
        }
    }

    private void EnterState_None()
    {
        m_stateHandler = State_None;
    }

    private void State_None()
    {
        if (CurrentGameState != GameStateType.None)
        {
            ExitState_None(m_states[CurrentGameState]);
        }
    }

    private void ExitState_None(StateHandler targetState)
    {
        m_stateHandler = targetState;
    }

    private void EnterState_Start()
    {
        m_stateHandler = State_Start;
    }

    private void State_Start()
    {
        if (CurrentGameState != GameStateType.Start)
        {
            ExitState_Start(m_states[CurrentGameState]);
            return;
        }

#if UNITY_EDITOR
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(m_mainCam.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue))
            {
                if (hit.collider.gameObject == startButton.gameObject)
                {
                    hit.collider.GetComponent<GameStartButton>().OnStartGameClick();
                    OnStartGame();
                    CurrentGameState = GameStateType.Select;
                }
            }
        }
#endif  // UNITY_EDITOR
    }

    private void ExitState_Start(StateHandler targetState)
    {
        m_stateHandler = targetState;
    }

    private void EnterState_Select()
    {
        m_stateHandler = State_Select;
    }

    private void State_Select()
    {
        if (CurrentGameState != GameStateType.Select)
        {
            ExitState_Select(m_states[CurrentGameState]);
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(m_mainCam.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, microsystemLayers))
        {
            Microsystem ms = hit.collider.transform.parent.GetComponent<Microsystem>();

            if (ms != null && !ms.InAnimation)
            {
                if (CurrentHoveredMicrosystem != null && CurrentHoveredMicrosystem != ms)
                {
                    CurrentHoveredMicrosystem.SetHovered(false);
                    CurrentHoveredMicrosystem = null;
                }

                CurrentHoveredMicrosystem = ms;
                CurrentHoveredMicrosystem.SetHovered(true);
#if UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentHoveredMicrosystem.SetHovered(false);
                    CurrentHoveredMicrosystem = null;
                    CurrentGameState = GameStateType.Puzzle;
                }
#endif  // UNITY_EDITOR
            }
        }
        else
        {
            if (CurrentHoveredMicrosystem != null)
            {
                CurrentHoveredMicrosystem.SetHovered(false);
                CurrentHoveredMicrosystem = null;
            }
        }
    }

    private void ExitState_Select(StateHandler targetState)
    {
        m_stateHandler = targetState;
    }

    private void EnterState_Puzzle()
    {
        m_stateHandler = State_Puzzle;
    }

    private void State_Puzzle()
    {
        if (CurrentGameState != GameStateType.Puzzle)
        {
            ExitState_Puzzle(m_states[CurrentGameState]);
            return;
        }
    }

    private void ExitState_Puzzle(StateHandler targetState)
    {
        m_stateHandler = targetState;
    }

    /// Start game session on button push
    public void OnStartGame()
    {
        startButton.gameObject.SetActive(false);
        CurrentSun = Instantiate(sunPrefab, startButton.transform.position, startButton.transform.rotation);
        CurrentSun.StartSpawnAnimation(m_mainCam.transform.forward);
        onGameStart.Invoke();
        Debug.Log("Game Started");
    }
}