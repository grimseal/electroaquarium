using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyBehavior", menuName = "EnemyBehaviour", order = 51)]
public class EnemyBehaviour : ScriptableObject
{
    public bool attackDistance;
    // public bool  

    public virtual bool CanAttackPlayer()
    {
        return false;
    }

    public virtual void EnergyHandler(bool power)
    {
        
    }

}
