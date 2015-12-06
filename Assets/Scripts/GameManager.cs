using AnarchyBros.Strings;
using UnityEngine;

namespace AnarchyBros
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public string State { get; private set; }

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
            State = GameStates.Play;
            PlayerManager.Instance.SetPlayersActive(true);
        }

        public void Stop()
        {
            State = GameStates.Stop;
        }

        public void Edit()
        {
            State = GameStates.Edit;
            PlayerManager.Instance.SetPlayersActive(false);
        }

        public bool IsPlay
        {
            get
            {
                return State == GameStates.Play;
            }
        }

        public bool IsStop
        {
            get
            {
                return State == GameStates.Stop;
            }
        }

        public bool IsEdit
        {
            get
            {
                return State == GameStates.Edit;
            }
        }
    }
}