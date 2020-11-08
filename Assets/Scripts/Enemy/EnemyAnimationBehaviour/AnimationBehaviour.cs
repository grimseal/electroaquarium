using UnityEngine;

namespace Enemy.EnemyAnimationBehaviour
{
    public class AnimationBehaviour : StateMachineBehaviour
    {
        protected Enemy enemy;
    
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy = enemy != null ? enemy : animator.GetComponent<Enemy>();
        }
    }
}
