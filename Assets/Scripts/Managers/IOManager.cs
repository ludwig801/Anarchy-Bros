using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

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
            List<Node> nodes = GraphManager.Instance.Nodes;
            List<Edge> edges = GraphManager.Instance.Edges;
            GameGraph toSave = new GameGraph();

            for (int i = 0; i < nodes.Count; i++)
            {
                GameNode gameNode = new GameNode();
                gameNode.Position = new Point(nodes[i].transform.position);
                gameNode.Type = nodes[i].Type;

                toSave.Nodes.Add(gameNode);
            }

            for (int i = 0; i < edges.Count; i++)
            {
                GameEdge gameEdge = new GameEdge();
                gameEdge.a = new Point(edges[i].A.transform.position);
                gameEdge.b = new Point(edges[i].B.transform.position);

                toSave.Edges.Add(gameEdge);
            }

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

                GraphManager.Instance.RebuildGraph(graph);

                graph.Nodes.Clear();
                graph.Edges.Clear();
            }
        }

        [Serializable]
        public class GameGraph
        {
            public List<GameNode> Nodes;
            public List<GameEdge> Edges;

            public GameGraph()
            {
                Nodes = new List<GameNode>();
                Edges = new List<GameEdge>();
            }
        }

        [Serializable]
        public class GameNode
        {
            public Node.NodeType Type;
            public Point Position;
        }

        [Serializable]
        public class GameEdge
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
