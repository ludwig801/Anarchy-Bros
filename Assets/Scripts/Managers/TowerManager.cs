using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class TowerManager : MonoBehaviour
    {
        public static TowerManager Instance { get; private set; }

        public Transform TowersObj, BulletsObj;
        public GameObject TowerPrefab;
        public int SelectedTower, MaxTowerCount, ActiveTowers;
        public List<Tower> Towers;

        GameManager _gameManager;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _gameManager = GameManager.Instance;

            SelectedTower = int.MinValue;

            Towers = new List<Tower>();
            ActiveTowers = 0;
        }

        void Update()
        {
        }

        void GetTowers()
        {
            for (int i = 0; i < TowersObj.childCount; i++)
            {
                Tower tower = TowersObj.GetChild(i).GetComponent<Tower>();
                Towers.Add(tower);
            }
        }

        void AssignSpot(Tower tower, Spot spot)
        {
            if (tower.Objective != null)
            {
                tower.Objective.Tower = null;
            }
            tower.Objective = spot;
            tower.Objective.Tower = tower;
        }

        void PlaceTower(Spot spot)
        {
            Tower tower;

            if (ActiveTowers < Towers.Count)
            {
                tower = GetTower();
                tower.Reborn();
                tower.gameObject.SetActive(true);
            }
            else
            {
                GameObject instance = Instantiate(TowerPrefab);
                instance.name = "Tower";
                instance.transform.parent = TowersObj;

                tower = instance.GetComponent<Tower>();
                tower.Reborn();
                tower.Bullets = BulletsObj;
                Towers.Add(tower);
            }

            AssignSpot(tower, spot);
            tower.transform.position = Tools2D.Convert(tower.transform.position, spot.transform.position);
            tower.MoveTo = spot;
            ActiveTowers++;
        }

        void RemoveTower(Tower tower)
        {
            if (tower.Objective != null)
            {
                tower.Objective.Tower = null;
                tower.Objective = null;
            }

            tower.gameObject.SetActive(false);
            ActiveTowers--;
        }

        void DestroyAllTowers()
        {
            for (int i = 0; i < Towers.Count; i++)
            {
                Destroy(Towers[i].gameObject);
            }

            Towers.Clear();
            ActiveTowers = 0;
        }

        void DestroyAllBullets()
        {
            for (int i = 0; i < BulletsObj.childCount; i++)
            {
                Destroy(BulletsObj.GetChild(i).gameObject);
            }

            Towers.Clear();
            ActiveTowers = 0;
        }

        int GetTowerIndex(Tower tower)
        {
            for (int i = 0; i < Towers.Count; i++)
            {
                if (Towers[i] == tower)
                {
                    return i;
                }
            }

            return int.MinValue;
        }

        Tower GetTower()
        {
            for (int i = 0; i < Towers.Count; i++)
            {
                if (!Towers[i].gameObject.activeSelf)
                {
                    return Towers[i];
                }
            }

            return null;
        }

        public Tower GetRandomTower()
        {
            int rand = Random.Range(0, ActiveTowers);
            for (int i = 0; i < Towers.Count; i++)
            {
                if (Towers[i].IsAlive())
                {
                    if (rand <= 0)
                    {
                        return Towers[i];
                    }
                    rand--;
                }
            }

            return null;
        }

        public void OnSpotClicked(Spot node)
        {
            switch (_gameManager.CurrentState)
            {
                case GameStates.Edit:
                    break;

                case GameStates.Place:
                    if (node.Type == SpotTypes.TowerSpot && ActiveTowers < MaxTowerCount)
                    {
                        PlaceTower(node);
                    }
                    break;

                case GameStates.Play:
                    if (node.Type == SpotTypes.TowerSpot)
                    {
                        if (SelectedTower >= 0 && SelectedTower < Towers.Count)
                        {
                            if (node.Occupied)
                            {
                                SelectedTower = GetTowerIndex(node.Tower);
                                _gameManager.Pause();
                            }
                            else
                            {
                                AssignSpot(Towers[SelectedTower], node);
                            }
                        }
                        else
                        {
                            SelectedTower = GetTowerIndex(node.Tower);
                        }
                    }
                    break;

                case GameStates.Pause:
                    if (node.Type == SpotTypes.TowerSpot)
                    {
                        if (SelectedTower >= 0 && SelectedTower < Towers.Count)
                        {
                            if (node.Occupied)
                            {
                                SelectedTower = GetTowerIndex(node.Tower);
                            }
                            else
                            {
                                AssignSpot(Towers[SelectedTower], node);
                            }
                        }
                        else
                        {
                            SelectedTower = GetTowerIndex(node.Tower);
                        }
                    }
                    break;
            }       
        }

        public void OnTowerClicked(PointerEventData eventData, Tower tower)
        {
            switch (_gameManager.CurrentState)
            {
                case GameStates.Edit:
                    break;

                case GameStates.Place:
                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        RemoveTower(tower);
                    }
                    break;

                case GameStates.Play:
                    SelectedTower = GetTowerIndex(tower);
                    _gameManager.Pause();
                    break;

                case GameStates.Pause:
                    _gameManager.Play();
                    break;
            }
        }

        public void OnTowerKill()
        {
            ActiveTowers--;
        }

        public void OnGameStateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.Edit:
                    DestroyAllTowers();
                    DestroyAllBullets();
                    break;

                case GameStates.Place:
                    DestroyAllTowers();
                    DestroyAllBullets();
                    break;

                case GameStates.Play:
                    break;
            }
        }
    }
}
