using System;
using System.Linq;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TimerValue[] minutes;
    public TimerValue[] seconds;

    public float startTime;
    public bool isStarted = false;
    public float countdownTime = 121f;

    public void StartCountdown()
    {
        isStarted = true;
        startTime = Time.time;
    }

    public void StopCountdown()
    {
        isStarted = false;
    }

    private void Start()
    {
        var t = countdownTime;
        var m = Mathf.FloorToInt(t / 60);
        var s = Mathf.FloorToInt(t - m * 60);
        Apply(m, minutes);
        Apply(s, seconds);
    }
    
    private void Update()
    {
        if (!isStarted) return;
        var t = startTime + countdownTime - Time.time;
        if (t <= 0)
        {
            isStarted = false;
            Apply(0, minutes);
            Apply(0, seconds);
            GameManager.Instance.GameOver();
            return;
        }
        
        var m = Mathf.FloorToInt(t / 60);
        var s = Mathf.FloorToInt(t - m * 60);
        Apply(m, minutes);
        Apply(s, seconds);
    }

    private void Apply(int value, TimerValue[] values)
    {
        var str = value.ToString();
        if (str.Length < 2) str = "0" + str;
        if (str.Length < 2) str = "0" + str;
        var arr = str.ToCharArray().Select(c =>
            {
                var st = c.ToString();
                return int.TryParse(st, out var result) ? result : 0;
            }
        ).ToArray();
        for (var i = 0; i < values.Length; i++) values[i].SetValue(arr[i]);
    }
}