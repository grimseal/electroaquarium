using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : Singleton<GameManager>
{
    private PowerLine[] powerLines;
    private Dictionary<PowerLine, PowerLine> joints;
    public bool allConnected;
    private PlayerHealthBar playerHealthBar;
    private Player player;
    private Timer timer;
    private GameOver gameOver;

    public AudioSource menuMusic;
    public AudioSource gameMusic;

    public void Start()
    {
        playerHealthBar = FindObjectOfType<PlayerHealthBar>();
        player = FindObjectOfType<Player>();
        powerLines = FindObjectsOfType<PowerLine>();
        joints = new Dictionary<PowerLine, PowerLine>();
        timer = FindObjectOfType<Timer>();
        gameOver = FindObjectOfType<GameOver>();
        SetPlayerHealth(player.power, player.powerMax);
        menuMusic.Play();
        gameMusic.Stop();
    }

    public void PowerLineJoined(PowerLine a, PowerLine b)
    {
        a.Join(b);
        joints[a] = b;
        if (CheckIfAllConnected())
        {
            allConnected = true;
            GameOver();
        }
    }

    private bool CheckIfAllConnected()
    {
        return powerLines.All(p => p.powered);
    }

    public void GameOver()
    {
        if (!player.startListen) return;
        
        gameMusic.Stop();
        menuMusic.Play();
        
        player.startListen = false;
        timer.StopCountdown();
        gameOver.Enable();
        var enemies = FindObjectsOfType<Enemy.Enemy>();
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<NavMeshAgent>().enabled = false;
            enemy.enabled = false;
        }
    }

    public void StartPlay()
    {
        gameMusic.Play();
        menuMusic.Stop();
        player.startListen = true;
        timer.StartCountdown();
        SetPlayerHealth(player.power, player.powerMax);
    }

    public void SetPlayerHealth(int value, int max)
    {
        playerHealthBar.SetValue(value);
        playerHealthBar.SetMax(max);
    }
}