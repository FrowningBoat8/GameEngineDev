using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAIScript : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    [SerializeField] Transform tempTarget;
    BuildingScript targetBuilding;
    Transform target;
    BuildingManager buildingManager;
    bool hasReachedTarget;
    float attackRate = 0.5F;
    float health;
    [SerializeField] float maxHealth;
    [SerializeField] TextMesh healthText;
    [SerializeField] SpriteRenderer healthBar;
    [SerializeField] Color[] healthBarColors;
    float healthBarPercentage;


    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        buildingManager = GameObject.FindWithTag("BuildingManager").GetComponent<BuildingManager>();
        target = transform;
        health = maxHealth;
        healthBar.color = healthBarColors[0];
        healthText.text = health.ToString();
    }

    void Update()
    {
        if(!hasReachedTarget)
        {
            CheckHasReachedTarget();
        }
        if (target.Equals(transform))
        {
            FindClosestBuilding();
            navMeshAgent.SetDestination(target.position);
        }
    }

    void FindClosestBuilding()
    {
        //gets the list of avilible buildings
        float i = 5000;
        float j = 5000;
        target = transform;
        //looks though list to find closest one
        foreach (Transform buildingTransform in buildingManager.transform)
        {
            BuildingScript building = buildingTransform.GetComponent<BuildingScript>();
            if (building.Health > 0)
            {
                j = Vector3.Distance(transform.position, building.transform.position);
                if (i > j)
                {
                    i = j;
                    target = building.transform;
                    targetBuilding = building;
                }
            }
        }
        //sets citizen to working and the building to being uesd and looks for the nearest entrance
        if (!target.Equals(gameObject.transform))
        {
            FindClosestEntrance();
        }
    }

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

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.CompareTag("Tower"))
        {
            other.transform.parent.GetComponent<BuildingScript>().EnemyEnteredRange(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other) {
        TookDamage(1);
        Debug.Log(health);
    }

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

    void ReachedTarget()
    {
        if (!target.Equals(gameObject.transform))
        {
            hasReachedTarget = true;
            StartCoroutine(AttackBuilding());
        }
    }

    IEnumerator AttackBuilding()
    {
        while(hasReachedTarget)
        {
            yield return new WaitForSeconds(attackRate);
            targetBuilding.TookDamage(1);
            Debug.Log("Building Under Attack " + targetBuilding.Health);
            if (targetBuilding.Health < 1)
            {
                Debug.Log("Building Destroyed " + targetBuilding.Health);
                target = transform;
                hasReachedTarget = false;
            }
        }
    }

    /// <summary>
    /// Function for when the enemy takes damage, removes the given amount of health and updates the health bar
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
            GameObject.Destroy(gameObject);
        }
    }

}
