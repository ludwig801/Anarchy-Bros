using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class PlayerManager : MonoBehaviour
    {
        public int SelectedPlayer;
        public List<Player> _players;
        public List<PlayerSpot> _spots;

        void Start()
        {
            SelectedPlayer = int.MinValue;
            GetPlayers();
            GetSpots();
        }

        void Update()
        {
            if (GameManager.Instance.CurrentState == GameManager.State.Playing)
            {
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Input.GetMouseButtonDown(0))
                {
                    for (int i = 0; i < _players.Count; i++)
                    {
                        if (_players[i].Collider.OverlapPoint(point))
                        {
                            SelectedPlayer = (SelectedPlayer == i) ? int.MinValue : i;
                            break;
                        }
                    }

                    for (int i = 0; i < _spots.Count; i++)
                    {
                        if (SelectedPlayer >= 0)
                        {
                            if (!_spots[i].Occupied && _spots[i].Collider.OverlapPoint(point))
                            {
                                MoveTo(_spots[i]);
                            }
                        }
                    }
                }
            }
        }

        void GetPlayers()
        {
            if (_players == null)
            {
                _players = new List<Player>();
            }
            else
            {
                _players.Clear();
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).tag == "Player")
                {
                    Player p = transform.GetChild(i).GetComponent<Player>();
                    p.CurrentSpot.Occupied = true;
                    _players.Add(p);
                }
            }
        }

        public void GetSpots()
        {
            Transform t = GameObject.FindGameObjectWithTag("Player Spots").transform;
            if (_spots == null)
            {
                _spots = new List<PlayerSpot>();
            }
            else
            {
                _spots.Clear();
            }

            for (int i = 0; i < t.childCount; i++)
            {
                _spots.Add(t.GetChild(i).GetComponent<PlayerSpot>());
            }
        }

        void MoveTo(PlayerSpot spot)
        {
            _players[SelectedPlayer].CurrentSpot.Occupied = false;
            _players[SelectedPlayer].CurrentSpot = spot;
            _players[SelectedPlayer].CurrentSpot.Occupied = true;
        }
    }
}
