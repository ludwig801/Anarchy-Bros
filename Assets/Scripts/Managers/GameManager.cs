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
            NodeManager.Instance.ReEvaluate();
            SpawnManager.Instance.ReEvaluate();
            PawnManager.Instance.ReEvaluate();
        }

        public void Stop()
        {
            _currentState = GameStates.Stop;
            PawnManager.Instance.ReEvaluate();
        }

        public void Edit()
        {
            _currentState = GameStates.Edit;
            PawnManager.Instance.ReEvaluate();
        }

        public void Place()
        {
            _currentState = GameStates.Place;
            PawnManager.Instance.ReEvaluate();
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