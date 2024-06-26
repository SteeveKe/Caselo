using System;
using Game;
using UnityEngine;

namespace Enemies
{
    [Serializable]
    public class EnemyTargetValue
    {
        public GameManager.EnemyFocusType enemyFocusType;
        public bool hasInfiniteRange;
        public float rangeDetection;
        public bool isPersistant = true;
    }
}
