using System;
using System.Collections.Generic;
using DG.Tweening;
using Enemies;
using Unity.Loading;
using UnityEngine;

namespace Game.Nexus
{
    public class NexusManager : Building.Building
    {
        public static NexusManager nexusManager;

        protected override void Init()
        {
            if (nexusManager != null)
            {
                Destroy(this);
            }
            nexusManager = this;
            base.Init();
        }

        private void FixedUpdate()
        {
            //FindEnemies();
            
            if (life <= 0)
            {
                Debug.Log("Game Over");
            }
        }

        /*
        private void FindEnemies()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 100f, 1 << 7);

            foreach (var col in colliders)
            {
                EnemyManager enemy = col.GetComponent<EnemyManager>();
                if (enemy == null)
                {
                    continue;
                }
                if (enemy.state == EnemyManager.EnemyState.ChasseNexus)
                {
                    if (Mathf.Abs((transform.position - enemy.transform.position).magnitude) < enemy.nexusAttackRange)
                    {
                        enemy.state = EnemyManager.EnemyState.TargetNexus;
                        enemy.NavMeshAgent.isStopped = true;
                    }
                }
            }
        } 
        */
    }
}
