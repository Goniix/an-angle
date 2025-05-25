using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(this);
    }

    public void Throw(Vector2 direction, float strength)
    {
        _rb.AddForce(direction.normalized * strength);
    }
}