using UnityEditor;
using UnityEngine;


    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
    public class EnemyStat : UnityEngine.ScriptableObject
    {
        public float health;
        public float speed;
        public float acceleration;
        public float nexusAttackRange;
        public float buildingDetectionRange;
        public float attackRange;
        public float attackRate;
        public float attackDamage;
    }
