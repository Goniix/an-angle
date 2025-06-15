using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : NetworkBehaviour
{
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isServer) return;
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    public void Throw(Vector2 direction, float strength)
    {
        _rb.AddForce(direction.normalized * strength, ForceMode2D.Impulse);
        RpcInitSyncRigibody(_rb.linearVelocity);
    }

    [ClientRpc]
    private void RpcInitSyncRigibody(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }
}