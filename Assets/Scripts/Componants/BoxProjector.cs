using UnityEngine;

public class BoxProjector : MonoBehaviour
{
    public Vector2 size;
    public float offset;
    public LayerMask mask;
    public bool drawGizmo;

    private void OnDrawGizmos()
    {
        if (drawGizmo)
            Gizmos.DrawWireCube(transform.position - transform.up * offset, size);
    }

    public bool Collides()
    {
        return Physics2D.BoxCast(transform.position, size, 0, -transform.up, offset, mask);
    }
}