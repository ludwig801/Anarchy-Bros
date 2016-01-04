using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameStates CurrentState { get { return _currentState; } }

        GameStates _currentState;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            ChangeState(GameStates.Edit);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void ChangeState(string newState)
        {
            GameStates state = GameStates.Edit;
            bool found = false;
            if (GameStates.Edit.ToString() == newState)
            {
                found = true;
                state = GameStates.Edit;
            }
            else if (GameStates.Place.ToString() == newState)
            {
                found = true;
                state = GameStates.Place;
            }
            else if (GameStates.Play.ToString() == newState)
            {
                found = true;
                state = GameStates.Play;
            }

            if (found)
            {
                ChangeState(state);
            }
        }

        public void ChangeState(GameStates newState)
        {
            _currentState = newState;
            MapManager.Instance.OnGameStateChanged(_currentState);
            TowerManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            if (newState == GameStates.Pause)
            {
                Time.timeScale = 1f;
            }
        }

        public bool IsCurrentState(GameStates comp)
        {
            return comp == _currentState;
        }
    }
}