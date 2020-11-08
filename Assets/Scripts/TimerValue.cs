using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TimerValue : MonoBehaviour
{
    public Sprite[] counts;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetValue(int value)
    {
        image.sprite = counts[value];
    }
}