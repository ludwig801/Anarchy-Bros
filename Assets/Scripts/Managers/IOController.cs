using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class IOController : MonoBehaviour
{
    public TextAsset Level;

    GameManager _gameController;

    void Start()
    {
        _gameController = GameManager.Instance;
    }

    public void SaveGraph()
    {
        Map map = _gameController.Map;
        List<Spot> spots = map.Graph.Spots;
        List<Edge> edges = map.Graph.Edges;

        GameGraph graph = new GameGraph();
        for (int i = 0; i < spots.Count; i++)
        {
            graph.AddSpot(spots[i]);
        }

        for (int i = 0; i < edges.Count; i++)
        {
            graph.AddEdge(edges[i]);
        }

        BinaryFormatter bf = new BinaryFormatter();
        Stream file = File.Create(Application.dataPath + "/Resources/GraphData.txt");
        bf.Serialize(file, graph);
        file.Close();
    }

    public void LoadGraph()
    {
        Map map = _gameController.Map;

        Stream s = new MemoryStream(Level.bytes);

        BinaryFormatter bf = new BinaryFormatter();
        GameGraph graph = (GameGraph)bf.Deserialize(s);

        map.Graph.RemoveAll();

        for (int i = 0; i < graph.Spots.Count; i++)
        {
            IOSpot spot = graph.Spots[i];
            map.Graph.CreateSpot(spot.Position.ToVector2, spot.Type);
        }

        for (int i = 0; i < graph.Edges.Count; i++)
        {
            IOEdge edge = graph.Edges[i];
            map.Graph.CreateEdge(map.Graph.FindSpot(edge.a.ToVector2), map.Graph.FindSpot(edge.b.ToVector2));
        }
        
        s.Close();
    }

    [Serializable]
    class GameGraph
    {
        internal List<IOSpot> Spots;
        internal List<IOEdge> Edges;

        internal GameGraph()
        {
            Spots = new List<IOSpot>();
            Edges = new List<IOEdge>();
        }

        internal void AddSpot(Spot spot)
        {
            IOSpot x = new IOSpot();
            x.Position = new Point(spot.transform.position);
            x.Type = spot.Type;
            Spots.Add(x);
        }

        internal void AddEdge(Edge edge)
        {
            IOEdge x = new IOEdge();
            x.a = new Point(edge.A.transform.position);
            x.b = new Point(edge.B.transform.position);
            Edges.Add(x);
        }
    }

    [Serializable]
    class IOSpot
    {
        internal SpotTypes Type;
        internal Point Position;
    }

    [Serializable]
    class IOEdge
    {
        internal Point a, b;
    }

    [Serializable]
    class Point
    {
        internal float x, y;

        internal Vector2 ToVector2
        {
            get
            {
                return new Vector2(x, y);
            }
        }

        internal Point(Vector2 pos)
        {
            x = pos.x;
            y = pos.y;
        }
    }
}