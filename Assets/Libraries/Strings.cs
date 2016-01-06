using System.Collections.Generic;

namespace Enums
{
    public enum GameStates
    {
        Stop = 0,
        Play = 1,
        Place = 2,
        Edit = 3,
        Pause = 4,
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
        EnemySpot = 2
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
}