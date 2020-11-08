using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Cable,
        Weapon,
        Equipment
    }

    public Type type;

    private Collider2D col;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void EnableCollider(bool enable)
    {
        col.enabled = enable;
    }

    public void Push(Vector2 force)
    {
        rb.AddForce(force * 30);
        Invoke(nameof(EnableCollider), 0.2f);
    }

    private void EnableCollider()
    {
        col.enabled = true;
    }
    
}