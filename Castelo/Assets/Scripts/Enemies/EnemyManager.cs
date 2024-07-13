using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.Target;
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
        
        public EnemyTargetable enemyTargetable;
        
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
            attackRange = enemyStat.attackRange;
            attackRate = enemyStat.attackRate;
            attackDamage = enemyStat.attackDamage;
            
            state = EnemyState.ChasseNexus;
            
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _gameManager = GameManager.gameManager;
            _attackColdown = attackRate;
            
            _gameManager.AddEnemy(this);
        }

        private void FixedUpdate()
        {

            if (health <= 0)
            {
                Destroy(gameObject);
                return;
            }
            
            if (!targetPersistance || !enemyTargetable)
            {
                FindTarget();
            }
            
            float dist = Vector3.Distance(_navMeshAgent.pathEndPosition, transform.position);
            if (dist < NavMeshAgent.stoppingDistance)
            {
                if (_attackColdown <= 0)
                {
                    if (enemyTargetable)
                    {
                        _attackColdown = attackRate;
                        enemyTargetable.TakeDamage(attackDamage);
                    }
                }
                else
                {
                    _attackColdown -= Time.deltaTime;
                }
            }
            else
            {
                _attackColdown -= Time.deltaTime;
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
        }

        private void FindTarget()
        {
            EnemyTargetable nearestTarget = _gameManager.GetNearestTarget(this, enemyTarget);
            if (nearestTarget && (nearestTarget != enemyTargetable || nearestTarget.GetType() == typeof(EnemyPlayerTarget)))
            {
                _navMeshAgent.destination = nearestTarget.transform.position;
                _navMeshAgent.stoppingDistance = nearestTarget.targetRadius + attackRange;
                enemyTargetable = nearestTarget;
            }
        }

        private void OnDestroy()
        {
            _gameManager.RemoveEnemy(this);
        }
    }
}
