using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public Bullet bulletPrefab;
    public float strength;
    public float cooldownTime;

    private Timer _cooldownTimer;
    private Camera _cam;

    private InputAction _fireAction;

    private void Awake()
    {
        _fireAction = InputSystem.actions.FindAction("Attack");
        _fireAction.Enable();

        _cooldownTimer = this.AddComponent<Timer>();
        _cooldownTimer.duration = cooldownTime;
        _cooldownTimer.ResetFunc = _fireAction.IsPressed;
        _cooldownTimer.allowResetBeforeTimeout = false;

        _cam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_fireAction.IsPressed() && _cooldownTimer.TimedOut()) Fire();
    }

    private void Fire()
    {
        var bullet = Instantiate(bulletPrefab, transform);
        var mousePos = (Vector2)_cam.ScreenToWorldPoint(Mouse.current.position.value);
        bullet.Throw(mousePos - (Vector2)transform.position, strength);
    }
}