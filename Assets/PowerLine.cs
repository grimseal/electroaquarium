using UnityEngine;

public class PowerLine : MonoBehaviour
{
    public float segmentLength = 1f;
    [SerializeField] private LineRenderer wire;

    private Collider2D col;
    private bool hasTarget;
    private Transform target;
    public bool joined { get; private set; }

    private void Start()
    {
        col = GetComponent<Collider2D>();
        Rewind();
    }

    private void Update()
    {
        Stretch();
    }
    
    public void TakeWireBy(Transform tr)
    {
        target = tr;
        hasTarget = true;
        AddPoint(tr.position);
    }

    private void Stretch()
    {
        if (joined || !hasTarget) return;
        if (Vector3.Distance(wire.GetPosition(wire.positionCount - 2), target.position) >= segmentLength)
            AddPoint(target.position);
        wire.SetPosition(wire.positionCount - 1, target.position);
    }

    public void Rewind()
    {
        if (joined) return;
        target = null;
        hasTarget = false;
        wire.positionCount = 1;
        wire.SetPosition(0, transform.position);
    }
    
    public void Join(PowerLine powerLine)
    {
        joined = true;
        this.target = null;
        AddPoint(powerLine.transform.position);
    }

    private void AddPoint(Vector3 position)
    {
        var pos = wire.positionCount; 
        wire.positionCount = pos + 1;
        wire.SetPosition(pos, position);
    }
}
