using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    BaseResources baseResources;
    public Item[] items;
    int blankSlot;
    int itemSlot;
    int slotChecking;
    int slotSelected;
    public int SlotSelected { get {return slotSelected; } }
    [SerializeField] int inventorySize = 3;
    InventorySlots inventorySlots;
    [SerializeField] bool isPlayerInventory;
    [SerializeField] bool isCrafterInventory;
    Color defaultColor;
    public bool IsCrafterInventory { get {return isCrafterInventory; } }
    Text[] inventoryContentsTxt;
    CraftingRecipe currentCraftingRecipe;
    Slider craftingProgressBar;
    Text craftTimeText;
    bool crafting;

    void Start()
    {
        //gets gameobject refrences
        baseResources = GameObject.FindWithTag("BuildingManager").GetComponent<BaseResources>();
        inventorySlots = GameObject.FindWithTag("InventorySlots").GetComponent<InventorySlots>();
        craftingProgressBar = inventorySlots.craftingProgressBar;
        craftTimeText = inventorySlots.craftingTimerText;
        //populates the items array with values and with a size of inventory size
        items = new Item[inventorySize];
        for(int i = 0; i < items.Length; i += 1)
        {
            items[i] = new Item();
        }
        //Sets up the inventory text objects
        if (isPlayerInventory)
        {
            inventoryContentsTxt = new Text[inventorySlots.playerInventorySlots.Length * 2];
            int j = 0;
            for(int i = 0; i < inventorySlots.playerInventorySlots.Length; i += 1)
            {
                inventoryContentsTxt[j] = inventorySlots.playerInventorySlots[i].transform.GetChild(0).GetComponent<Text>();
                j += 1;
                inventoryContentsTxt[j] = inventorySlots.playerInventorySlots[i].transform.GetChild(1).GetComponent<Text>();
                j += 1;
            }
            UpdateUI();
        } else if (isCrafterInventory) {
            inventoryContentsTxt = new Text[inventorySlots.crafterInventorySlots.Length * 2];
            int j = 0;
            for(int i = 0; i < inventorySlots.crafterInventorySlots.Length; i += 1)
            {
                inventoryContentsTxt[j] = inventorySlots.crafterInventorySlots[i].transform.GetChild(0).GetComponent<Text>();
                j += 1;
                inventoryContentsTxt[j] = inventorySlots.crafterInventorySlots[i].transform.GetChild(1).GetComponent<Text>();
                j += 1;
            }
            //Temp
            SetRecipe(baseResources.allRecipes[0]);
        } else {
            inventoryContentsTxt = new Text[inventorySlots.inventorySlots.Length * 2];
            int j = 0;
            for(int i = 0; i < inventorySlots.inventorySlots.Length; i += 1)
            {
                inventoryContentsTxt[j] = inventorySlots.inventorySlots[i].transform.GetChild(0).GetComponent<Text>();
                j += 1;
                inventoryContentsTxt[j] = inventorySlots.inventorySlots[i].transform.GetChild(1).GetComponent<Text>();
                j += 1;
            }
        }
        //Adds starting supplys to inventory
        if (gameObject.CompareTag("Base"))
        {
            AddItemToInventory((int)BaseResources.itemIDs.WOOD, 30);
        }
        slotSelected = -1;
        defaultColor = new Color(1F,1F,1F,1F);
    }

    /// <summary>
    /// This adds a amount of items to the objects inventory of the ID specified
    /// </summary>
    /// <param name="ID">ID of item</param>
    /// <param name="amount">Amount of items to be added</param>
    /// <returns>N/A</returns>
    public void AddItemToInventory(int ID, int amount)
    {
        //if item is already in inventory and stack size is not maximum, adds 1 to the stack
        bool itemFound = SearchForItemInInventory(ID,amount);
        if (itemFound)
        {
            items[itemSlot].amount += amount;
        }
        else
        //otherwise tries to find empty slot, if non are found the function ends
        {
            if (blankSlot == -1)
            {
                return;
            } else {
                
                if (ID >= 0)
                {
                    items[blankSlot] = new Item(baseResources.allGameItems[ID].item);
                    items[blankSlot].amount += amount;
                }
            }
        }
        if (isCrafterInventory)
        {
            CheckCanCraftItem();
        }
    }

    /// <summary>
    /// Searches the inventory to see if amount of item ID can fit into a avilible space.
    /// </summary>
    /// <param name="ID">ID of item to be checked</param>
    /// <param name="amount">Amount of item to be checked</param>
    /// <returns>Returns true if availble space was found, otherwise returns false</returns>
    bool SearchForItemInInventory(int ID, int amount)
    {
        //Searches for item in inventory with the ID specified with enough space left 
        //to add the amount to without exceeding the max stack size
        slotChecking = 0;
        blankSlot = -1;
        itemSlot = -1;
        for(int i = 0; i < items.Length; i += 1)
        {
            if (items[slotChecking].Id == 0 && blankSlot == -1)
            {
                blankSlot = slotChecking;
            }
            if (items[slotChecking].Id == ID && itemSlot == -1 && items[slotChecking].amount + amount <= items[slotChecking].Stacksize)
            {
                itemSlot = slotChecking;
            }
            slotChecking += 1;
        }
        if (itemSlot == -1)
        {
            itemSlot = 0;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Searchs for the first item in the inventory
    /// </summary>
    /// <returns>The slot num of the item found, otherwise if no slot found returns -1</returns>
    public int SearchForFirstItemInInventory()
    {
        slotChecking = 0;
        for(int i = 0; i < items.Length; i += 1)
        {
            if (items[slotChecking].Type == 0)
            {
                
            }
            else
            {
                return slotChecking;
            }
            slotChecking += 1;
        }
        return -1;
    }

    /// <summary>
    /// Sets how many of the inventory slots are shown on the UI depending on inventory size and type
    /// </summary>
    void SetHowManySlotsAreDisplayed()
    {
        if (isPlayerInventory)
        {
            for(int i = 0; i < inventorySlots.playerInventorySlots.Length; i += 1)
            {
                if(i < inventorySize)
                {
                    inventorySlots.playerInventorySlots[i].SetActive(true);
                } else {
                    inventorySlots.playerInventorySlots[i].SetActive(false);
                }
            }
        } else if (isCrafterInventory) {
            for(int i = 0; i < inventorySlots.crafterInventorySlots.Length; i += 1)
            {
                if(i < 4)
                {
                    if (currentCraftingRecipe.inputItemsAmount > i)
                    {
                        inventorySlots.crafterInventorySlots[i].SetActive(true);
                    }
                    else {inventorySlots.crafterInventorySlots[i].SetActive(false);}
                } else if (i < 8) {
                    if (currentCraftingRecipe.outputItemsAmount > (i - 4))
                    {
                        inventorySlots.crafterInventorySlots[i].SetActive(true);
                    }
                    else {inventorySlots.crafterInventorySlots[i].SetActive(false);}
                }
            }
            craftTimeText.text = currentCraftingRecipe.craftTime + " Sec";
        } else {
            for(int i = 0; i < inventorySlots.inventorySlots.Length; i += 1)
            {
                if(i < inventorySize)
                {
                    inventorySlots.inventorySlots[i].SetActive(true);
                } else {
                    inventorySlots.inventorySlots[i].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Updates the inventorys UI
    /// </summary>
    public void UpdateUI()
    {
        int j = 0;
        for(int i = 0; i < items.Length; i += 1)
        {
            if (items[i].Id == 0)
            {
                inventoryContentsTxt[j].text = "";
                j += 1;
                inventoryContentsTxt[j].text = "";
                j += 1;
            }
            else{
                inventoryContentsTxt[j].text = items[i].ItemName;
                j += 1;
                inventoryContentsTxt[j].text = items[i].amount.ToString();
                j += 1;
            }
            inventoryContentsTxt[j-2].transform.parent.GetComponent<TooltipButton>().tooltipItem = items[i];
        }
        SetHowManySlotsAreDisplayed();
    }

    /// <summary>
    /// Sets this buildings crafting recipe to the one provied
    /// </summary>
    /// <param name="recipe">The new recipe</param>
    public void SetRecipe(CraftingRecipe recipe)
    {
        currentCraftingRecipe = recipe;    
        items[0] = new Item(recipe.inputItems[0]);
        if (recipe.inputItemsAmount > 1) {
            items[1] = new Item(recipe.inputItems[1]);
        } else if (recipe.inputItemsAmount > 2) {
            items[2] = new Item(recipe.inputItems[2]);
        } else if (recipe.inputItemsAmount > 3) {
            items[3] = new Item(recipe.inputItems[3]);
        }
        items[4] = new Item(recipe.outputItems[0]);
        if (recipe.outputItemsAmount > 1) {
            items[5] = new Item(recipe.outputItems[1]);
        } else if (recipe.outputItemsAmount > 2) {
            items[6] = new Item(recipe.outputItems[2]);
        } else if (recipe.outputItemsAmount > 3) {
            items[7] = new Item(recipe.outputItems[3]);
        }
    }

    /// <summary>
    /// Checks to see if the requied items are avilible, that crafting the item wont exceed the current items stack size
    /// and that it isnt already crafting, then begins crafting the recipe item/items.
    /// </summary>
    public void CheckCanCraftItem()
    {
        if (crafting){return;}
        if (items[0].amount >= currentCraftingRecipe.inputAmounts[0] && items[4].Stacksize > items[4].amount + currentCraftingRecipe.outputAmounts[0])
        {
            if (items[1].amount < currentCraftingRecipe.inputAmounts[1] && currentCraftingRecipe.inputItemsAmount > 1)
            {
                return;
            }
            if (items[2].amount < currentCraftingRecipe.inputAmounts[2] && currentCraftingRecipe.inputItemsAmount > 1)
            {
                return;
            }
            if (items[3].amount < currentCraftingRecipe.inputAmounts[3] && currentCraftingRecipe.inputItemsAmount > 1)
            {
                return;
            }
            crafting = true;
            StartCoroutine(StartCrafting(currentCraftingRecipe.craftTime));
        }
    }

    /// <summary>
    /// Crafts the item/items after the amount of time requied has elapsed
    /// </summary>
    /// <param name="craftTime">Amount of time requied to craft the item/items</param>
    /// <returns></returns>
    IEnumerator StartCrafting(float craftTime)
    {
        for(int i = 1; i < 21; i += 1)
        {
            craftingProgressBar.value = i;
            yield return new WaitForSeconds(craftTime/20);
        }
        
        items[0].amount -= currentCraftingRecipe.inputAmounts[0];
        items[1].amount -= currentCraftingRecipe.inputAmounts[1];
        items[2].amount -= currentCraftingRecipe.inputAmounts[2];
        items[3].amount -= currentCraftingRecipe.inputAmounts[3];
        items[4].amount += currentCraftingRecipe.outputAmounts[0];
        items[5].amount += currentCraftingRecipe.outputAmounts[1];
        items[6].amount += currentCraftingRecipe.outputAmounts[2];
        items[7].amount += currentCraftingRecipe.outputAmounts[3];
        craftingProgressBar.value = 0;
        crafting = false;
        UpdateUI();
        CheckCanCraftItem();
    }

    /// <summary>
    /// Updates the colour of the inventory slot
    /// </summary>
    /// <param name="slot">Inventory slot number</param>
    public void selectSlot(int slot)
    {   
        if (slot == -1){
            if (isPlayerInventory)
            {
                inventorySlots.playerInventorySlots[slotSelected].GetComponent<Image>().color = defaultColor;
            } else if (IsCrafterInventory){
                inventorySlots.crafterInventorySlots[slotSelected].GetComponent<Image>().color = defaultColor;
            } else {
                inventorySlots.inventorySlots[slotSelected].GetComponent<Image>().color = defaultColor;
            }
        }
        slotSelected = slot;
        if (slot >= 0)
        {
            if (isPlayerInventory)
            {
                inventorySlots.playerInventorySlots[slot].GetComponent<Image>().color = inventorySlots.slotSelectedColor;
            } else if (IsCrafterInventory){
                inventorySlots.crafterInventorySlots[slot].GetComponent<Image>().color = inventorySlots.slotSelectedColor;
            } else {
                inventorySlots.inventorySlots[slot].GetComponent<Image>().color = inventorySlots.slotSelectedColor;
            }
        }
    }

    /// <summary>
    /// Checks if the resource is available in the inventory, removes the item if true.
    /// </summary>
    /// <param name="resourceID">ID of item to be searched for</param>
    /// <param name="amount">Amount of item</param>
    /// <returns>True if item successfully found, false otherwise</returns>
    public bool CheckIfResourceAvilible(int resourceID, int amount)
    {
        bool itemFound = SearchForItemInInventory(resourceID,0);
        if (itemFound && items[itemSlot].amount >= amount)
        {
            items[itemSlot].amount -= amount;
            if (items[itemSlot].amount < 1)
            {
                items[itemSlot] = new Item();
            }
            UpdateUI();
            return true;
        }
        return false;
    }
}
