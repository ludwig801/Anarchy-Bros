using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        public int SelectedPlayer;
        public List<Player> Players;

        void Start()
        {
            Instance = this;

            SelectedPlayer = int.MinValue;
            GetPlayers();
        }

        void Update()
        {
        }

        public void OnNodeClicked(Node n)
        {
            if (n.Type == Node.NodeType.PlayerSpot)
            {
                PlayerSpot spot = NodeManager.Instance.GetPlayerSpot(n.transform.position);

                if (SelectedPlayer >= 0)
                {
                    if (spot.Occupied)
                    {
                        SelectedPlayer = GetPlayerIndex(spot.Player);
                    }
                    else
                    {
                        MoveSelectedPlayerTo(spot);
                    }
                }
                else
                {
                    SelectedPlayer = GetPlayerIndex(spot.Player);
            }
            }
        }

        public void OnPlayerClicked(Player p)
        {
            SelectedPlayer = GetPlayerIndex(p);
        }

        void GetPlayers()
        {
            if (Players == null)
            {
                Players = new List<Player>();
            }
            else
            {
                Players.Clear();
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).tag == "Player")
                {
                    Player p = transform.GetChild(i).GetComponent<Player>();
                    Players.Add(p);
                }
            }
        }

        void MoveSelectedPlayerTo(PlayerSpot spot)
        {
            Player p = Players[SelectedPlayer];
            p.CurrentSpot.Player = null;
            p.CurrentSpot = spot;
            p.CurrentSpot.Player = p;
        }

        int GetPlayerIndex(Player p)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i] == p)
                {
                    return i;
                }
            }

            return int.MinValue;
        }

        public void SetPlayersActive(bool value)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].SetActive(value);
            }
        }
    }
}
