using System;
using DG.Tweening;
using Enemies.Target;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Building
{
    [RequireComponent(typeof(EnemyBuildingTarget))]
    public abstract class Building : MonoBehaviour
    {
        public enum BuildingType
        {
            Nexus,
            SimpleBuilding,
            DefenseTower
        }

        protected EnemyTargetable _enemyTargetable;
        protected GameManager _gameManager;
        public BuildingType buildingType;
        public float initLife = 100;
        [SerializeField]protected float life;

        private void Start()
        {
            Init();
        }

        private void FixedUpdate()
        {
            if (life <= 0)
            {
                _gameManager.RemoveBuilding(buildingType, _enemyTargetable);
                transform.DOComplete();
                Destroy(gameObject);
            }
            TestTarget();
        }

        protected virtual void TestTarget()
        {
            
        }

        protected virtual void Init()
        {
            _enemyTargetable = GetComponent<EnemyTargetable>();
            _gameManager = GameManager.gameManager;
            life = initLife;
            //_gameManager.AddBuilding(buildingType, this);
        }

        protected void DamageAnimation()
        {
            transform.DOComplete();
            transform.DOShakePosition(0.5f, 0.1f);
            transform.DOShakeScale(0.5f, 0.1f);
            transform.DOShakeRotation(0.5f, 10f);
        }
        
        public void TakeDamage(float dmg)
        {
            DamageAnimation();
            life -= dmg;
        }
    }
}
