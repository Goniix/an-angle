using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float duration;
    public Func<bool> ResetCondition = () => false;

    private float _counter;

    // Update is called once per frame
    private void Update()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(double delta)
    {
        if (ResetCondition()) _counter = duration;
        else if (_counter > 0.0f) _counter -= (float)delta;
    }

    public void TimeOut()
    {
        _counter = 0;
    }

    public bool TimedOut()
    {
        return _counter <= 0;
    }
}