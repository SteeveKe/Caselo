using System;
using Game;
using UnityEngine;

namespace Enemies.Target
{
    public abstract class EnemyTargetable : MonoBehaviour
    {
        private Transform _transform;
        protected GameManager GameManager;
        public float targetRadius = 5f;

        private void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            _transform = transform;
            GameManager = GameManager.gameManager;
        }

        public abstract void TakeDamage(float damage);
    }
}
