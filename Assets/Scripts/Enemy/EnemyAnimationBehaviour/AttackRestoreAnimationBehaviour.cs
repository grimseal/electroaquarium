using UnityEngine;

namespace Enemy.EnemyAnimationBehaviour
{
    public class AttackRestoreAnimationBehaviour : AnimationBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            enemy.AttackRestoreStart();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.AttackRestoreEnd();
        }
    }
}