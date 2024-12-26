using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "New Target", menuName = "EnemyTarget")]
    public class EnemyTarget : UnityEngine.ScriptableObject
    {
        public List<EnemyTargetValue> enemyTargetPriority = new List<EnemyTargetValue>();
    }
}
