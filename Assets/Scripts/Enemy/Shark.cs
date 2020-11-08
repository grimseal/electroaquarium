using UnityEngine;

namespace Enemy
{
    public class Shark : Enemy
    {
        private Vector3 attackTargetVector;
        public float pushForce = 20;
        
        public override void AttackChargeStart()
        {
            base.AttackChargeStart();
            StopAgent();
        }

        public override void AttackStart()
        { 
            base.AttackStart();
            attackTargetVector = playerTransform.position;
        }

        public override void AttackEnd()
        {
            base.AttackEnd();
            hitRange.Enable(false);
        }
        
        protected override void HitRangeHandler(Collider2D other)
        {
            var playerObject = other.GetComponent<Player>();
            if (!playerObject) return;
            if (playerObject.HitHandler(attackPower))
            {
                playerObject.Push(this, pushForce);
                hitRange.Enable(false);
                DecreasePower();
            }
        }

        #region Animation event handlers
        public void SharkAttackMove()
        {
            transform.position = attackTargetVector;
        }
        
        public void SharkAttackStart()
        {
            hitRange.Enable(true);
        }
        #endregion
        
    }
}