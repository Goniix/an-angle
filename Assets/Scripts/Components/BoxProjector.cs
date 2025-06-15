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
        // ReSharper disable once Unity.PreferNonAllocApi
        var collides = Physics2D.BoxCastAll(transform.position, size, 0, -transform.up, offset, mask);

        //from end because collides is sorted by distance from transform.position
        for (var i = collides.Length - 1; i >= 0; i--)
            if (collides[i].transform != transform)
                return collides[i];

        return false;
    }
}