using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float duration;
    public bool allowResetBeforeTimeout = true;
    public Func<bool> ResetFunc = () => false;

    private float _counter;

    // Update is called once per frame
    private void Update()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(double delta)
    {
        var shouldReset = ResetFunc();
        if (_counter > 0.0f)
        {
            _counter -= (float)delta;
            if (allowResetBeforeTimeout && shouldReset) Restart();
        }
        else
        {
            if (shouldReset) Restart();
        }
    }

    public void TimeOut()
    {
        _counter = 0;
    }

    public bool TimedOut()
    {
        return _counter <= 0;
    }

    public void Restart()
    {
        _counter = duration;
    }
}