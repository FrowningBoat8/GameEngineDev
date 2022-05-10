using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    List<Task> tasks;
    public List<Task> Tasks { get {return tasks; } }
    List<Task> tasksNotChecked;
    List<Task> tasksToBeWorked;
    public List<Task> TasksToBeWorked { get {return tasksToBeWorked; } }
    List<GameObject> treePlanters;
    public List<GameObject> TreePlanters { get {return treePlanters; } }
    List<GameObject> storageBuildings;
    public List<GameObject> StorageBuildings { get {return storageBuildings; } }
    public enum taskTypes{NOTHING ,GARTHER_WOOD, CRAFT_WOOD_PLANKS};
    public Transform basePos;
    [HideInInspector] public int[] currentPrioritys;
    [SerializeField] Transform citizenPool;
    List<AIMovement> citizens;
    
    //Managages the lists of buildings created
    void Awake()
    {
        
        treePlanters = new List<GameObject>();
        storageBuildings = new List<GameObject>();
        tasks = new List<Task>();
        tasksNotChecked = new List<Task>();
        tasksToBeWorked = new List<Task>();
        citizens = new List<AIMovement>();
        foreach(Transform citizen in citizenPool)
        {
            citizens.Add(citizen.GetComponent<AIMovement>());
        }
    }

    public void AddBuildingToList(GameObject building, PlayerControls.buildings buildingType)
    {
        building.GetComponent<BuildingScript>().buildingManager = this;
    }

    /// <summary>
    /// Orders the tasks by priority value
    /// </summary>
    /// <returns>The lowest priory num to be assigned to a citizen</returns>
    public int OrderByPriority()
    {
        if (tasks.Count > 0)
        {
            tasks = tasks.OrderBy (e => e.priority).ToList ();
        }
        if (citizens.Count > tasks.Count)
        {
            return tasks[0].priority;
        }
        return tasks[tasks.Count - citizens.Count].priority;
    }

    void RecheckJobsToAssign()
    {
        tasksNotChecked = new List<Task>(tasks);
        if (tasksNotChecked.Count == 0) {return;}
        /*
        for(int i = 0; i < citizens.Count; i += 1)
        {
            int j = -1;
            if (tasksNotChecked.Count == 0) {break;}
            Task taskToBeAdded = tasks[0];
            foreach(Task task in tasksNotChecked)
            {
                if (task.priority > j)
                {
                    j = task.priority;
                    taskToBeAdded = task;
                }
            }
            tasksToBeWorked.Add(taskToBeAdded);
            tasksNotChecked.Remove(taskToBeAdded);
        }
        */
        int i = OrderByPriority();
        FirstCheck(i);
        SecondCheck(i);
        AssignCitizenTasks();
        StartCoroutine(ClearLists());
    }

    void FirstCheck(int x)
    {
        foreach(Task task in tasksNotChecked)
        {
            if (task.priority > x)
            {
                tasksToBeWorked.Add(task);
            }
        }
    }

    void SecondCheck(int x)
    {
        if (tasksToBeWorked.Count == citizens.Count){return;}
        foreach(Task task in tasksNotChecked)
        {
            if (task.priority == x)
            {
                tasksToBeWorked.Add(task);
            }
            if (tasksToBeWorked.Count == citizens.Count){return;}
        }
    }

    IEnumerator ClearLists()
    {
        yield return null;
        yield return null;
        tasksNotChecked.Clear();
        tasksToBeWorked.Clear();
    }

    /// <summary>
    /// Asiigns the found jobs to the current citizens
    /// </summary>
    void AssignCitizenTasks()
    {
        foreach(AIMovement citizen in citizens)
        {
            citizen.UpdateJob();
        }
    }

    public void UpdateBuildingPrioritys(int[] prioritys)
    {
        currentPrioritys = prioritys;
        if (tasks.Count > 0)
        {
            foreach(Task task in tasks)
            {
                task.priority = prioritys[(int)task.taskType];
            }
            RecheckJobsToAssign();
        }
    }

    public Task CheckForTask(BuildingScript building, int taskNum)
    {
        foreach (Task task in tasks)
        {
            if (task.taskNum == taskNum && task.taskBuilding == building)
            {
                return task;
            }
        }
        return new Task();
    }

    public void RemoveAllTasksFromBuilding(BuildingScript building)
    {
        List<Task> tasksToBeRemoved = new List<Task>(); 
        foreach (Task task in tasks)
        {
            if (task.taskBuilding == building)
            {
                tasksToBeRemoved.Add(task);
            }
        }
        foreach (Task task in tasksToBeRemoved)
        {
            tasks.Remove(task);
        }
    }
}

    public class Task
    {
    public BuildingScript taskBuilding;
    public BuildingManager.taskTypes taskType;
    public int priority;
    public int taskNum;

    public Task(BuildingScript tB, BuildingManager bM, int tN)
    {
        taskBuilding = tB;
        taskNum = tN;
        SetTaskType();
        priority = bM.currentPrioritys[(int)taskType];
    }

    public Task()
    {
        taskNum = -1;

    }

    public void SetTaskType()
    {
        switch (taskBuilding.buildingType)
        {
            case PlayerControls.buildings.TREE_PLANTER: taskType = GetResourceBuildingTask(); break;
            case PlayerControls.buildings.CRAFTING_TIER_1: taskType = BuildingManager.taskTypes.NOTHING; break;
            case PlayerControls.buildings.STORAGE_TIER_1: taskType = GetStorageBuildingTask(); break;
            case PlayerControls.buildings.BASIC_TOWER: taskType = BuildingManager.taskTypes.NOTHING; break;
        }
    }

    BuildingManager.taskTypes GetResourceBuildingTask()
    {
        if (taskBuilding.outputDestinations.Count > 0)
        {
            switch(taskBuilding.outputDestinations[taskNum].GetComponent<BuildingScript>().buildingType)
            {
                case PlayerControls.buildings.CRAFTING_TIER_1: return BuildingManager.taskTypes.CRAFT_WOOD_PLANKS;
                default: return BuildingManager.taskTypes.GARTHER_WOOD;
            }
        }
        return BuildingManager.taskTypes.GARTHER_WOOD;
    }

    BuildingManager.taskTypes GetStorageBuildingTask()
    {
        if (taskBuilding.outputDestinations.Count > 0)
        {
            switch(taskBuilding.outputDestinations[0].GetComponent<BuildingScript>().buildingType)
            {
                case PlayerControls.buildings.CRAFTING_TIER_1: return BuildingManager.taskTypes.CRAFT_WOOD_PLANKS;
                default: return BuildingManager.taskTypes.NOTHING;
            }
        }
        return BuildingManager.taskTypes.NOTHING;
    }

}
