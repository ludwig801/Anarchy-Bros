using UnityEngine;

namespace AnarchyBros
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public SpawnManager SpawnManager;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }
}