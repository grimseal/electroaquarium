using System.Collections.Generic;
using System.Linq;

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
        return powerLines.All(p => p.powered);
    }
}