using System;
using UnityEngine;
using UnityEngine.Events;

public class RangeHandler : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Collider2D> onEnter = new UnityEvent<Collider2D>();
    [HideInInspector] public UnityEvent<Collider2D> onExit = new UnityEvent<Collider2D>();

    private Collider2D selfCollider;
    
    private void Awake()
    {
        selfCollider = GetComponent<Collider2D>();
    }

    public bool IsInRange(Component other)
    {
        return selfCollider.IsTouching(other.GetComponent<Collider2D>());
    }

    public bool IsInRange(Collider2D other)
    {
        return selfCollider.IsTouching(other);
    }
    
    public void Enable(bool value)
    {
        selfCollider.enabled = value;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        onEnter?.Invoke(other);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        onExit?.Invoke(other);
    }
}