using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BaseResources : MonoBehaviour
{
    public ItemCraftingRecipes[] allGameItems;

    public CraftingRecipe[] allRecipes;
    public enum itemIDs{EMPTY, WOOD, PLANKS};
    public enum itemTypes{UNASSIGNED, MATERIAL};

    void Awake()
    {
        //Creates items and crafting recipes then adds them to the relivent list
        allGameItems = new ItemCraftingRecipes[3];
        allRecipes = new CraftingRecipe[5];
        allGameItems[0] = (new ItemCraftingRecipes("N/A", (int)itemIDs.EMPTY, 0, "Bug Here", 0, 64));
        allGameItems[1] = (new ItemCraftingRecipes("Wood Logs", (int)itemIDs.WOOD, (int)itemTypes.MATERIAL, "Obtained from trees", 0, 64));
        allGameItems[2] = (new ItemCraftingRecipes("Wood Planks", (int)itemIDs.PLANKS, (int)itemTypes.MATERIAL, "Crafted from logs", 0, 64));
        allRecipes[0] = new CraftingRecipe(allGameItems[1].item, allGameItems[2].item, 5, 3, 3);
        allGameItems[1].AddCraftingRecipe(allRecipes[0]);
    }

    /*
    public int SearchItemList(int ID)
    {
        int i = 0;
        foreach(ItemCraftingRecipes item in allGameItems)
        {
            if (item.item.Id == ID)
            {
                Debug.Log(i + " / " + ID);
                return i;
            }
            i += 1;
        }
        return -1;
    }
    */
}

public class ItemCraftingRecipes
{
    public Item item;
    public List<CraftingRecipe> craftingRecipes;

    public ItemCraftingRecipes(string iN, int id, int t, string d, int a, int ss)
    {
        item = new Item(iN, id, t, d, a, ss);
        craftingRecipes = new List<CraftingRecipe>();
    }

    /// <summary>
    /// Adds a crafting recipe to the item
    /// </summary>
    /// <param name="cR">The Crafting recipe to add</param>
    public void AddCraftingRecipe(CraftingRecipe cR)
    {
        craftingRecipes.Add(cR);
    }
}


public class Item
{
    string itemName;
    public string ItemName { get {return itemName; } }
    int ID;
    public int Id { get {return ID; } }
    int type;
    public int Type { get {return type; } }
    string description;
    public string Description { get {return description; } }
    public int amount;
    int stacksize;
    public int Stacksize { get {return stacksize; } }

    public Item()
    {
        itemName = "Empty Slot";
        ID = 0;
        type = 0;
        description = "";
        amount = 0;
        stacksize = 0;
    }

    public Item(string iN, int id, int t, string d, int a, int ss)
    {
        itemName = iN;
        ID = id;
        type = t;
        description = d;
        amount = a;
        stacksize = ss;
    }

    public Item(Item item)
    {
        itemName = item.itemName;
        ID = item.Id;
        type = item.Type;
        description = item.Description;
        amount = 0;
        stacksize = item.Stacksize;
    }

    /// <summary>
    /// Provides the items tooltip text
    /// </summary>
    /// <returns>A string of the tooltip text</returns>
    public string GetTooltipInfoText()
    {
        //Use for coloured text
        //stringBuilder.Append("<color=green>Description: ").Append(description).Append("</color>").AppendLine();

        //Creates Tooltip Text to display
        StringBuilder stringBuilder = new StringBuilder();
        if(ID == 0){return "";}

        stringBuilder.Append(ItemTypeToString()).AppendLine();

        stringBuilder.Append("Description: ").Append(description).AppendLine();
        
        stringBuilder.Append("Amount: ").Append(amount).Append("/").Append(stacksize);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Converts the int into a string for the tooltip to display
    /// </summary>
    /// <returns>Itemtype as string</returns>
    string ItemTypeToString()
    {
        switch (type)
        {
            case (int)BaseResources.itemTypes.UNASSIGNED: return "No Type";
            case (int)BaseResources.itemTypes.MATERIAL: return "Material";
            default: return "Fix this";
        }
    }
}

public class CraftingRecipe
{
    public Item[] inputItems;
    public Item[] outputItems;
    public int[] inputAmounts;
    public int[] outputAmounts;
    public float craftTime;
    public int inputItemsAmount;
    public int outputItemsAmount;

    public CraftingRecipe(Item i, Item oI, int iA, int oA, float cT)
    {
        inputItems = new Item[4];
        outputItems = new Item[4];
        inputAmounts = new int[4];
        outputAmounts = new int[4];
        inputItems[0] = i;
        outputItems[0] = oI;
        inputAmounts[0] = iA;
        outputAmounts[0] = oA;
        craftTime = cT;
        inputItemsAmount = 1;
        outputItemsAmount = 1;
    }

    /// <summary>
    /// Adds a item required for crafting the item/items of this recipe
    /// </summary>
    /// <param name="i">The Item</param>
    /// <param name="amount">Amount of the item</param>
    public void AddInputItem(Item i, int amount)
    {
        inputItems[inputItemsAmount] = i;
        inputAmounts[inputItemsAmount] = amount;
        inputItemsAmount += 1;
    }

    /// <summary>
    /// Adds a item to be crafted from this recipe
    /// </summary>
    /// <param name="i">The item</param>
    /// <param name="amount">Amount of the item</param>
    public void AddOutputItem(Item i, int amount)
    {
        outputItems[outputItemsAmount] = i;
        outputAmounts[outputItemsAmount] = amount;
        outputItemsAmount += 1;
    }

}
