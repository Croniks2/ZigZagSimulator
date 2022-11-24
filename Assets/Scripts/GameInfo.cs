using System;
using UnityEngine;

public enum PlatformSize { Small = 0, Medium = 1, Large = 2 };
public enum UIViewType { Menu, MenuButtons, MenuSettings, Preparation, Game };

public enum MoveDirection { Right = 0, Left = 1 };

[Serializable]
public struct PointsInfo
{
    public Vector3 firsPoint;
    public Vector3 secondPoint;
}

[Serializable]
public class LevelInfo
{
    public PlatformSize levelDifficulty;
    public ReusablePlatform platformPrefab;
}

public class PlatformsInfo
{
    public int maxPlatformsCount;
    public ReusablePlatform platformPrefab;

    public PlatformsInfo(int maxPlatformsCount, ReusablePlatform platformPrefab)
    {
        this.maxPlatformsCount = maxPlatformsCount;
        this.platformPrefab = platformPrefab;
    }
}

public class PlayerPrefsSettingsNames
{
    public static string LevelDifficulty => "LevelDifficulty";
    public static string MoveSpeed => "SphereSpeed";
}