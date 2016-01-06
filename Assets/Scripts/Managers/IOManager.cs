using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using Enums;

public class IOManager : MonoBehaviour
{
    public static IOManager Instance { get; private set; }

    public TextAsset Level;

    MapManager _mapManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _mapManager = MapManager.Instance;

        LoadGraph();
    }

    public void SaveGraph()
    {
        List<Spot> spots = _mapManager.Spots;
        List<Edge> edges = _mapManager.Edges;
        GameGraph toSave = new GameGraph();

        for (int i = 0; i < spots.Count; i++)
        {
            IOSpot gameSpot = new IOSpot();
            gameSpot.Position = new Point(spots[i].transform.position);
            gameSpot.Type = spots[i].Type;

            toSave.Spots.Add(gameSpot);
        }

        for (int i = 0; i < edges.Count; i++)
        {
            IOEdge gameEdge = new IOEdge();
            gameEdge.a = new Point(edges[i].A.transform.position);
            gameEdge.b = new Point(edges[i].B.transform.position);

            toSave.Edges.Add(gameEdge);
        }

        BinaryFormatter bf = new BinaryFormatter();
        Stream file = File.Create(Application.dataPath + "/Resources/GraphData.txt");
        bf.Serialize(file, toSave);
        file.Close();
    }

    public void LoadGraph()
    {
        //TextAsset level = Resources.Load("GraphData", typeof(TextAsset)) as TextAsset;
        Stream s = new MemoryStream(Level.bytes);

        BinaryFormatter bf = new BinaryFormatter();
        GameGraph graph = (GameGraph)bf.Deserialize(s);

        _mapManager.RebuildGraph(graph);

        s.Close();

        graph.Spots.Clear();
        graph.Edges.Clear();
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