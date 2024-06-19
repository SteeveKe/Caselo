using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Building
{
    public abstract class Building : MonoBehaviour
    {
        public enum BuildingType
        {
            Nexus,
            SimpleBuilding,
            DefenseTower
        }

        private GameManager _gameManager;
        public BuildingType buildingType;
        public float initLife;
        [SerializeField]protected float life;

        private void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            _gameManager = GameManager.gameManager;
            life = initLife;
            _gameManager.AddBuilding(buildingType, this);
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
