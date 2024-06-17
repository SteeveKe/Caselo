using UnityEditor;
using UnityEngine;


    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
    public class EnemyStat : UnityEngine.ScriptableObject
    {
        public float life;
        public float speed;
        public float acceleration;
        public float nexusAttackRange;
        public float attackRange;
        public float attackRate;
        public float attackDamage;
    }
