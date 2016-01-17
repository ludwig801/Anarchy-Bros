using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public Transform ObjBullets;
    public GameObject TowerPrefab;
    public int MaxNumTowers;
    public PieceBehavior SelectedTower;
    public List<PieceBehavior> Objects;
    public int AliveTowers;
    public bool HasSelectedTower { get { return SelectedTower != null; } }

    GameManager _gameManager;

    void Start()
    {
        _gameManager = GameManager.Instance;
        AliveTowers = 0;
    }

    PieceBehavior Find()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            PieceBehavior x = Objects[i];
            if (x.Reciclable)
            {
                return Objects[i];
            }
        }

        PieceBehavior piece = Instantiate(TowerPrefab).GetComponent<PieceBehavior>();
        piece.name = tag.ToString();
        piece.transform.parent = transform;
        Objects.Add(piece);

        return piece;
    }

    public bool Find(Vector2 pos, out PieceBehavior tower)
    {
        tower = null;
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Tools2D.SamePos(pos, Objects[i].transform.position, 0.5f * Objects[i].transform.localScale.x))
            {
                tower = Objects[i];
            }
        }

        return (tower != null);
    }

    public bool NoTowerHasTarget(MapSpot target)
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Movement.Target == target)
            {
                return false;
            }
        }

        return true;
    }

    public void SelectTower(PieceBehavior tower)
    {
        SelectedTower = tower;
    }

    public void MoveTower(PieceBehavior tower, MapSpot moveTo)
    {
        _gameManager.Map.Graph.RemovePieceFromSpot(tower);
        tower.Movement.Target = moveTo;
        moveTo.Piece = tower;
    }

    public bool GetClosestTower(MoveBehavior requester, out MoveBehavior closestTower)
    {
        float minDist = float.MaxValue;
        closestTower = null;

        for (int i = 0; i < Objects.Count; i++)
        {
            PieceBehavior tower = Objects[i];
            if (tower.Alive)
            {
                float d = _gameManager.Map.GraphLogic.DistanceBetween(requester, tower.Movement);
                if (d < minDist)
                {
                    minDist = d;
                    closestTower = tower.Movement;
                }
            }
        }

        return (closestTower != null);
    }

    int CountActiveTowers()
    {
        int count = 0;

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].gameObject.activeSelf)
            {
                count++;
            }
        }

        return count;
    }

    int CountAliveTowers()
    {
        int count = 0;

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Alive)
            {
                count++;
            }
        }

        return count;
    }

    public void Spawn(MapSpot spot)
    {
        if (AliveTowers < MaxNumTowers)
        {
            PieceBehavior obj = Find();
            obj.name = TowerPrefab.name;
            obj.gameObject.SetActive(true);
            obj.transform.position = Tools2D.Convert(obj.transform.position, spot.transform.position);
            obj.Live();
            spot.Piece = obj;
            obj.Movement.Target = spot;
            obj.Movement.CurrentSpot = spot;
            obj.GetComponent<RangedBehavior>().BulletCan = ObjBullets;
            _gameManager.UI.CreateTowerHealthElement(obj);
            AliveTowers++;
        }
    }

    public bool GetRandomTower(out PieceBehavior tower)
    {
        tower = null;

        int rand = UnityEngine.Random.Range(0, AliveTowers);
        for (int i = 0; i < Objects.Count; i++)
        {
            PieceBehavior x = Objects[i];
            if (x.Alive)
            {
                if (rand <= 0)
                {
                    tower = x;
                    break;
                }
                rand--;
            }
        }

        return (tower != null);
    }

    public void Remove(PieceBehavior tower)
    {
        Objects.Remove(tower);
        Destroy(tower.gameObject);
        AliveTowers--;
    }

    public void RemoveAll()
    {
        RemoveAllBullets();

        for (int i = 0; i < Objects.Count; i++)
        {
            Destroy(Objects[i].gameObject);
        }

        Objects.Clear();
    }

    public void KillAll()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            PieceBehavior x = Objects[i];
            if (x.Alive)
            {
                StartCoroutine(x.Die());
            }
        }
    }

    public void OnTowerKilled()
    {
        AliveTowers--;
    }

    public void RemoveAllBullets()
    {
        for (int i = 0; i < ObjBullets.childCount; i++)
        {
            Destroy(ObjBullets.GetChild(i).gameObject);
        }
    }

    public bool OnPointerClick(Vector2 pos)
    {
        PieceBehavior tower;
        if (Find(pos, out tower))
        {
            return true;
        }

        return false;
    }
}
