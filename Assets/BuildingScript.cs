using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class BuildingScript : MonoBehaviour
{
    public List<Transform> entrances;
    public GameObject selectionCircle;
    [HideInInspector] public List<Transform> outputDestinations;
    [HideInInspector] public BuildingManager buildingManager;
    BaseResources baseResources;
    [HideInInspector] public bool workable;
    [HideInInspector] public List<GameObject> citizensWorking;
    public float workTime;
    [SerializeField] float cooldownTime;
    public PlayerControls.buildings buildingType;
    [HideInInspector] public BuildingManager.taskTypes buildingTask;
    [SerializeField] BaseResources.itemIDs resourceIDToBeCollected;
    public Item resourceToBeCollected;
    public int citizensWorkingNum;
    public GameObject[] TowerPositions;
    public int maxCapacity;
    [HideInInspector] public int currentCapacity;
    List<Task> towerTasks;
    public List<GameObject> enemiesWithinRange;
    float health;
    [SerializeField] float maxHealth;
    [SerializeField] TextMesh healthText;
    [SerializeField] SpriteRenderer healthBar;
    [SerializeField] Color[] healthBarColors;
    float healthBarPercentage;
    public float Health { get {return health; } }
    

    //Script for building functions
    void Awake() {
        outputDestinations = new List<Transform>();
        citizensWorking = new List<GameObject>();
        enemiesWithinRange = new List<GameObject>();
        if (gameObject.CompareTag("Tower"))
        {
            towerTasks = new List<Task>();
        }
        currentCapacity = maxCapacity;
        workable = true;
        health = maxHealth;
        healthBar.color = healthBarColors[0];
        healthText.text = health.ToString();
        healthBar.transform.parent.rotation = new Quaternion(0.6F,0F,0F,0.8F);
        //TransformUtils.SetInspectorRotation(healthBar.transform.parent, new Vector3(70,270,0));
    }

    void AddTowerTasks()
    {
        for(int i = 0; i < maxCapacity; i += 1)
        {
            towerTasks.Add(new Task(this, buildingManager, i));
            if (i < currentCapacity)
            {
                buildingManager.Tasks.Add(towerTasks[i]);
            }
        }
    }

    void Start()
    {
        if (gameObject.CompareTag("Base"))
        {
            buildingType = PlayerControls.buildings.NOTHING;
        }
        GetBuildingTask();
        StartCoroutine("LateStart");
        if (gameObject.CompareTag("Tower"))
        {
            AddTowerTasks();
        }
    }

    IEnumerator LateStart()
    {
        yield return null;
        baseResources = buildingManager.GetComponent<BaseResources>();
        resourceToBeCollected = new Item(baseResources.allGameItems[(int)resourceIDToBeCollected].item);
    }

    public void GetBuildingTask()
    {
        switch (buildingType)
        {
            case PlayerControls.buildings.TREE_PLANTER: buildingTask = GetResourceBuildingTask(); break;
            case PlayerControls.buildings.CRAFTING_TIER_1: buildingTask = BuildingManager.taskTypes.NOTHING; break;
            case PlayerControls.buildings.STORAGE_TIER_1: buildingTask = GetStorageBuildingTask(); break;
        }
    }

    BuildingManager.taskTypes GetResourceBuildingTask()
    {
        if (outputDestinations.Count > 0)
        {
            switch(outputDestinations[0].GetComponent<BuildingScript>().buildingType)
            {
                case PlayerControls.buildings.CRAFTING_TIER_1: return BuildingManager.taskTypes.CRAFT_WOOD_PLANKS;
                default: return BuildingManager.taskTypes.GARTHER_WOOD;
            }
        }
        return BuildingManager.taskTypes.GARTHER_WOOD;
    }

    BuildingManager.taskTypes GetStorageBuildingTask()
    {
        if (outputDestinations.Count > 0)
        {
            switch(outputDestinations[0].GetComponent<BuildingScript>().buildingType)
            {
                case PlayerControls.buildings.CRAFTING_TIER_1: return BuildingManager.taskTypes.CRAFT_WOOD_PLANKS;
                default: return BuildingManager.taskTypes.NOTHING;
            }
        }
        return BuildingManager.taskTypes.NOTHING;
    }

    public void StartCooldown(float workTime)
    {
        StartCoroutine(Cooldown(workTime));
    }

    IEnumerator Cooldown(float workTime)
    {
        workable = false;
        yield return new WaitForSeconds(cooldownTime + workTime);
        workable = true;
    }

    public void BuildingRemoved()
    {
        foreach(GameObject citizenWorking in citizensWorking)
        {
            citizenWorking.GetComponent<AIMovement>().BuildingRemoved();
        }
    }

    public void RemoveOutputDestination(int taskNum)
    {
        outputDestinations.RemoveAt(taskNum);
    }

    public void AddTowerTask()
    {
        buildingManager.Tasks.Add(towerTasks[currentCapacity-1]);
    }

    public void RemoveTowerTask()
    {
        buildingManager.Tasks.Remove(towerTasks[currentCapacity]);
    }

    /// <summary>
    /// Adds the enemy onto the enimies within range list when a enemy enters range and updates all citizens working at the building
    /// </summary>
    /// <param name="enemy">Enemy that has entered range</param>
    public void EnemyEnteredRange(GameObject enemy)
    {
        enemiesWithinRange.Add(enemy);
        foreach(GameObject citizen in citizensWorking)
        {
            citizen.GetComponent<AIMovement>().SetWeaponTarget();
        }
    }

    /// <summary>
    /// Finds the closest enemy to the citizen calling the function
    /// </summary>
    /// <param name="citizenTransform">The citizens transform</param>
    /// <returns>The closest enemy transform</returns>
    public Transform GetClosestEnemy(Transform citizenTransform)
    {
        float i = 5000;
        float j = 5000;
        Transform currentClosestEnemy = citizenTransform;
        foreach (GameObject enemy in enemiesWithinRange)
        {
            if (enemy == null){enemiesWithinRange.Remove(enemy);}
            j = Vector3.Distance(citizenTransform.position, enemy.transform.position);
            if (i > j)
            {
                i = j;
                currentClosestEnemy = enemy.transform;
            }
        }
        return currentClosestEnemy;
    }

    /// <summary>
    /// Function for when the building takes damage, removes the given amount of health and updates the health bar
    /// </summary>
    /// <param name="amount">Amount of health to remove</param>
    public void TookDamage(int amount)
    {
        health -= amount;
        healthText.text = health.ToString();
        healthBarPercentage = ((health/maxHealth)*4.8F)+0.2F;
        healthBar.size = new Vector2(healthBarPercentage,healthBar.size.y);
        if (healthBarPercentage > 3.6F)
        {
            healthBar.color = healthBarColors[0];
        }
        else if (healthBarPercentage > 1.9F)
        {
            healthBar.color = healthBarColors[1];
        }
        else
        {
            healthBar.color = healthBarColors[2];
        }
        Debug.Log(healthBar.size.x);
        if (health < 0)
        {
            //GameObject.Destroy(gameObject);
        }
    }
}
