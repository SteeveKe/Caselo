using System;
using Enemies;
using UnityEngine;

namespace Game.Building
{
    public class DefenseTower : Building
    {
        public float attackRange;
        public float attackDamage;
        public float attackRate;
        public EnemyManager target = null;

        private float _attackColdown = 0;

        protected override void Init()
        {
            buildingType = BuildingType.DefenseTower;
            base.Init();
        }

        private void FixedUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (_attackColdown < 0)
            {
                _attackColdown = attackRate;
                AttackEnemy();
            }
            else
            {
                _attackColdown -= Time.deltaTime;
            }
        }

        protected void AttackEnemy()
        {
            target.TakeDamage(attackDamage);
        }
    }
}
