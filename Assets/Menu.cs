using UnityEngine;

public class Menu : MonoBehaviour
{

    public void StartPlay()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartPlay();
    }

    public void DontPlay()
    {
        Application.Quit();
    }
}
