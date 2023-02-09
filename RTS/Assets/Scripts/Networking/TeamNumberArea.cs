using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamNumberArea : MonoBehaviour
{
    [SerializeField] private int teamNumber;

    public int GetTeamNumber()
    {
        return teamNumber;
    }
}
