namespace AnarchyBros.Enums
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
        Any = -1,
        Connection = 0,
        TowerSpot = 1,
        EnemySpawn = 2
    }
}