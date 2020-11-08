using UnityEngine;

namespace Enemy
{
    public class Gull : Enemy
    {
        public float attackSpeed = 10;
        
        private Vector3 attackTargetPosition;
        private Rigidbody2D rb;
        private Collider2D col;
        private Vector2 attackForce;

        protected override void Start()
        {
            base.Start();
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            col.isTrigger = true;
        }

        protected override void Update()
        {
            if (state == State.Attack) return;
            base.Update();
        }

        public override void AttackChargeStart()
        {
            base.AttackChargeStart();
            attackForce = (playerTransform.position - transform.position).normalized * attackSpeed;
        }

        public override void AttackStart()
        {
            base.AttackStart();
            StopAgent();
            col.isTrigger = false;
            hitRange.Enable(true);
            rb.AddForce(attackForce);
        }

        public override void AttackEnd()
        {
            base.AttackEnd();
            col.isTrigger = true;
            rb.velocity = Vector2.zero;
            hitRange.Enable(false);
        }

        protected override void HitRangeHandler(Collider2D other)
        {
            var playerObject = other.GetComponent<Player>();
            if (!playerObject) return;
            if (playerObject.HitHandler(attackPower))
            {
                hitRange.Enable(false);
                playerObject.DropItem();
                DecreasePower();
            }
        }
    }
}