using UnityEngine;

namespace Enemy
{
    public class Crab : Enemy
    {
        protected Vector3 currentDestination;

        protected override void Update()
        {
            if (canAttack)
            {
                StartAttackHandler();
                return;
            }

            if (state == State.Normal)
            {
                if (!inAttackRange)
                {
                    if (agent.hasPath && Vector2.Distance(agent.pathEndPosition, transform.position) <= 0.01f)
                        agent.ResetPath();
                    if (!agent.hasPath) CalcDestination();
                    SetDestination(currentDestination);
                }
                else agent.ResetPath();
            }
        }

        public override void AttackChargeStart()
        {
            base.AttackChargeStart();
            agent.ResetPath();
        }

        private void CalcDestination()
        {
            if (playerLost) return;
            var playerPosition = player.transform.position;
            var crabPosition = transform.position;
            var v = playerPosition - crabPosition;
            var m = v / 2 + crabPosition;
            var r = Vector2.Distance(playerPosition, crabPosition);
            var p = new Vector2(-v.y, v.x) / Mathf.Sqrt(v.x * v.x + v.y * v.y);
            currentDestination = p * Random.Range(-r, r) + new Vector2(m.x, m.y);
        }

        public override void AttackStart()
        {
            base.AttackStart();
            hitRange.Enable(true);
        }

        public override void AttackEnd()
        {
            base.AttackEnd();
            hitRange.Enable(false);
        }

        public override void PlayerLostTrigger(Collider2D other)
        {
            base.PlayerLostTrigger(other);
        }
    }
}