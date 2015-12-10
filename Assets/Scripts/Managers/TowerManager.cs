using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;

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

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _selected = int.MinValue;

            Towers = new List<Tower>();
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
                tower.Spot.Pawn = null;
            }
            tower.Spot = spot;
            tower.Spot.Pawn = tower;
        }

        void MoveTowerImmediate(Tower tower, Node spot)
        {
            MoveTower(tower, spot);
            tower.transform.position = Tools2D.Move(tower.transform.position, spot.transform.position);
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
            GameObject instance = Instantiate(TowerPrefab);
            Tower tower = instance.GetComponent<Tower>();
            Towers.Add(tower);
            MoveTowerImmediate(tower, spot);
        }

        public void OnNodeClicked(Node node)
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Place))
            {
                if (Towers.Count < MaxTowerCount && node.Type == Node.NodeType.TowerSpot)
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
                            _selected = GetTowerIndex(node.Pawn);
                        }
                        else
                        {
                            MoveTower(Towers[_selected], node);
                        }
                    }
                    else
                    {
                        _selected = GetTowerIndex(node.Pawn);
                    }
                }
            }           
        }

        public void OnTowerClicked(Tower p)
        {
            _selected = GetTowerIndex(p);
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
