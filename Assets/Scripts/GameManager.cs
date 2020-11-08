using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private PowerLine[] powerLines;

    private Dictionary<PowerLine, PowerLine> joints;

    public bool allConnected;

    public void Start()
    {
        powerLines = FindObjectsOfType<PowerLine>();
        joints = new Dictionary<PowerLine, PowerLine>();
    }

    public void PowerLineJoined(PowerLine a, PowerLine b)
    {
        a.Join(b);
        joints[a] = b;

        if (CheckIfAllConnected()) allConnected = true;
    }

    private bool CheckIfAllConnected()
    {
        var list = powerLines.ToList();
        var next = list[0];
        list.RemoveAt(0);
        if (!joints.ContainsKey(next))
        {
            Debug.Log($"not enouth {list.Count}");
            return false;
        }
        while (list.Count > 1)
        {
            if (!joints.ContainsKey(joints[next]))
            {
                Debug.Log($"not enouth {list.Count}");
                return false;
            }
            next = joints[joints[next]];
        }
        Debug.Log("Win?");
        return true;
    }
    
    
}