using System;
using System.Collections.Generic;
using DG.Tweening;
using Enemies;
using Unity.Loading;
using UnityEngine;

namespace Game.Nexus
{
    public class NexusManager : MonoBehaviour
    {
        public static NexusManager nexusManager;
        [SerializeField] private float initLife;
        [SerializeField] private float life;
        [SerializeField] private List<Collider> enemiesList = new List<Collider>();

        public float distance;

        private void Awake()
        {
            if (nexusManager != null)
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            nexusManager = this;
            life = initLife;
        }

        private void FixedUpdate()
        {
            FindEnemies();
            
            if (life <= 0)
            {
                Debug.Log("Game Over");
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                TakeDamage(1);
            }
        }

        public void TakeDamage(float dmg)
        {
            life -= dmg;
            transform.DOComplete();
            transform.DOShakePosition(0.5f, 0.1f);
            transform.DOShakeScale(0.5f, 0.1f);
            transform.DOShakeRotation(0.5f, 10f);
        }

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
                    if (Mathf.Abs((transform.position - enemy.transform.position).magnitude) < enemy.enemyStat.nexusAttackRange)
                    {
                        enemy.state = EnemyManager.EnemyState.TargetNexus;
                        enemy.NavMeshAgent.isStopped = true;
                    }
                }
            }
        } 
    }
}
