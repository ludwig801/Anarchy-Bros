using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public Transform ObjBullets;
    public GameObject TowerPrefab;
    public int MaxNumTowers;
    public float TowerHitTolerance;
    public Piece SelectedTower;
    public List<Piece> Objects;
    public int ActiveTowers { get { return CountActiveTowers(); } }
    public bool HasSelectedTower { get { return SelectedTower != null; } }

    Piece Find()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Piece x = Objects[i];
            if (!x.Alive)
            {
                return Objects[i];
            }
        }

        Piece piece = Instantiate(TowerPrefab).GetComponent<Piece>();
        piece.name = tag.ToString();
        piece.transform.parent = transform;
        Objects.Add(piece);
        //piece.SetHealthElement(_uiController.GetHealthElement());

        return piece;
    }

    public bool Find(Vector2 pos, out Piece tower)
    {
        tower = null;
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Tools2D.SamePos(pos, Objects[i].transform.position, TowerHitTolerance))
            {
                tower = Objects[i];
            }
        }

        return (tower != null);
    }

    public bool NoTowerHasTarget(Spot target)
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

    public void SelectTower(Piece tower)
    {
        SelectedTower = tower;
    }

    public void MoveTower(Piece tower, Spot moveTo)
    {
        tower.Movement.Target = moveTo;
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

    public void Spawn(Spot spot)
    {
        if (ActiveTowers < MaxNumTowers)
        {
            Piece obj = Find();
            obj.name = TowerPrefab.name;
            obj.gameObject.SetActive(true);
            obj.transform.position = Tools2D.Convert(obj.transform.position, spot.transform.position);
            obj.Live();
            spot.Tower = obj;
            obj.Movement.Target = spot;
            obj.Movement.Step = spot;
        }
        else
        {
            Debug.Log("Tower spawn is not possible: maximum towers achieved");
        }
    }

    public Piece Random()
    {
        int rand = UnityEngine.Random.Range(0, ActiveTowers);
        for (int i = 0; i < Objects.Count; i++)
        {
            Piece x = Objects[i];
            if (x.Alive)
            {
                if (rand <= 0) return x;
                rand--;
            }
        }

        return null;
    }

    public void Remove(Piece tower)
    {
        Objects.Remove(tower);
        Destroy(tower.gameObject);
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
            Piece x = Objects[i];
            if (x.Alive)
            {
                StartCoroutine(x.Die());
            }
        }
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
        Piece tower;
        if (Find(pos, out tower))
        {
            return true;
        }

        return false;
    }
}
