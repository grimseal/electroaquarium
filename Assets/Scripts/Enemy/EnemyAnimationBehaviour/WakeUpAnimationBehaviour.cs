using UnityEngine;

namespace Enemy.EnemyAnimationBehaviour
{
    public class WakeUpAnimationBehaviour : AnimationBehaviour
    {

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.WakeUp();
        }
    }
}