using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public enum GameStateType { None, PressStart, Running }
    public GameStartButton startButton;

    [HideInInspector] public UnityEvent onGameStart;

    public GameStateType CurrentGameState { get; private set; }

#if UNITY_EDITOR
    [SerializeField] private Camera m_mainCam;
#endif  // UNITY_EDITOR

    private void Start()
    {
        CurrentGameState = GameStateType.PressStart;
    }

    private void Update()
    {
#if UNITY_EDITOR
        RaycastHit hit;
        if(Input.GetMouseButtonDown(0) && CurrentGameState == GameStateType.PressStart && Physics.Raycast(m_mainCam.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue))
        {
            if(hit.collider.gameObject == startButton.gameObject)
            {
                hit.collider.GetComponent<GameStartButton>().OnStartGameClick();
                StartGame();
                CurrentGameState = GameStateType.Running;
            }
        }
#endif  // UNITY_EDITOR
    }

    /// Start game session on button push
    public void StartGame()
    {
        onGameStart.Invoke();
        Debug.Log("Game Started");
    }
}
