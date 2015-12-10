using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class PawnManager : MonoBehaviour
    {
        public static PawnManager Instance { get; private set; }

        public Transform PawnsObj;
        public int MaxPawnCount;
        public List<Pawn> Pawns;

        int _selected;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _selected = int.MinValue;

            GetPawns();
        }

        void Update()
        {
        }

        void GetPawns()
        {
            if (Pawns == null)
            {
                Pawns = new List<Pawn>();
            }
            else
            {
                Pawns.Clear();
            }

            for (int i = 0; i < PawnsObj.childCount; i++)
            {
                Pawn p = PawnsObj.GetChild(i).GetComponent<Pawn>();
                Pawns.Add(p);
            }
        }

        void MovePawnTo(Pawn p, Node spot)
        {
            p.CurrentSpot.Pawn = null;
            p.CurrentSpot = spot;
            p.CurrentSpot.Pawn = p;
        }

        void MovePawnToImmediate(Pawn p, Node spot)
        {
            MovePawnTo(p, spot);
            p.transform.position = spot.transform.position;
        }

        int GetPawnIndex(Pawn p)
        {
            for (int i = 0; i < Pawns.Count; i++)
            {
                if (Pawns[i] == p)
                {
                    return i;
                }
            }

            return int.MinValue;
        }

        void PlacePawn(Node n)
        {
            // TODO
        }

        public void OnNodeClicked(Node n)
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Place))
            {
                if (Pawns.Count < MaxPawnCount)
                {
                    PlacePawn(n);
                }
            }
            else if(GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                if (n.Type == Node.NodeType.PlayerSpot)
                {
                    if (_selected >= 0)
                    {
                        if (n.Occupied)
                        {
                            _selected = GetPawnIndex(n.Pawn);
                        }
                        else
                        {
                            MovePawnTo(Pawns[_selected], n);
                        }
                    }
                    else
                    {
                        _selected = GetPawnIndex(n.Pawn);
                    }
                }
            }           
        }

        public void OnPawnClicked(Pawn p)
        {
            _selected = GetPawnIndex(p);
        }

        public void ReEvaluate()
        {
            GetPawns();

            for (int i = 0; i < Pawns.Count; i++)
            {
                Pawns[i].CurrentSpot = NodeManager.Instance.GetHitNode<Node>(Pawns[i].transform.position);
                Pawns[i].gameObject.SetActive(GameManager.Instance.IsCurrentState(GameStates.Play));
            }
        }
    }
}
