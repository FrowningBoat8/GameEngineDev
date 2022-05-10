using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenus : MonoBehaviour
{
    PlayerControls playerControls;
    Inventory playerInventory;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] BaseResources baseResources;
    List<GameObject> buildingsSelected;
    public List<GameObject> BuildingsSelected { get {return buildingsSelected; } }
    [SerializeField] Text buildingSelectedTxt;
    bool changingOutputDestination;
    [SerializeField] Text[] outputDestinationTexts;
    [SerializeField] GameObject outputDestinationsMenu;
    [SerializeField] Text currentTowerCapacityText;
    [SerializeField] GameObject towerCapacityMenu;
    [HideInInspector] public PlayerControls.buildings buildingToBeBuilt;
    [SerializeField] Image buildingToBeBuiltButton;
    [SerializeField] Color buttonSelectedColor;
    [HideInInspector] public Building[] buildingsInfo;
    Color defaultColor;
    [Header("Menu UI Sections")]
    [SerializeField] GameObject buildingInventoryPanel;
    [SerializeField] GameObject crafterInventoryPanel;
    [SerializeField] GameObject changeRecipePanel;
    [SerializeField] GameObject playerInventoryPanel;
    [SerializeField] GameObject basePriorityPanel;

    [Header("Health Options")]
    [SerializeField] float maxHealth;
    float health;
    [SerializeField] TextMesh healthText;
    [SerializeField] SpriteRenderer healthBar;
    [SerializeField] Color[] healthBarColors;
    float healthBarPercentage;

    void Start()
    {
        buildingsSelected = new List<GameObject>();
        playerControls = GetComponent<PlayerControls>();
        playerInventory = GetComponent<Inventory>();
        defaultColor = new Color(1F,1F,1F,1F);
        buildingsInfo = new Building[Enum.GetNames(typeof(PlayerControls.buildings)).Length];
        buildingsInfo[0] = new Building(baseResources, "Tree Planter","Grow trees here for wood.");
        buildingsInfo[0].AddResourceCost((int)BaseResources.itemIDs.WOOD,5);
        buildingsInfo[1] = new Building(baseResources, "Storage Tier 1","Simple storage building");
        buildingsInfo[1].AddResourceCost((int)BaseResources.itemIDs.WOOD,5);
        buildingsInfo[2] = new Building(baseResources, "Crafter Tier 1","Simple crafting building");
        buildingsInfo[2].AddResourceCost((int)BaseResources.itemIDs.WOOD,5);
        buildingsInfo[3] = new Building(baseResources, "Basic Tower","Basic defence tower");
        buildingsInfo[3].AddResourceCost((int)BaseResources.itemIDs.WOOD,5);
        health = maxHealth;
        healthBar.color = healthBarColors[0];
        healthText.text = health.ToString();
    }

    /// <summary>
    /// Performs the relevent actions depending on which building was clicked on
    /// </summary>
    /// <param name="hit">The building clicked on</param>
    public void CheckClicked(RaycastHit hit)
    {
        if (!changingOutputDestination && !hit.collider.CompareTag("Buildable"))
            {
                buildingToBeBuiltButton.color = defaultColor;
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    TurnOffSelectionCircle();
                    buildingsSelected.Clear();
                }
                if (buildingsSelected.Count > 0)
                {
                    if (hit.collider.transform.parent.gameObject.GetComponent<BuildingScript>().buildingType == buildingsSelected[0].GetComponent<BuildingScript>().buildingType)
                    {
                        buildingsSelected.Add(hit.collider.transform.parent.gameObject);
                    }
                } else {
                    buildingsSelected.Add(hit.collider.transform.parent.gameObject);
                }
                UpdateSelectionPanel();
                buildingToBeBuilt = PlayerControls.buildings.NOTHING;
                playerControls.DisplayBuildGuide(false, buildingToBeBuilt);
                outputDestinationsMenu.SetActive(true);
                if(buildingsSelected[0].CompareTag("Tower"))
                {
                    crafterInventoryPanel.SetActive(false);
                    buildingInventoryPanel.SetActive(false);
                    outputDestinationsMenu.SetActive(false);
                    playerInventoryPanel.SetActive(false);
                    towerCapacityMenu.SetActive(true);
                }
                else if (buildingsSelected[0].TryGetComponent<Inventory>(out Inventory inventory))
                {
                    inventory.UpdateUI();
                    towerCapacityMenu.SetActive(false);
                    playerControls.MenuEnabled(true);
                    if (inventory.IsCrafterInventory)
                    {
                        crafterInventoryPanel.SetActive(true);
                        buildingInventoryPanel.SetActive(false);
                    }
                    else 
                    {
                        buildingInventoryPanel.SetActive(true);
                        crafterInventoryPanel.SetActive(false);
                    }
                    playerInventoryPanel.SetActive(true);
                }
            }
            else if (changingOutputDestination && hit.collider.transform.parent.TryGetComponent<Inventory>(out Inventory inventory))
            {
                foreach(GameObject building in buildingsSelected)
                {
                    building.GetComponent<BuildingScript>().outputDestinations.Add(hit.collider.transform.parent);
                    building.GetComponent<BuildingScript>().GetBuildingTask();
                    buildingManager.Tasks.Add(new Task(building.GetComponent<BuildingScript>(), buildingManager, building.GetComponent<BuildingScript>().outputDestinations.Count-1));
                }
                UpdateSelectionPanel();
                buildingToBeBuilt = PlayerControls.buildings.NOTHING;
                playerControls.DisplayBuildGuide(false, buildingToBeBuilt);
            }
    }

    /// <summary>
    /// Updates the selection panel UI
    /// </summary>
    void UpdateSelectionPanel()
    {
        if (buildingsSelected.Count > 0)
        {
            if (!changingOutputDestination)
            {
                buildingsSelected.Last().GetComponent<BuildingScript>().selectionCircle.SetActive(true);
            }
            buildingSelectedTxt.text = buildingsSelected[0].name;
            for(int i = 0; i < outputDestinationTexts.Length; i++)
            {
                if (buildingsSelected[0].GetComponent<BuildingScript>().outputDestinations.Count <= i)
                {
                    outputDestinationTexts[i].text = "???";
                } else {
                    outputDestinationTexts[i].text = buildingsSelected[0].GetComponent<BuildingScript>().outputDestinations[i].name;
                }
            }
            currentTowerCapacityText.text = buildingsSelected[0].GetComponent<BuildingScript>().currentCapacity.ToString();
        }
        else {
            buildingSelectedTxt.text = "None";
        }
    }

    public void RemoveBuildingOutput(int value)
    {
        outputDestinationTexts[value].text = "???";
        buildingsSelected[0].GetComponent<BuildingScript>().outputDestinations.RemoveAt(value);
        buildingManager.Tasks.Remove(buildingManager.CheckForTask(buildingsSelected[0].GetComponent<BuildingScript>(), buildingsSelected[0].GetComponent<BuildingScript>().outputDestinations.Count));
        UpdateSelectionPanel();
    }

    public void SetOutputDestination()
    {
        changingOutputDestination = true;
        buildingToBeBuilt = PlayerControls.buildings.NOTHING;
        playerControls.DisplayBuildGuide(false, buildingToBeBuilt);
    }

    /// <summary>
    /// Checks if resouces are avilible for the building to be constructed
    /// </summary>
    /// <returns></returns>
    public bool CheckCanBuild()
    {
        if (playerInventory.CheckIfResourceAvilible((int)BaseResources.itemIDs.WOOD, 5))
        {
            return true;
        }
        return false;
    }

    public void PrepareToBuild(BuildingSelection buildingClass)
    {
        buildingToBeBuiltButton.color = defaultColor;
        DeselectBuilding();
        buildingToBeBuilt = buildingClass.building;
        buildingToBeBuiltButton = buildingClass.thisButtonImage;
        buildingToBeBuiltButton.color =  buttonSelectedColor;
        playerControls.DisplayBuildGuide(true, buildingToBeBuilt);
    }

    /// <summary>
    /// Deselectets the current building
    /// </summary>
    public void DeselectBuilding()
    {
        TurnOffSelectionCircle();
        buildingsSelected.Clear();
        changingOutputDestination =  false;
        UpdateSelectionPanel();
        buildingInventoryPanel.SetActive(false);
        crafterInventoryPanel.SetActive(false);
        playerInventoryPanel.SetActive(false);
        changeRecipePanel.SetActive(false);
        towerCapacityMenu.SetActive(false);
        outputDestinationsMenu.SetActive(false);
        playerControls.MenuEnabled(false);
    }

    void TurnOffSelectionCircle()
    {
        if (buildingsSelected.Count > 0)
        {
            foreach(GameObject building in buildingsSelected)
            {
                building.GetComponent<BuildingScript>().selectionCircle.SetActive(false);
            }
        }
    }

    public void DestroyBuilding()
    {
        List<GameObject> buildingsToBeDestroyed;
        buildingsToBeDestroyed = new List<GameObject>();
        foreach(GameObject building in buildingsSelected)
        {
            if (building.CompareTag("Base")) {} else 
            {
                buildingsToBeDestroyed.Add(building);
            }
        }
        DeselectBuilding();
        PlayerControls.buildings buildingToBeDestroyedType = buildingsToBeDestroyed[0].GetComponent<BuildingScript>().buildingType;
        foreach(GameObject building in buildingsToBeDestroyed)
        {
            //playerControls.RemoveBuildingFromList(building, buildingToBeDestroyedType);
            building.GetComponent<BuildingScript>().BuildingRemoved();
            buildingManager.RemoveAllTasksFromBuilding(building.GetComponent<BuildingScript>());
            GameObject.Destroy(building);
        }
        buildingsToBeDestroyed.Clear();
    }

    public void BeginChangingRecipe(bool enable)
    {
        if (buildingsSelected[0].GetComponent<Inventory>().IsCrafterInventory)
        {
            changeRecipePanel.SetActive(enable);
            crafterInventoryPanel.SetActive(!enable);
        }
    }

    public void ChangeBuildingRecipe(int value)
    {
        foreach(GameObject building in buildingsSelected)
        {
            building.GetComponent<Inventory>().SetRecipe(baseResources.allRecipes[value]);
        }
        changeRecipePanel.SetActive(false);
    }

    public void ChangeTowerCapacity(int amount)
    {
        BuildingScript buildingScript = buildingsSelected[0].GetComponent<BuildingScript>();
        if(buildingScript.currentCapacity + amount <= buildingScript.maxCapacity && buildingScript.currentCapacity + amount > 0)
        {
            buildingScript.currentCapacity += amount;
        }
        currentTowerCapacityText.text = buildingScript.currentCapacity.ToString();
        if (amount == 1)
        {
            buildingScript.AddTowerTask();
        }
        else
        {
            buildingScript.RemoveTowerTask();
        }
    }

    public void SelectPlayerInventorySlot(int value)
    {
        SelectInventorySlot(value, true);
    }

    public void SelectBuildingInventorySlot(int value)
    {
        SelectInventorySlot(value, false);
    }

    /// <summary>
    /// Selects the inventory slot clicked on if non selected, otherwise moves the items from the previously selcted slot to the slot clicked and deslectes all inventory slots.
    /// </summary>
    /// <param name="value">The number of the slot clicked</param>
    /// <param name="isPlayerInventory">True if player inventory clicked, false otherwise</param>
    void SelectInventorySlot(int value, bool isPlayerInventory)
    {
        Inventory buildingInventory = buildingsSelected[0].GetComponent<Inventory>();
        Item tempItem = new Item();
        if (playerInventory.SlotSelected == -1 & buildingInventory.SlotSelected == -1)
        {
            if (isPlayerInventory)
            {
                playerInventory.selectSlot(value);
            } else {
                buildingInventory.selectSlot(value);
            }
        }
        else
        {
            if(playerInventory.SlotSelected == -1)
            {
                if (isPlayerInventory)
                {
                    if(buildingInventory.items[buildingInventory.SlotSelected].Id == playerInventory.items[value].Id)
                    {
                        int tempValue = playerInventory.items[value].amount;
                        playerInventory.items[value].amount += buildingInventory.items[buildingInventory.SlotSelected].amount;
                        if (playerInventory.items[value].amount > playerInventory.items[value].Stacksize)
                        {
                            playerInventory.items[value].amount = playerInventory.items[value].Stacksize;
                            buildingInventory.items[buildingInventory.SlotSelected].amount -= (playerInventory.items[value].amount - tempValue);
                        }
                        else 
                        {
                            buildingInventory.items[buildingInventory.SlotSelected] = new Item();
                        }
                    }
                    else
                    {
                        tempItem = buildingInventory.items[buildingInventory.SlotSelected];
                        buildingInventory.items[buildingInventory.SlotSelected] = playerInventory.items[value];
                        playerInventory.items[value] = tempItem;
                    }
                }
                else
                {
                    if(buildingInventory.items[buildingInventory.SlotSelected].Id == buildingInventory.items[value].Id && buildingInventory.SlotSelected != value)
                    {
                        int tempValue = buildingInventory.items[value].amount;
                        buildingInventory.items[value].amount += buildingInventory.items[buildingInventory.SlotSelected].amount;
                        if (buildingInventory.items[value].amount > buildingInventory.items[value].Stacksize)
                        {
                            buildingInventory.items[value].amount = buildingInventory.items[value].Stacksize;
                            buildingInventory.items[buildingInventory.SlotSelected].amount -= (buildingInventory.items[value].amount - tempValue);
                        }
                        else if (tempValue < buildingInventory.items[value].amount)
                        {
                            buildingInventory.items[buildingInventory.SlotSelected] = new Item();
                        }
                    }
                    else
                    {
                        tempItem = buildingInventory.items[buildingInventory.SlotSelected];
                        buildingInventory.items[buildingInventory.SlotSelected] = buildingInventory.items[value];
                        buildingInventory.items[value] = tempItem;
                    }
                }
                buildingInventory.selectSlot(-1);
            }
            else
            {
                if (isPlayerInventory)
                {
                    if(playerInventory.items[playerInventory.SlotSelected].Id == playerInventory.items[value].Id && playerInventory.SlotSelected != value)
                    {
                        int tempValue = playerInventory.items[value].amount;
                        playerInventory.items[value].amount += playerInventory.items[playerInventory.SlotSelected].amount;
                        if (playerInventory.items[value].amount > playerInventory.items[value].Stacksize)
                        {
                            playerInventory.items[value].amount = playerInventory.items[value].Stacksize;
                            playerInventory.items[playerInventory.SlotSelected].amount -= (playerInventory.items[value].amount - tempValue);
                        }
                        else
                        {
                            playerInventory.items[playerInventory.SlotSelected] = new Item();
                        }
                    }
                    else
                    {
                        tempItem = playerInventory.items[playerInventory.SlotSelected];
                        playerInventory.items[playerInventory.SlotSelected] = playerInventory.items[value];
                        playerInventory.items[value] = tempItem;
                    }
                }
                else
                {
                    if(playerInventory.items[playerInventory.SlotSelected].Id == buildingInventory.items[value].Id)
                    {
                        int tempValue = buildingInventory.items[value].amount;
                        buildingInventory.items[value].amount += playerInventory.items[playerInventory.SlotSelected].amount;
                        if (buildingInventory.items[value].amount > buildingInventory.items[value].Stacksize)
                        {
                            buildingInventory.items[value].amount = buildingInventory.items[value].Stacksize;
                            playerInventory.items[playerInventory.SlotSelected].amount -= (buildingInventory.items[value].amount - tempValue);
                        }
                        else 
                        {
                            playerInventory.items[playerInventory.SlotSelected] = new Item();
                        }
                    }
                    else
                    {
                        tempItem = playerInventory.items[playerInventory.SlotSelected];
                        playerInventory.items[playerInventory.SlotSelected] = buildingInventory.items[value];
                        buildingInventory.items[value] = tempItem;
                    }
                }
                playerInventory.selectSlot(-1);
            }
            buildingInventory.UpdateUI();
            if (buildingInventory.IsCrafterInventory)
            {
                buildingInventory.CheckCanCraftItem();
            }
            playerInventory.UpdateUI();
        }
    }

    public void OpenPrioritysMenu(bool shouldOpen)
    {
        basePriorityPanel.SetActive(shouldOpen);
        playerControls.menuUIBlock.SetActive(shouldOpen);
    }

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
