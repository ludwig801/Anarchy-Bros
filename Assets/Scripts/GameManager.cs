using UnityEngine;

namespace AnarchyBros
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public State CurrentState { get; private set; }

        public enum State
        {
            Stopped = 0,
            Playing = 1,
            Editing = 2
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Stop();
        }

        void Play()
        {
            CurrentState = State.Playing;
        }

        void Stop()
        {
            CurrentState = State.Stopped;
        }

        void Edit()
        {
            CurrentState = State.Editing;
        }
    }
}