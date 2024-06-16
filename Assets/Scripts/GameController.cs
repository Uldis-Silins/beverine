using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public enum GameStateType { None, Start, Select, Puzzle, GameOver }

    private delegate void StateHandler();

    public Pinchtract input;
    public GameStartButton startButton;
    public Sun sun;
    public GameObject panoramaObject;
    public Renderer panoramaRenderer;
    public LayerMask microsystemLayers;

    public Transform puzzlePosition;

    public Puzzle puzzlePrefab;
    public SunLink linkPrefab;

    [HideInInspector] public UnityEvent onGameStart;

    private StateHandler m_stateHandler;

    private Microsystem m_currentPuzzleMicrosystem;

    private Dictionary<GameStateType, StateHandler> m_states;
    private List<Microsystem> m_solvedMicrosystems;

    public GameStateType CurrentGameState { get; private set; }
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
            { GameStateType.Puzzle, EnterState_Puzzle },
            { GameStateType.GameOver, EnterState_GameOver }
        };

        m_solvedMicrosystems = new List<Microsystem>();
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
        input.onStartHit.AddListener(OnStartGameClick);
        input.SetState(Pinchtract.GameStateType.Start);
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
                    OnStartGameClick(hit.collider.gameObject);
                }
            }
        }
#endif  // UNITY_EDITOR
    }

    private void ExitState_Start(StateHandler targetState)
    {
        input.onStartHit.RemoveListener(OnStartGameClick);
        m_stateHandler = targetState;
    }

    private void EnterState_Select()
    {
        input.SetState(Pinchtract.GameStateType.Select);
        input.onMicrosystemSelect.AddListener(OnMicrosystemSelect);
        input.onMicrosystemDeselect.AddListener(OnMicrosystemDeselect);
        m_stateHandler = State_Select;
    }

    private void State_Select()
    {
        if (CurrentGameState != GameStateType.Select)
        {
            ExitState_Select(m_states[CurrentGameState]);
            return;
        }

        if(input.CurrentHitMicrosystem != null && !m_solvedMicrosystems.Contains(input.CurrentHitMicrosystem))
        {
            CurrentHoveredMicrosystem = input.CurrentHitMicrosystem;
            CurrentHoveredMicrosystem.Select(puzzlePosition.position);
            CurrentHoveredMicrosystem.onSelcted.AddListener(OnMoveToPuzzle);
        }
        else
        {
            if(CurrentHoveredMicrosystem != null)
            {
                CurrentHoveredMicrosystem.Deselect();
                CurrentHoveredMicrosystem.onSelcted.RemoveListener(OnMoveToPuzzle);
                CurrentPuzzle = CurrentHoveredMicrosystem.GetComponent<Puzzle>();
                m_currentPuzzleMicrosystem = CurrentHoveredMicrosystem;
                CurrentHoveredMicrosystem = null;
            }
        }
    }

    private void ExitState_Select(StateHandler targetState)
    {
        input.onMicrosystemSelect.RemoveListener(OnMicrosystemSelect);
        input.onMicrosystemDeselect.RemoveListener(OnMicrosystemDeselect);
        m_stateHandler = targetState;
    }

    private void EnterState_Puzzle()
    {
        input.SetState(Pinchtract.GameStateType.Puzzle);
        if(CurrentPuzzle != null)
        {
            CurrentPuzzle.onSolved.AddListener(OnPuzzleSolved);
        }

        m_stateHandler = State_Puzzle;
    }

    private void State_Puzzle()
    {
        if(input.CurrentHitPuzzle != null)
        {
            CurrentPuzzle = input.CurrentHitPuzzle;
            CurrentPuzzle.Rotate(input.DragDelta);
        }

        if (CurrentGameState != GameStateType.Puzzle)
        {
            ExitState_Puzzle(m_states[CurrentGameState]);
            return;
        }
    }

    private void ExitState_Puzzle(StateHandler targetState)
    {
        CurrentPuzzle.onSolved.RemoveListener(OnPuzzleSolved);
        m_stateHandler = targetState;
    }

    private void EnterState_GameOver()
    {
        panoramaObject.SetActive(true);
        m_stateHandler = State_GameOver;
    }

    private void State_GameOver()
    {
        if(CurrentGameState != GameStateType.GameOver)
        {
            ExitState_GameOver(m_states[CurrentGameState]);
        }

        panoramaRenderer.material.SetColor("_BaseColor", Color.Lerp(Color.white, Color.clear, Time.deltaTime * 0.25f));
    }

    private void ExitState_GameOver(StateHandler targetState)
    {
        m_stateHandler = targetState;
    }

    /// Start game session on button push
    public void OnStartGame()
    {
        startButton.gameObject.SetActive(false);
        sun.StartSpawnAnimation();
        onGameStart.Invoke();
        Debug.Log("Game Started");
    }

    public void OnPuzzleSolved()
    {
        CurrentPuzzle.GetComponent<Microsystem>().Deselect();
        CurrentGameState = GameStateType.Select;
        m_solvedMicrosystems.Add(m_currentPuzzleMicrosystem);

        if(m_solvedMicrosystems.Count >= 8)
        {
            CurrentGameState = GameStateType.GameOver;
        }

        SunLink link = Instantiate(linkPrefab, m_currentPuzzleMicrosystem.transform.position, Quaternion.identity);
        link.StartMoveAnimation(m_currentPuzzleMicrosystem.transform.position, sun.transform.position);

        m_currentPuzzleMicrosystem = null;
    }

    private void OnStartGameClick(GameObject hitObject)
    {
        startButton.transform.parent = null;
        puzzlePosition.transform.parent = null;
        sun.gameObject.SetActive(true);
        sun.transform.position = startButton.transform.position;
        sun.transform.rotation = startButton.transform.rotation;
        sun.transform.localScale = startButton.transform.localScale;
        hitObject.GetComponent<GameStartButton>().OnStartGameClick();
        OnStartGame();
        CurrentGameState = GameStateType.Select;
    }

    private void OnMicrosystemSelect(Microsystem hit)
    {
        if(CurrentHoveredMicrosystem != null && CurrentHoveredMicrosystem != hit)
        {
            CurrentHoveredMicrosystem.Deselect();
        }

        CurrentHoveredMicrosystem = hit;
        CurrentHoveredMicrosystem.Select(puzzlePosition.position);
    }

    private void OnMicrosystemDeselect()
    {
    }

    private void OnMoveToPuzzle()
    {
        CurrentGameState = GameStateType.Puzzle;
    }
}
