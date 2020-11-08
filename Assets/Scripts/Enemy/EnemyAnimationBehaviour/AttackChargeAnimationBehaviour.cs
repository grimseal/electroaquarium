using UnityEngine;

namespace Enemy.EnemyAnimationBehaviour
{
    public class AttackChargeAnimationBehaviour : AnimationBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            enemy.AttackChargeStart();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.AttackChargeEnd();
        }
    }
}