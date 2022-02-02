using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayers
{
    public GameObject Player;
    public float Distance;

    public TargetPlayers(GameObject player, float distance)
    {
        Player = player;
        Distance = distance;
    }
}
