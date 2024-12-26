using System;
using Enemies;
using UnityEngine;

namespace Game.Building
{
    public class DefenseTower : Building
    {
        public float pivotSpeed;
        public Transform partToPivot;
        public Vector3 targetRotation;

        public Transform partToFace;
        public Vector2 faceClamp;
        
        public float attackRange;
        public float attackDamage;
        public float attackRate;
        public EnemyManager target = null;

        protected float _attackColdown = 0;

        protected override void Init()
        {
            buildingType = BuildingType.DefenseTower;
            base.Init();
        }

        protected override void TestTarget()
        {
            SetTarget();
            if (!target)
            {
                return;
            }
            
            TurnOnTarget();

            if (_attackColdown < 0)
            {
                _attackColdown = attackRate;
                AttackEnemy();
            }
            else
            {
                _attackColdown -= Time.deltaTime;
            }
            base.TestTarget();
        }
         protected virtual void TurnOnTarget()
        {
            Vector3 dir = target.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            targetRotation = Quaternion.Lerp(partToPivot.rotation, lookRotation, Time.deltaTime * pivotSpeed).eulerAngles;
            partToPivot.rotation = Quaternion.Euler(0f, targetRotation.y, 0f);
        }

        protected void AttackEnemy()
        {
            target.TakeDamage(attackDamage);
        }

        protected void SetTarget()
        {
            if (target)
            {
                if (Vector3.Distance(target.transform.position, transform.position) < attackRange)
                {
                    return;
                }
            }
            
            EnemyManager enemy = _gameManager.FindNearestEnemy(transform.position);

            if (Vector3.Distance(enemy.transform.position, transform.position) < attackRange)
            {
                target = enemy;
            }
            else
            {
                target = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
