using AnarchyBros.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace AnarchyBros
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public RectTransform PanelGraph, PanelGameState, PanelMap, PanelTowerPlacement;
        public Text TowerPlacementTowersLeft;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            if (PanelTowerPlacement.gameObject.activeSelf)
            {
                TowerPlacementTowersLeft.text = "Towers left: " + (TowerManager.Instance.MaxTowerCount - TowerManager.Instance.ActiveTowers);
            }
        }

        public void OnGameStateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.Play:
                    PanelGraph.gameObject.SetActive(false);
                    PanelGameState.gameObject.SetActive(true);
                    PanelMap.gameObject.SetActive(false);
                    PanelTowerPlacement.gameObject.SetActive(false);
                    break;

                case GameStates.Pause:
                    PanelGraph.gameObject.SetActive(false);
                    PanelGameState.gameObject.SetActive(false);
                    PanelMap.gameObject.SetActive(false);
                    PanelTowerPlacement.gameObject.SetActive(false);
                    break;

                case GameStates.Stop:
                    PanelGraph.gameObject.SetActive(false);
                    PanelGameState.gameObject.SetActive(false);
                    PanelMap.gameObject.SetActive(false);
                    PanelTowerPlacement.gameObject.SetActive(false);
                    break;

                case GameStates.Edit:
                    PanelGraph.gameObject.SetActive(true);
                    PanelGameState.gameObject.SetActive(true);
                    PanelMap.gameObject.SetActive(true);
                    PanelTowerPlacement.gameObject.SetActive(false);
                    break;

                case GameStates.Place:
                    PanelGraph.gameObject.SetActive(true);
                    PanelGameState.gameObject.SetActive(true);
                    PanelMap.gameObject.SetActive(false);
                    PanelTowerPlacement.gameObject.SetActive(true);
                    break;

                default:
                    break;

            }
        }
    }
}