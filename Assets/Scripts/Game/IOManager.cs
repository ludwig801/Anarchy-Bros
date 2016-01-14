using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class IOManager : MonoBehaviour
{
    GameManager _gameManager;

    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void SaveGraph()
    {
        MapManager map = _gameManager.Map;
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


        XmlSerializer serializer = new XmlSerializer(typeof(GameGraph));
        if (!Directory.Exists(Application.persistentDataPath + "/Resources/Levels/" + LevelManager.Instance.CurrentEra.Title))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Resources/Levels/" + LevelManager.Instance.CurrentEra.Title);
        }

        StreamWriter stream = new StreamWriter(Application.persistentDataPath + "/Resources/Levels/" + LevelManager.Instance.CurrentEra.Title +
            "/lvl_" + LevelManager.Instance.CurrentLevel.Order + ".xml", false, System.Text.Encoding.GetEncoding("UTF-8"));

        serializer.Serialize(stream, graph);
        stream.Dispose();
    }

    public void LoadGraph()
    {
        MapManager map = _gameManager.Map;
        bool existed = false;

        XmlSerializer serializer = new XmlSerializer(typeof(GameGraph));
        Stream stream;
        if (File.Exists(Application.persistentDataPath + "/Resources/Levels/"
            + LevelManager.Instance.CurrentEra + "/lvl_" + LevelManager.Instance.CurrentLevel.Order + ".xml"))
        {
            //Debug.Log("Loading from persistent data path...");
            existed = true;
            stream = new FileStream(Application.persistentDataPath + "/Resources/Levels/"
            + LevelManager.Instance.CurrentEra + "/lvl_" + LevelManager.Instance.CurrentLevel.Order + ".xml", FileMode.Open);
        }
        else
        {
            //Debug.Log("Loading from resources...");
            string path = "Levels/" + LevelManager.Instance.CurrentEra.Title + "/lvl_" + LevelManager.Instance.CurrentLevel.Order;
            TextAsset lvl = Resources.Load(path, typeof(TextAsset)) as TextAsset;
            stream = new MemoryStream(lvl.bytes);
        }
            
        GameGraph graph = serializer.Deserialize(stream) as GameGraph;
        stream.Dispose();

        _gameManager.Map.Graph.RemoveAll();

        for (int i = 0; i < graph.Spots.Count; i++)
        {
            IOSpot spot = graph.Spots[i];
            map.Graph.CreateSpot(spot.Position.ToVector2, spot.Type);
        }

        for (int i = 0; i < graph.Edges.Count; i++)
        {
            IOEdge edge = graph.Edges[i];
            map.Graph.CreateEdge(map.Graph.FindSpotExact(edge.a.ToVector2), map.Graph.FindSpotExact(edge.b.ToVector2));
        }

        if (!existed)
        {
            SaveGraph();
        }
    }
}

[XmlRoot("GameGraph")]
public class GameGraph
{
    [XmlArray("Spots"), XmlArrayItem("Spot")]
    public List<IOSpot> Spots;
    [XmlArray("Edges"), XmlArrayItem("Edge")]
    public List<IOEdge> Edges;

    public GameGraph()
    {
        Spots = new List<IOSpot>();
        Edges = new List<IOEdge>();
    }

    public void AddSpot(Spot spot)
    {
        IOSpot x = new IOSpot();
        x.Position = new Point();
        x.Position.x = spot.transform.position.x;
        x.Position.y = spot.transform.position.y;
        x.Type = spot.Type;
        Spots.Add(x);
    }

    public void AddEdge(Edge edge)
    {
        IOEdge ioEdge = new IOEdge();
        ioEdge.a = new Point();
        ioEdge.a.x = edge.A.transform.position.x;
        ioEdge.a.y = edge.A.transform.position.y;
        ioEdge.b = new Point();
        ioEdge.b.x = edge.B.transform.position.x;
        ioEdge.b.y = edge.B.transform.position.y;
        Edges.Add(ioEdge);
    }
}

[XmlRoot("Spot")]
public class IOSpot
{
    public SpotTypes Type;
    public Point Position;
}

[XmlRoot("Edge")]
public class IOEdge
{
    public Point a, b;
}

[System.Serializable]
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
}