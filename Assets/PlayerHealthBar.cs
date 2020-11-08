using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private float chargeWidth;
    [SerializeField] private GameObject[] charges;
    [SerializeField] private RectTransform bar;

    public void SetMax(int max)
    {
        bar.sizeDelta = new Vector2(max * chargeWidth, 340);
    }

    public void SetValue(int val)
    {
        foreach (var charge in charges) charge.SetActive(false);
        for (var i = 0; i < val; i++) charges[i].SetActive(true);
    }
    
}
