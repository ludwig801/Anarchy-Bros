using UnityEngine;
using System.Collections.Generic;
using Enums;
using UnityEngine.EventSystems;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    public Transform BulletCan;
    public GameObject TowerPrefab;
    public Piece SelectedTower;
    public int MaxTowerCount, ActiveTowers;
    public List<Piece> Towers;

    GameManager _gameManager;
    EnemyManager _enemyManager;
    MapManager _mapManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _enemyManager = EnemyManager.Instance;
        _mapManager = MapManager.Instance;

        ActiveTowers = 0;
    }

    void Update()
    {
    }

    void AssignTowerToSpot(Piece tower, Spot spot)
    {
        spot.Tower = tower;
    }

    void PlaceTower(Spot spot)
    {
        Piece tower = GetTower();
        tower.gameObject.SetActive(true);
        spot.Tower = tower;
        tower.transform.position = Tools2D.Convert(tower.transform.position, spot.transform.position);
        tower.Target = spot.transform;
        tower.MoveTo = spot.transform;
        tower.Live();
        ActiveTowers++;
    }

    void RemoveTower(Piece tower)
    {
        Spot hit;
        if (_mapManager.SpotAt(tower.Target.transform.position, out hit))
        {
            hit.Tower = null;
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

    Piece GetTower()
    {
        for (int i = 0; i < Towers.Count; i++)
        {
            if (!Towers[i].gameObject.activeSelf)
            {
                return Towers[i];
            }
        }

        GameObject instance = Instantiate(TowerPrefab);
        instance.name = "Tower";
        instance.transform.parent = transform;

        Piece tower = instance.GetComponent<Piece>();
        Towers.Add(tower);
        tower.SetHealthElement(UIManager.Instance.GetHealthElement());

        return tower;
    }

    public Piece TowerAt(Vector2 pos)
    {
        for (int i = 0; i < Towers.Count; i++)
        {
            if (Towers[i].Collider.OverlapPoint(pos))
            {
                return Towers[i];
            }
        }

        return null;
    }

    public Piece GetRandomTower()
    {
        int rand = Random.Range(0, ActiveTowers);
        for (int i = 0; i < Towers.Count; i++)
        {
            if (Towers[i].Alive)
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

    public Piece GetNearestEnemy(Transform towerTransform)
    {
        float minDist = float.MaxValue;
        Piece closest = null;

        for (int i = 0; i < _enemyManager.Enemies.Count; i++)
        {
            Piece enemy = _enemyManager.Enemies[i];
            if (enemy.Alive)
            {
                float d = _mapManager.DistanceBetween(towerTransform, enemy.transform);
                if (d < minDist)
                {
                    minDist = d;
                    closest = enemy;
                }
            }
        }

        return closest;
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
                            AssignTowerToSpot(SelectedTower, spot);
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
                            AssignTowerToSpot(SelectedTower, spot);
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

    public void OnTowerClicked(PointerEventData eventData, Piece tower)
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

    public void OnTowerDie()
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