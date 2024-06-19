using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public enum BuildingType
        {
            AnyBuilding,
            Nexus,
            SimpleBuilding,
            DefenseTower
        }
        
        public static GameManager gameManager;
        private Dictionary<BuildingType, List<Building.Building>> _buildingDictionary;
        [SerializeField] private List<Transform> allPlayer;

        private void Start()
        {
            if (gameManager != null)
            {
                Destroy(this);
            }

            gameManager = this;
            InitBuildDictionary();
        }

        public void AddBuilding(Building.Building.BuildingType type, Building.Building building)
        {
            switch (type)
            {
                case Building.Building.BuildingType.SimpleBuilding:
                    AddBuildingDictionary(BuildingType.SimpleBuilding, building);
                    break;
                case Building.Building.BuildingType.DefenseTower:
                    AddBuildingDictionary(BuildingType.DefenseTower, building);
                    break;
                case Building.Building.BuildingType.Nexus:
                    AddBuildingDictionary(BuildingType.Nexus, building);
                    break;
                default:
                    Debug.Log("Add building error not maching type");
                    break;
            }
        }

        private void AddBuildingDictionary(BuildingType buildingType, Building.Building building)
        {
            if (!_buildingDictionary[buildingType].Contains(building) && 
                !_buildingDictionary[BuildingType.AnyBuilding].Contains(building))
            {
                _buildingDictionary[buildingType].Add(building);
                _buildingDictionary[BuildingType.AnyBuilding].Add(building);
            }
            else
            {
                Debug.Log("bugg in add building to dictionary");
            }
        }

        private void InitBuildDictionary()
        {
            _buildingDictionary = new Dictionary<BuildingType, List<Building.Building>>();
            _buildingDictionary.Add(BuildingType.AnyBuilding, new List<Building.Building>());
            _buildingDictionary.Add(BuildingType.Nexus, new List<Building.Building>());
            _buildingDictionary.Add(BuildingType.SimpleBuilding, new List<Building.Building>());
            _buildingDictionary.Add(BuildingType.DefenseTower, new List<Building.Building>());
        }
    }
}
