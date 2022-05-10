using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePriorityManager : MonoBehaviour
{
    //This class simpily holds the current prioirty values for each job catergory
    [HideInInspector]
    public int[] prioritys;

    [SerializeField] BuildingManager buildingManager;

    void Awake() {
        prioritys = new int[System.Enum.GetValues(typeof(BuildingManager.taskTypes)).Length];
    }

    void Start()
    {
        UpdateBuildings();
    }

    /// <summary>
    /// Updates building prioritys
    /// </summary>
    public void UpdateBuildings()
    {
        buildingManager.UpdateBuildingPrioritys(prioritys);
    }
}
