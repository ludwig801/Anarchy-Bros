using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
            List<Node> nodes = NodeManager.Instance.Nodes;
            List<GameNode> toSave = new List<GameNode>();

            for (int i = 0; i < nodes.Count; i++)
            {
                List<Edge> neighbors = nodes[i].Edges;
                GameNode gameNode = new GameNode();
                gameNode.Position = new Point(nodes[i].transform.position.x, nodes[i].transform.position.y);

                for (int j = 0; j < neighbors.Count; j++)
                {
                    Vector2 pt = neighbors[j].GetNeighbor(nodes[i]).transform.position;
                    gameNode.Neighbors.Add(new Point(pt.x, pt.y));
                }

                toSave.Add(gameNode);
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
                List<GameNode> nodes = (List<GameNode>)bf.Deserialize(file);
                file.Close();
                
                // TODO : Rebuild graph
            }
        }

        [System.Serializable]
        public class GameNode
        {
            public enum NodeType { PlayerSpot = 0, SpawnPoint = 1, Node = 2 }

            public NodeType Type;
            public Point Position;
            public List<Point> Neighbors;

            public GameNode()
            {
                Neighbors = new List<Point>();
            }
        }

        [System.Serializable]
        public class Point
        {
            public float x, y;

            public Point(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
