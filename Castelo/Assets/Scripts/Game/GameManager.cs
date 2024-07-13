using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Enemies.Target;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;


namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public enum EnemyFocusType
        {
            Nexus,
            Building,
            DefenseTower,
            Player
        }
        
        public static GameManager gameManager;
        private Dictionary<EnemyFocusType, List<EnemyTargetable>> _targetDictionary;
        private List<EnemyManager> _allEnemies = new List<EnemyManager>();

        private void Start()
        {
            if (gameManager != null)
            {
                Destroy(this);
            }

            gameManager = this;
            InitBuildDictionary();
        }

        public void AddBuilding(Building.Building.BuildingType type, EnemyTargetable target)
        {
            switch (type)
            {
                case Building.Building.BuildingType.SimpleBuilding:
                    AddBuildingDictionary(EnemyFocusType.Building, target);
                    break;
                case Building.Building.BuildingType.DefenseTower:
                    AddBuildingDictionary(EnemyFocusType.DefenseTower, target);
                    break;
                case Building.Building.BuildingType.Nexus:
                    AddBuildingDictionary(EnemyFocusType.Nexus, target);
                    break;
                default:
                    Debug.Log("Add building error not maching type");
                    break;
            }
        }

        public void RemoveBuilding(Building.Building.BuildingType type, EnemyTargetable target)
        {
            switch (type)
            {
                case Building.Building.BuildingType.SimpleBuilding:
                    RemoveBuildingDictionary(EnemyFocusType.Building, target);
                    break;
                case Building.Building.BuildingType.DefenseTower:
                    RemoveBuildingDictionary(EnemyFocusType.DefenseTower, target);
                    break;
                case Building.Building.BuildingType.Nexus:
                    RemoveBuildingDictionary(EnemyFocusType.Nexus, target);
                    break;
                default:
                    Debug.Log("remove building error not maching type");
                    break;
            }
        }

        public void AddPlayer(EnemyTargetable target)
        {
            if (!_targetDictionary[EnemyFocusType.Player].Contains(target))
            {
                _targetDictionary[EnemyFocusType.Player].Add(target);
            }
            else
            {
                Debug.Log("bugg in add player to dictionary");
            }
        }

        private void AddBuildingDictionary(EnemyFocusType buildingType, EnemyTargetable target)
        {
            if (!_targetDictionary[buildingType].Contains(target) && 
                !_targetDictionary[EnemyFocusType.Building].Contains(target))
            {
                _targetDictionary[buildingType].Add(target);
                _targetDictionary[EnemyFocusType.Building].Add(target);
            }
            else
            {
                Debug.Log("bugg in add building to dictionary");
            }
        }
        
        private void RemoveBuildingDictionary(EnemyFocusType buildingType, EnemyTargetable target)
        {
            if (_targetDictionary[buildingType].Contains(target) && 
                _targetDictionary[EnemyFocusType.Building].Contains(target))
            {
                _targetDictionary[buildingType].Remove(target);
                _targetDictionary[EnemyFocusType.Building].Remove(target);
            }
            else
            {
                Debug.Log("bugg in remove building to dictionary");
            }
        }

        private void InitBuildDictionary()
        {
            _targetDictionary = new Dictionary<EnemyFocusType, List<EnemyTargetable>>();
            _targetDictionary.Add(EnemyFocusType.Building, new List<EnemyTargetable>());
            _targetDictionary.Add(EnemyFocusType.Nexus, new List<EnemyTargetable>());
            _targetDictionary.Add(EnemyFocusType.DefenseTower, new List<EnemyTargetable>());
            _targetDictionary.Add(EnemyFocusType.Player, new List<EnemyTargetable>());
        }

        public EnemyTargetable GetNearestTarget(EnemyManager enemy, EnemyTarget targets)
        { 
            UnityEngine.Vector3 pos = enemy.transform.position;
            EnemyTargetable newTarget = null;
            float nearestDistance = Mathf.Infinity;
            float distance;
            bool persistant = false;

            foreach (EnemyTargetValue targetValue in targets.enemyTargetPriority)
            {
                foreach (EnemyTargetable target in _targetDictionary[targetValue.enemyFocusType])
                {
                    distance = UnityEngine.Vector3.Distance(pos, target.transform.position);
                    if (targetValue.hasInfiniteRange || distance < targetValue.rangeDetection)
                    {
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            newTarget = target;
                            persistant = targetValue.isPersistant;
                        }
                    }
                }

                if (newTarget != null)
                {
                    break;
                }
            }

            enemy.targetPersistance = persistant;
            return newTarget;
        }

        public void AddEnemy(EnemyManager enemy)
        {
            _allEnemies.Add(enemy);
            Debug.Log("enemy added");
        }

        public void RemoveEnemy(EnemyManager enemy)
        {
            _allEnemies.Remove(enemy);
            Debug.Log("enemy removed");
        }

        public EnemyManager FindNearestEnemy(UnityEngine.Vector3 pos)
        {
            EnemyManager nearestEnemy = null;
            float nearestDistance = Mathf.Infinity;
            float distance;
            
            foreach (EnemyManager enemy in _allEnemies)
            {
                distance = UnityEngine.Vector3.Distance(pos, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }
    }
}
