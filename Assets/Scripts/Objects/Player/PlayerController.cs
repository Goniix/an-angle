using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(PlayerBehavior))]
public class PlayerController : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner) gameObject.GetComponent<PlayerBehavior>().enabled = false;
    }
}