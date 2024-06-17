using System;
using Game.Nexus;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    [RequireComponent(typeof(EnemyManager))]
    public class EnemyMouvement : MonoBehaviour
    {
        private EnemyManager _enemyManager;
        private NavMeshAgent _navMeshAgent;
        [SerializeField] private Transform destination;

        private void Start()
        {
            _enemyManager = GetComponent<EnemyManager>();
            _navMeshAgent = _enemyManager.NavMeshAgent;
            destination = NexusManager.nexusManager.transform;
            _navMeshAgent.SetDestination(destination.position);
        }
    }
}
