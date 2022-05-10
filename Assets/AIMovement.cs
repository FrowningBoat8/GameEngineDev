using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform target;
    public Task currentTask;
    BuildingManager buildingManager;
    BaseResources baseResources;
    Item resouceHolding;
    bool hasReachedTarget;
    GameObject buildingBeingUsed;
    int itemSlot;
    [SerializeField] Transform AIWeapon;
    ParticleSystem.EmissionModule AIWeaponParticleSystem;
    Transform enemyTarget;

    void Start()
    {
        //Finds Object Refrences
        navMeshAgent = GetComponent<NavMeshAgent>();
        buildingManager = GameObject.FindWithTag("BuildingManager").GetComponent<BuildingManager>();
        baseResources = GameObject.FindWithTag("BuildingManager").GetComponent<BaseResources>();
        AIWeaponParticleSystem = AIWeapon.GetComponent<ParticleSystem>().emission;
        resouceHolding = new Item();
        currentTask = new Task();
        target = transform;
    }

    
    void Update()
    {
        if (currentTask.taskNum == -1){ return; }
        //Checks if citizen has reached target
        if (target != null && target != transform)
        {
            navMeshAgent.SetDestination(target.position);
            if (!target.Equals(gameObject.transform))
            {
                if(!hasReachedTarget)
                {
                    CheckHasReachedTarget();
                }
            }
        }
        if (currentTask.taskBuilding.CompareTag("Tower") && hasReachedTarget && enemyTarget == null)
        {
            SetWeaponTarget();
        }
    }

    /// <summary>
    /// Checks if current job still needs working
    /// </summary>
    public void UpdateJob()
    {
        //Checks if citizen needs a new job due to changed prioritys
        if (currentTask.taskNum != -1)
        {
            foreach(Task job in buildingManager.TasksToBeWorked)
            {
                if (currentTask.taskBuilding == job.taskBuilding && currentTask.taskNum == job.taskNum)
                {
                    buildingManager.TasksToBeWorked.Remove(job);
                    return;
                }
            }
        }
        StartCoroutine(FindNewJob());
    }

    /// <summary>
    /// Provides the citizen with a new job
    /// </summary>
    /// <returns></returns>
    IEnumerator FindNewJob()
    {
        yield return null;
        if (currentTask.taskBuilding != null)
        {
            //Removes citizen from tower if perious job building was a tower
            if (currentTask.taskBuilding.CompareTag("Tower"))
            {
                transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                hasReachedTarget = false;
                target = transform;
                navMeshAgent.enabled = true;
                AIWeaponParticleSystem.enabled = false;
            }
        }
        foreach(Task job in buildingManager.TasksToBeWorked)
        {
            currentTask = job;
            buildingManager.TasksToBeWorked.Remove(job);
            if(target == transform)
            {
                target = currentTask.taskBuilding.transform;
                SetCurrentJobToTarget();
            }
            break;
        }
    }

    /// <summary>
    /// Sets the current target to the new job building
    /// </summary>
    void SetCurrentJobToTarget()
    {
        //sets the ai's target to the current job
        if (buildingBeingUsed != null)
        {
            buildingBeingUsed.GetComponent<BuildingScript>().citizensWorking.Remove(gameObject);
        }
        buildingBeingUsed = target.gameObject;
        buildingBeingUsed.GetComponent<BuildingScript>().citizensWorking.Add(gameObject);
        FindClosestEntrance();
    }

    /// <summary>
    /// Finds the closest entrace of the target building and sets it as the target
    /// </summary>
    void FindClosestEntrance()
    {
        //looks though the list of entrance objects and sets destination to closest one
        List<Transform> entrances = target.GetComponent<BuildingScript>().entrances;
        float i = 5000;
        float j = 5000;
        foreach (Transform entrance in entrances)
        {
            navMeshAgent.SetDestination(entrance.position);
            j = Vector3.Distance(transform.position, entrance.position);
            if (i > j)
            {
                i = j;
                target = entrance;
            }
        }
    }

    /// <summary>
    /// Runs ReachedTarget() if the citizen has reached the target destination
    /// </summary>
    void CheckHasReachedTarget()
    {
        //checks if reached target
        for(int i = 0; i < target.parent.transform.childCount; i += 1)
        {
            if (Vector3.Distance(transform.position, target.parent.transform.GetChild(i).position) < 1)
            {
                ReachedTarget();
            }
        }
    }

    /// <summary>
    /// Function for begining using the building
    /// </summary>
    void ReachedTarget()
    {
        //if citizen has resources and building is target, places resources in building and becomes ready for more work.
        if (resouceHolding.Id != (int)BaseResources.itemIDs.EMPTY)
        {
            hasReachedTarget = true;
            target.parent.GetComponent<Inventory>().AddItemToInventory((int)resouceHolding.Id, 1);
            resouceHolding = new Item();
            if (target.parent.GetComponent<BuildingScript>().selectionCircle.activeSelf)
            {
                target.parent.GetComponent<Inventory>().UpdateUI();
            }
            target = currentTask.taskBuilding.transform;
            SetCurrentJobToTarget();
            hasReachedTarget = false;
        }
        //if target is building being worked begins working at building.
        else if (target.parent.tag.Equals("TreePlanter"))
        {
            hasReachedTarget = true;
            if (CheckWorkable())
            {
                StartCoroutine(workingTime(target.parent.GetComponent<BuildingScript>().workTime,1));
            }
            else
            {
                StartCoroutine(WaitForBuilding(1));
            }
        }
        else if(target.parent.tag.Equals("Storage"))
        {
            hasReachedTarget = true;
            itemSlot = buildingBeingUsed.GetComponent<Inventory>().SearchForFirstItemInInventory();
            if(itemSlot != -1)
            {
                StartCoroutine(workingTime(target.parent.GetComponent<BuildingScript>().workTime,2));
            }
            else
            {
                StartCoroutine(WaitForBuilding(2));
            }
        }
        else if(target.parent.tag.Equals("Tower") && !hasReachedTarget)
        {
            hasReachedTarget = true;
            target = transform;
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;
            navMeshAgent.Warp(currentTask.taskBuilding.TowerPositions[currentTask.taskBuilding.citizensWorkingNum].transform.position);
            currentTask.taskBuilding.citizensWorkingNum += 1;
            SetWeaponTarget();
        }
    }

    /// <summary>
    /// Clears current job variables if job building removed
    /// </summary>
    public void BuildingRemoved()
    {
        //Removes target when target is removed.
        if (currentTask.taskBuilding.CompareTag("Tower"))
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            hasReachedTarget = false;
            navMeshAgent.enabled = true;
            AIWeaponParticleSystem.enabled = false;
        }
        StopAllCoroutines();
        currentTask = new Task();
        target = gameObject.transform;
    }

    /// <summary>
    /// Function that waits the amount of time to work at the building then performs the relevent actions for the given job type.
    /// </summary>
    /// <param name="time">Amount of time work takes to complete</param>
    /// <param name="jobType">The job type</param>
    /// <returns></returns>
    IEnumerator workingTime(float time, int jobType)
    {
        buildingBeingUsed.GetComponent<BuildingScript>().StartCooldown(time);
        //Timer for how long work takes to complete
        yield return new WaitForSeconds(time);
        if (buildingBeingUsed == gameObject) { hasReachedTarget = false; yield break; }
        switch (jobType)
        {
            case 1: resouceHolding = buildingBeingUsed.GetComponent<BuildingScript>().resourceToBeCollected; break;
            case 2: JobType2(); break;
        }
        if (buildingBeingUsed.GetComponent<BuildingScript>().outputDestinations.Count == 0)
        {
            ReturnToBase();
        }
        else{
            target = buildingBeingUsed.GetComponent<BuildingScript>().outputDestinations[currentTask.taskNum];
            FindClosestEntrance();
        }
        hasReachedTarget = false;
    }

    /// <summary>
    /// Switch Statment Function
    /// </summary>
    void JobType2()
    {
        resouceHolding = new Item(buildingBeingUsed.GetComponent<Inventory>().items[itemSlot]);
        buildingBeingUsed.GetComponent<Inventory>().CheckIfResourceAvilible(resouceHolding.Id,1);
        resouceHolding.amount = 1;
    }

    /// <summary>
    /// Sets the target to the base building, was used for testing.
    /// </summary>
    void ReturnToBase()
    {
        //returns to base building
        target = buildingManager.basePos;
        FindClosestEntrance();
    }

    /// <summary>
    /// Waits for the building to become avilible then begins work
    /// </summary>
    /// <param name="jobType">The job type</param>
    /// <returns></returns>
    IEnumerator WaitForBuilding(int jobType)
    {
        //Timer for when citizen is waiting for building to be ready
        while(true)
        {
            yield return new WaitForSeconds(0.1F);
            if (jobType == 2)
            {
                itemSlot = buildingBeingUsed.GetComponent<Inventory>().SearchForFirstItemInInventory();
            }
            switch(jobType)
            {
                case 1: if (CheckWorkable()) {StartCoroutine(workingTime(1,jobType)); yield break;} break;
                case 2: if (CheckWorkable() && itemSlot != -1) {StartCoroutine(workingTime(1,jobType)); yield break;} break;
                    
            }
        }
    }

    /// <summary>
    /// Checks if the building is workable
    /// </summary>
    /// <returns>true if the building is workable</returns>
    bool CheckWorkable()
    {
        //Checks if building is workable
        if(buildingBeingUsed.GetComponent<BuildingScript>().workable)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the enemy target by geting a target from the current building
    /// </summary>
    public void SetWeaponTarget()
    {
        enemyTarget = currentTask.taskBuilding.GetClosestEnemy(transform);
        if (enemyTarget != transform)
        {
            StopAllCoroutines();
            AIWeaponParticleSystem.enabled = true;
            AIWeapon.LookAt(enemyTarget.position);
            StartCoroutine(TargetEnemy());
        }
        else
        {
            enemyTarget = transform;
            AIWeaponParticleSystem.enabled = false;
        }
    }

    /// <summary>
    /// Aims weapon at the current enemy target
    /// </summary>
    /// <returns></returns>
    IEnumerator TargetEnemy()
    {
        while (enemyTarget != null)
        {
            AIWeapon.LookAt(enemyTarget.position);
            yield return new WaitForSeconds(0.1F);
        }
    }
}
