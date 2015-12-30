using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class IOManager : MonoBehaviour
    {
        public static IOManager Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
        }

        public void SaveGraph()
        {
            List<Spot> spots = MapManager.Instance.Graph.Spots;
            List<Edge> edges = MapManager.Instance.Graph.Edges;
            GameGraph toSave = new GameGraph();

            for (int i = 0; i < spots.Count; i++)
            {
                IOSpot gameNode = new IOSpot();
                gameNode.Position = new Point(spots[i].transform.position);
                gameNode.Type = spots[i].Type;

                toSave.Spots.Add(gameNode);
            }

            for (int i = 0; i < edges.Count; i++)
            {
                IOEdge gameEdge = new IOEdge();
                gameEdge.a = new Point(edges[i].A.transform.position);
                gameEdge.b = new Point(edges[i].B.transform.position);

                toSave.Edges.Add(gameEdge);
            }

            Debug.Log("SAVE: " + toSave.Spots.Count + " spots & " + toSave.Edges.Count);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.dataPath + "/GraphData.sav");
            bf.Serialize(file, toSave);
            file.Close();
        }

        public void LoadGraph()
        {
            if (File.Exists(Application.dataPath + "/GraphData.sav"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.dataPath + "/GraphData.sav", FileMode.Open);
                GameGraph graph = (GameGraph)bf.Deserialize(file);
                file.Close();

                //Debug.Log("LOAD: " + graph.Spots.Count + " spots & " + graph.Edges.Count);

                MapManager.Instance.RebuildGraph(graph);

                graph.Spots.Clear();
                graph.Edges.Clear();
            }
        }

        [Serializable]
        public class GameGraph
        {
            public List<IOSpot> Spots;
            public List<IOEdge> Edges;

            public GameGraph()
            {
                Spots = new List<IOSpot>();
                Edges = new List<IOEdge>();
            }
        }

        [Serializable]
        public class IOSpot
        {
            public SpotTypes Type;
            public Point Position;
        }

        [Serializable]
        public class IOEdge
        {
            public Point a, b;
        }

        [Serializable]
        public class Point
        {
            public float x, y;

            public Vector2 ToVector2
            {
                get
                {
                    return new Vector2(x, y);
                }
            }

            public Point(Vector2 pos)
            {
                x = pos.x;
                y = pos.y;
            }
        }
    }
}
