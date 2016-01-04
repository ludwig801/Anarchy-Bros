using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class TowerManager : MonoBehaviour
    {
        public static TowerManager Instance { get; private set; }

        public Transform BulletCan;
        public GameObject TowerPrefab;
        public Tower SelectedTower;
        public int MaxTowerCount, ActiveTowers;
        public List<Tower> Towers;

        GameManager _gameManager;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _gameManager = GameManager.Instance;

            Towers = new List<Tower>();
            ActiveTowers = 0;
        }

        void Update()
        {
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
                instance.transform.parent = transform;

                tower = instance.GetComponent<Tower>();
                tower.Reborn();
                Towers.Add(tower);
                tower.BulletCan = BulletCan;
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
            for (int i = 0; i < BulletCan.childCount; i++)
            {
                Destroy(BulletCan.GetChild(i).gameObject);
            }
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

        public void OnSpotClicked(Spot spot)
        {
            switch (_gameManager.CurrentState)
            {
                case GameStates.Edit:
                    break;

                case GameStates.Place:
                    if (spot.Type == SpotTypes.TowerSpot && ActiveTowers < MaxTowerCount)
                    {
                        PlaceTower(spot);
                    }
                    break;

                case GameStates.Play:
                    if (spot.Type == SpotTypes.TowerSpot)
                    {
                        if (SelectedTower != null)
                        {
                            if (spot.Occupied)
                            {
                                SelectedTower = spot.Tower;
                                _gameManager.ChangeState(GameStates.Pause);
                            }
                            else
                            {
                                AssignSpot(SelectedTower, spot);
                            }
                        }
                        else
                        {
                            SelectedTower = spot.Tower;
                        }
                    }
                    break;

                case GameStates.Pause:
                    if (spot.Type == SpotTypes.TowerSpot)
                    {
                        if (SelectedTower != null)
                        {
                            if (spot.Occupied)
                            {
                                SelectedTower = spot.Tower;
                            }
                            else
                            {
                                AssignSpot(SelectedTower, spot);
                            }
                        }
                        else
                        {
                            SelectedTower = spot.Tower;
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
                    SelectedTower = tower;
                    _gameManager.ChangeState(GameStates.Pause);
                    break;

                case GameStates.Pause:
                    _gameManager.ChangeState(GameStates.Play);
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
