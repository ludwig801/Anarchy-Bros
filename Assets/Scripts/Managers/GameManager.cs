using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public Texture2D HandCursorTexture;
        public GameStates CurrentState { get { return _currentState; } }

        GameStates _currentState;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Edit();
            Cursor.SetCursor(HandCursorTexture, new Vector2(HandCursorTexture.width * 0.4f, HandCursorTexture.height * 0f), CursorMode.Auto);
        }

        public void Play()
        {
            _currentState = GameStates.Play;
            MapManager.Instance.OnGameStateChanged(_currentState);
            TowerManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
            Time.timeScale = 1f;
        }

        public void Pause()
        {
            _currentState = GameStates.Pause;
            MapManager.Instance.OnGameStateChanged(_currentState);
            TowerManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
            Time.timeScale = 0f;
        }

        public void Stop()
        {
            _currentState = GameStates.Stop;
            MapManager.Instance.OnGameStateChanged(_currentState);
            TowerManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
        }

        public void Edit()
        {
            _currentState = GameStates.Edit;
            MapManager.Instance.OnGameStateChanged(_currentState);
            TowerManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
        }

        public void Place()
        {
            _currentState = GameStates.Place;
            MapManager.Instance.OnGameStateChanged(_currentState);
            TowerManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
        }

        public bool IsCurrentState(GameStates comp)
        {
            return comp == _currentState;
        }
    }
}