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
        public int MaxTowerCount;
        public List<Tower> Towers;

        int _selected;
        int _activeTowers;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _selected = int.MinValue;

            Towers = new List<Tower>();
            _activeTowers = 0;
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

        void MoveTower(Tower tower, Node spot)
        {
            if (tower.Spot != null)
            {
                tower.Spot.Tower = null;
            }
            tower.Spot = spot;
            tower.Spot.Tower = tower;
        }

        void MoveTowerImmediate(Tower tower, Node spot)
        {
            MoveTower(tower, spot);
            tower.transform.position = Tools2D.ConvertKeepZ(tower.transform.position, spot.transform.position);
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

        void PlaceTower(Node spot)
        {
            Tower tower;

            if (_activeTowers < Towers.Count)
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

            MoveTowerImmediate(tower, spot);
            _activeTowers++;
        }

        void RemoveTower(Tower tower)
        {
            if (tower.Spot != null)
            {
                tower.Spot.Tower = null;
                tower.Spot = null;
            }

            tower.gameObject.SetActive(false);
            _activeTowers--;
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

        public void OnNodeClicked(Node node)
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Place))
            {
                if (node.Type == Node.NodeType.TowerSpot && _activeTowers < MaxTowerCount)
                {
                    PlaceTower(node);
                }
            }
            else if(GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                if (node.Type == Node.NodeType.TowerSpot)
                {
                    if (_selected >= 0)
                    {
                        if (node.Occupied)
                        {
                            _selected = GetTowerIndex(node.Tower);
                        }
                        else
                        {
                            MoveTower(Towers[_selected], node);
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

        public void ReEvaluate()
        {
            GetTowers();

            for (int i = 0; i < Towers.Count; i++)
            {
                Towers[i].Spot = GraphManager.Instance.GetHitNode<Node>(Towers[i].transform.position);
                Towers[i].gameObject.SetActive(GameManager.Instance.IsCurrentState(GameStates.Play));
            }
        }
    }
}
