using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Ownership))]
public class Bullet : NetworkBehaviour
{
    private Rigidbody2D _rb;
    private Ownership _ownership;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ownership = GetComponent<Ownership>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isServer) return;

        var otherTeam = other.gameObject.GetComponent<Ownership>();
        if (!otherTeam || (otherTeam && !otherTeam.AreInSameTeam(_ownership))) NetworkServer.Destroy(gameObject);
    }

    [Server]
    public void SetTeam(Ownership team)
    {
        _ownership.ownerId = team.ownerId;
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