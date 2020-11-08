using UnityEngine;

namespace Enemy.EnemyAnimationBehaviour
{
    public class AttackAnimationBehaviour : AnimationBehaviour
    {
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            enemy.AttackStart();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.AttackEnd();
        }
    }
}