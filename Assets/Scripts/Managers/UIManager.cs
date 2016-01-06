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

    }
}