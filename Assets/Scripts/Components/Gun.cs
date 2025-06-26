using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : NetworkBehaviour
{
    public Bullet bulletPrefab;
    public float strength;
    public float cooldownTime;

    private Timer _cooldownTimer;
    private Camera _cam;
    private Ownership _parentTeam;
    private InputAction _fireAction;

    private bool _clientFirePressed;
    private Vector2 _clientMousePos;


    // Update is called once per frame

    public void Start()
    {
        _parentTeam = GetComponentInParent<Ownership>();
    }

    private void Update()
    {
        if (isLocalPlayer)
            CmdUpdateInput(
                _fireAction.IsPressed(),
                _cam.ScreenToWorldPoint(Mouse.current.position.value)
            );
        if (isServer)
            if (_clientFirePressed && _cooldownTimer.TimedOut())
                Fire();
    }

    [Server]
    public override void OnStartServer()
    {
        _cooldownTimer = this.AddComponent<Timer>();
        _cooldownTimer.duration = cooldownTime;
        _cooldownTimer.allowResetBeforeTimeout = false;
    }

    [Client]
    public override void OnStartLocalPlayer()
    {
        _fireAction = InputSystem.actions.FindAction("Attack");
        _fireAction.Enable();

        _cam = Camera.main;
    }

    [Server]
    private void Fire()
    {
        _cooldownTimer.Restart();
        var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

        if (_parentTeam) bullet.SetTeam(_parentTeam);

        NetworkServer.Spawn(bullet.gameObject);
        bullet.Throw(_clientMousePos - (Vector2)transform.position, strength);
    }

    [Command]
    private void CmdUpdateInput(bool fire, Vector2 mousePos)
    {
        _clientFirePressed = fire;
        _clientMousePos = mousePos;
    }
}