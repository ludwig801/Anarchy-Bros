using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class TowerManager : MonoBehaviour
    {
        public static TowerManager Instance { get; private set; }

        public Transform TowersObj;
        public GameObject TowerPrefab;
        public int MaxTowerCount, ActiveTowers;
        public List<Tower> Towers;

        int _selected;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _selected = int.MinValue;

            Towers = new List<Tower>();
            ActiveTowers = 0;
        }

        void Update()
        {
        }

        void GetTowers()
        {
            if (Towers == null)
            {
                Towers = new List<Tower>();
            }
            else
            {
                Towers.Clear();
            }

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

        void PlaceTower(Spot spot)
        {
            Tower tower;

            if (ActiveTowers < Towers.Count)
            {
                tower = GetInactiveTower();
                tower.gameObject.SetActive(true);
            }
            else
            {
                GameObject instance = Instantiate(TowerPrefab);
                instance.name = "Tower";
                instance.transform.parent = TowersObj;

                tower = instance.GetComponent<Tower>();
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

        Tower GetInactiveTower()
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

        public void OnNodeClicked(Spot node)
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Place))
            {
                if (node.Type == SpotTypes.TowerSpot && ActiveTowers < MaxTowerCount)
                {
                    PlaceTower(node);
                }
            }
            else if(GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                if (node.Type == SpotTypes.TowerSpot)
                {
                    if (_selected >= 0)
                    {
                        if (node.Occupied)
                        {
                            _selected = GetTowerIndex(node.Tower);
                        }
                        else
                        {
                            AssignSpot(Towers[_selected], node);
                        }
                    }
                    else
                    {
                        _selected = GetTowerIndex(node.Tower);
                    }
                }
            }           
        }

        public void OnTowerClicked(PointerEventData eventData, Tower tower)
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                _selected = GetTowerIndex(tower);
            }
            else if (GameManager.Instance.IsCurrentState(GameStates.Place))
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    RemoveTower(tower);
                }
            }
        }

        public void OnTowerKill()
        {
            ActiveTowers--;
        }

        public void DestroyAllTowers()
        {
            for (int i = 0; i < Towers.Count; i++)
            {
                Destroy(Towers[i].gameObject);
            }

            Towers.Clear();
            ActiveTowers = 0;
        }

        public void ReEvaluate()
        {
            switch (GameManager.Instance.CurrentState)
            {
                case GameStates.Edit:
                    DestroyAllTowers();
                    break;

                case GameStates.Place:
                    DestroyAllTowers();
                    break;
            }
        }
    }
}
