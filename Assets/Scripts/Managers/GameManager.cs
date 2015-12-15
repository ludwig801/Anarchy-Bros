using AnarchyBros.Enums;
using UnityEngine;

namespace AnarchyBros
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        GameStates _currentState;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Edit();
        }

        public void Play()
        {
            _currentState = GameStates.Play;
            MapManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            TowerManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
        }

        public void Stop()
        {
            _currentState = GameStates.Stop;
            TowerManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
        }

        public void Edit()
        {
            _currentState = GameStates.Edit;
            TowerManager.Instance.OnGameStateChanged(_currentState);
            EnemyManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
        }

        public void Place()
        {
            _currentState = GameStates.Place;
            TowerManager.Instance.OnGameStateChanged(_currentState);
            UIManager.Instance.OnGameStateChanged(_currentState);
        }

        public bool IsCurrentState(GameStates state)
        {
            return _currentState == state;
        }
    }
}