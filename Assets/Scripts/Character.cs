using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    private static readonly int Power = Animator.StringToHash("power");
    
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject powerIcon;
    [SerializeField] protected GameObject overloadIcon;
    
    public int power;
    public int powerMax = 3;
    public int attackPower = 1;
    
    private bool _powered;
    
    protected Coroutine stateChangeCoroutine;

    public float rechargeTime = 3f;
    
    public bool powered
    {
        get => _powered;
        set
        {
            _powered = value;
            animator.SetFloat(Power, _powered ? 1 : 0);
            powerIcon.SetActive(value);
        }
    }

    protected bool _overloaded;
    public virtual bool overloaded
    {
        get => _overloaded;
        set
        {
            _overloaded = value;
            overloadIcon.SetActive(value);
        }
    }

    protected void IncreasePower(int value = 1)
    {
        power += value;
        if (power == powerMax) SetPowered();
        if (power > powerMax)
        {
            SetOverloadState();
            power = 0;
        }
        if (this is Player) GameManager.Instance.SetPlayerHealth(power, powerMax);
    }

    protected void DecreasePower()
    {
        var startPower = power; 
        power -= 1;
        if (power != powerMax) powered = false;
        if (power < 0) power = 0;
        if (this is Player) GameManager.Instance.SetPlayerHealth(power, powerMax);
        if (startPower == power || power != 0)  return;
        StartCoroutine(RechargeTimeout());
    }
    
    public virtual void SetPowered()
    {
        powered = true;
        overloaded = false;
    }
    
    public virtual void SetOverloadState()
    {
        powered = false;
        overloaded = true;
    }

    protected IEnumerator RechargeTimeout()
    {
        yield return new WaitForSeconds(rechargeTime);
        if (power < powerMax) IncreasePower();
    }
}