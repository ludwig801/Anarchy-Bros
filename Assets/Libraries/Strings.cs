using System.Collections.Generic;

public enum GameStates
{
    Play = 0,
    Place = 1,
    Edit = 2,
    Pause = 3,
    GameOver
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

public enum Tags
{
    Tower,
    Enemy
}

public enum LevelEras
{
    Egyptian,
    Greek,
    Roman
}

public enum SortingLayers
{
    Default,
    Background,
    Graph,
    Pieces
}

public enum MenuPanels
{
    Initial,
    GroupSelection,
    LevelSelection
}
