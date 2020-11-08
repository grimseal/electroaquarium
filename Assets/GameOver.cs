using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Image[] images;

    public void Enable()
    {
        foreach (var image in images) image.enabled = true;
    }
}
