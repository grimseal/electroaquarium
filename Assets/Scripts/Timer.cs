using System;
using System.Collections;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    public Coroutine DelayCall(Action callback, float time)
    {
        return StartCoroutine(DelayCallCoroutine(callback, time));
    }

    private IEnumerator DelayCallCoroutine(Action callback, float time)
    {
        yield return new WaitForSeconds(Time.time + time);
        callback();
    }
}