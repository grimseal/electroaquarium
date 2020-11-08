using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private PowerLine[] powerLines;
    private Dictionary<PowerLine, PowerLine> joints;
    [SerializeField] private PowerLine finishPowerLine;
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
        list.Remove(next);
        if (!joints.ContainsKey(next)) return false;
        while (list.Count > 1)
        {
            if (!joints.ContainsKey(joints[next])) return false;
            next = joints[next];
            list.Remove(next);
        }
        return list.Count == 1 && list[0] == finishPowerLine;
    }
}