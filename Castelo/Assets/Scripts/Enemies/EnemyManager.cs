using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Nexus;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyMouvement))]
    public class EnemyManager : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        public EnemyStat enemyStat;
        public EnemyState state;

        private float _maxHealth;
        public float health;
        public float speed;
        public float acceleration;
        public float nexusAttackRange;
        public float buildingDetectionRange;
        public float attackRange;
        public float attackRate;
        public float attackDamage;
        public EnemyTarget enemyTarget;
        public bool targetPersistance = false;

        public bool TargetPersistance
        {
            get => targetPersistance;
            set => targetPersistance = value;
        }

        [SerializeField]private float _attackColdown = 0;
        private GameManager _gameManager;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        public Transform target;
        
        public enum EnemyState
        {
            ChasseNexus,
            TargetNexus,
            TargetPlayer,
            TargetBuilding
        }
        
        void Start()
        {
            _maxHealth = enemyStat.health;
            health = _maxHealth;
            speed = enemyStat.speed;
            acceleration = enemyStat.acceleration;
            nexusAttackRange = enemyStat.nexusAttackRange;
            attackRange = enemyStat.attackRange;
            attackRate = enemyStat.attackRate;
            attackDamage = enemyStat.attackDamage;
            state = EnemyState.ChasseNexus;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _gameManager = GameManager.gameManager;
        }

        private void FixedUpdate()
        {
            FindTarget();
            if (state == EnemyState.TargetNexus)
            {
                if (_attackColdown < 0)
                {
                    _attackColdown = attackRate;
                    NexusManager.nexusManager.TakeDamage(attackDamage);
                }
                else
                {
                    _attackColdown -= Time.deltaTime;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
        }

        private void FindTarget()
        {
            Transform transform = _gameManager.GetNearestTarget(this, enemyTarget).transform;
            if (transform != null)
            {
                target = transform;
                _navMeshAgent.destination = target.position;
            }
        }
    }
}
