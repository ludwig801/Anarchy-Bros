using System.Collections.Generic;

public enum GameStates
{
    Play = 0,
    Place = 1,
    Edit = 2,
    Pause = 3
}

public enum GraphModes
{
    Edge = 0,
    TowerSpot = 1,
    EnemySpawn = 2
}

public enum SpotTypes
{
    Connection = 0,
    TowerSpot = 1,
    EnemySpawn = 2
}

public abstract class Tags
{
    public enum Tag
    {
        Tower,
        Enemy
    }

    static Dictionary<Tag, string> dictionary = new Dictionary<Tag, string>
    {
        { Tag.Tower, "Tower" },
        { Tag.Enemy, "Enemy" }
    };

    public static string GetStringTag(Tag tagIdentifier)
    {
        string result = "";
        dictionary.TryGetValue(tagIdentifier, out result);
        return result;
    }
}