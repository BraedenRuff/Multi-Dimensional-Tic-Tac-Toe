using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerOptions
{
    private static int kills, deaths, assists, points;

    public static int Kills
    {
        get
        {
            return kills;
        }
        set
        {
            kills = value;
        }
    }

    public static int Deaths
    {
        get
        {
            return deaths;
        }
        set
        {
            deaths = value;
        }
    }

    public static int Assists
    {
        get
        {
            return assists;
        }
        set
        {
            assists = value;
        }
    }

    public static int Points
    {
        get
        {
            return points;
        }
        set
        {
            points = value;
        }
    }
}

