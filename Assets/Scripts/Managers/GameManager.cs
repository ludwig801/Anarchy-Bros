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
            GraphManager.Instance.ReEvaluate();
            EnemyManager.Instance.ReEvaluate();
            TowerManager.Instance.ReEvaluate();
        }

        public void Stop()
        {
            _currentState = GameStates.Stop;
            TowerManager.Instance.ReEvaluate();
        }

        public void Edit()
        {
            _currentState = GameStates.Edit;
            TowerManager.Instance.ReEvaluate();
        }

        public void Place()
        {
            _currentState = GameStates.Place;
            TowerManager.Instance.ReEvaluate();
        }

        public bool IsCurrentState(GameStates state)
        {
            return _currentState == state;
        }

        public GameStates CurrentState
        {
            get
            {
                return _currentState;
            }
        }
    }
}