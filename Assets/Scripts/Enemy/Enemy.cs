using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Enemy : Character
    {
        public enum State
        {
            Sleep,
            Normal,
            AttackCharge,
            Attack,
            AttackRestore
        }

        public State state;

        public float attackTimeout = 1f;
        private float lastAttackTime = 0;

        protected bool inAttackRange;

        protected Player player;
        protected Transform playerTransform;

        #region Inspector links
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected RangeHandler attackRangeTrigger;
        [SerializeField] protected RangeHandler disturbedTrigger;
        [SerializeField] protected RangeHandler playerLostTrigger;
        [SerializeField] protected RangeHandler hitRange;
        #endregion

        #region Animator params
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int Sleep = Animator.StringToHash("sleep");
        private static readonly int Power = Animator.StringToHash("power");
        private static readonly int Attack = Animator.StringToHash("attack");
        #endregion

        protected bool canAttack => power > 0 && !overloaded && inAttackRange 
                                    && state == State.Normal
                                    && lastAttackTime + attackTimeout <= Time.time;

        protected bool playerLost;
    
        public float overloadTime = 4f;
        
        protected virtual void Start()
        {
            player = FindObjectOfType<Player>();
            playerTransform = player.transform;
            
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            
            SetSleepState();
            
            attackRangeTrigger.onEnter.AddListener(AttackRangeEnterHandler);
            attackRangeTrigger.onExit.AddListener(AttackRangeLeaveHandler);
            disturbedTrigger.onEnter.AddListener(DisturbedHandler);
            playerLostTrigger.onEnter.AddListener(PlayerFoundTrigger);
            playerLostTrigger.onExit.AddListener(PlayerLostTrigger);
            
            hitRange.onEnter.AddListener(HitRangeHandler);
            hitRange.Enable(false);
        }

        protected virtual void Update()
        {
            if (canAttack)
            {
                StartAttackHandler();
                return;
            }

            if (state == State.Normal)
            {
                if (!inAttackRange) SetDestination(playerTransform.position);
                else agent.ResetPath();
                return;
            }
        }

        protected void StopAgent()
        {
            agent.ResetPath();
            agent.enabled = false;
        }

        protected void StartAgent()
        {
            agent.enabled = true;
        }


        public virtual void StartAttackHandler()
        {
            if (!canAttack) return;
            state = State.AttackCharge;
            animator.SetTrigger(Attack);
        }

        public virtual void HitHandler()
        {
            StartAgent();
            lastAttackTime = Time.time;
            animator.SetTrigger(Hit);
            if (state == State.AttackCharge) SetNormalState();
            IncreasePower();
        }

        protected void SetDestination(Vector3 dest)
        {
            if (overloaded || playerLost || state != State.Normal || !agent.enabled) return;
            agent.SetDestination(dest);
        }

        #region Set state handlers
        public virtual void SetSleepState()
        {
            state = State.Sleep;
            animator.SetBool(Sleep, true);
        }

        public virtual void SetNormalState()
        {
            state = State.Normal;
            StartAgent();
            StartAttackHandler();
        }

        public override void SetOverloadState()
        {
            base.SetOverloadState();
            DelayCall(() =>
            {
                overloaded = false;
                if (power < 1) IncreasePower();
                SetNormalState();
            }, overloadTime);
        }
        #endregion

        #region Range trigger handlers
        public virtual void PlayerFoundTrigger(Collider2D other)
        {
            if (!other.GetComponent<Player>()) return;
            playerLost = false;
        }
        
        public virtual void PlayerLostTrigger(Collider2D other)
        {
            if (!other.GetComponent<Player>()) return;
            playerLost = true;
        }

        protected virtual void AttackRangeEnterHandler(Collider2D other)
        {
            if (!other.GetComponent<Player>()) return;
            inAttackRange = true;
            StartAttackHandler();
        }

        protected virtual void AttackRangeLeaveHandler(Collider2D other)
        {
            if (!other.GetComponent<Player>()) return;
            inAttackRange = false;
        }

        protected virtual void DisturbedHandler(Collider2D other)
        {
            if (state != State.Sleep) return;
            if (!other.GetComponent<Player>()) return;
            animator.SetBool(Sleep, false);
        }

        protected virtual void HitRangeHandler(Collider2D other)
        {
            var playerObject = other.GetComponent<Player>();
            if (!playerObject) return;
            if (playerObject.HitHandler(attackPower))
            {
                hitRange.Enable(false);
                DecreasePower();
            }
        }
        #endregion

        #region Animation handlers
        public virtual void AttackChargeStart() { }

        public virtual void AttackChargeEnd() { }

        public virtual void AttackStart()
        {
            state = State.Attack;
        }

        public virtual void AttackEnd()
        {
            lastAttackTime = Time.time;
        }

        public virtual void AttackRestoreStart()
        {
            state = State.AttackRestore;
        }

        public virtual void AttackRestoreEnd()
        {
            SetNormalState();
        }

        public void WakeUp()
        {
            SetNormalState();
        }

        #endregion

        #region Helpers
        protected void DelayCall(Action callback, float time)
        {
            if (stateChangeCoroutine != null) StopCoroutine(stateChangeCoroutine);
            stateChangeCoroutine = StartCoroutine(DelayCallCoroutine(callback, time));
        }

        private IEnumerator DelayCallCoroutine(Action callback, float time)
        {
            yield return new WaitForSeconds(time);
            callback();
            stateChangeCoroutine = null;
        }
        
        public static void DebugDrawPath(Vector3[] corners)
        {
            if (corners.Length < 2) { return; }
            int i = 0;
            for (; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.blue);
            }
            Debug.DrawLine(corners[0], corners[1], Color.red);
        }
        #endregion
    }
}