using System;
using System.Collections;
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

        [SerializeField]private float _attackColdown = 0;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        
        public enum EnemyState
        {
            ChasseNexus,
            TargetNexus,
            TargetPlayer,
            TargetBuilding
        }
        
        void Start()
        {
            state = EnemyState.ChasseNexus;
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void FixedUpdate()
        {
            if (state == EnemyState.TargetNexus)
            {
                if (_attackColdown < 0)
                {
                    _attackColdown = enemyStat.attackRate;
                    NexusManager.nexusManager.TakeDamage(enemyStat.attackDamage);
                }
                else
                {
                    _attackColdown -= Time.deltaTime;
                }
            }
        }
    }
}
