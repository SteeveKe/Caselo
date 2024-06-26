using Game.Building;
using UnityEngine;

namespace Enemies.Target
{
    [RequireComponent(typeof(Building))]
    public class EnemyBuildingTarget : EnemyTargetable
    {
        private Building _building;
        
        protected override void Init()
        {
            _building = GetComponent<Building>();
            base.Init();
            GameManager.AddBuilding(_building.buildingType, this);
        }
    }
}
