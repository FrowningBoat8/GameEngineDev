using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Building
{
    BaseResources baseResources;
    string buildingName;
    public string BuildingName { get {return buildingName; } }
    string description;
    public string Description { get {return description; } }
    List<Vector2Int> resourceCosts; //x = item id, y = item amount
    public List<Vector2Int> ResourceCosts { get {return resourceCosts; } }

    public Building(BaseResources bR, string bN, string d)
    {
        baseResources = bR;
        buildingName = bN;
        description = d;
        resourceCosts = new List<Vector2Int>();
    }

    /// <summary>
    /// Adds resource cost to costructing this building
    /// </summary>
    /// <param name="resourceID"></param>
    /// <param name="resourceAmount"></param>
    public void AddResourceCost(int resourceID, int resourceAmount)
    {
        resourceCosts.Add(new Vector2Int(resourceID, resourceAmount));
    }

    /// <summary>
    /// Provides the items tooltip text
    /// </summary>
    /// <returns>A string of the tooltip text</returns>
    public string GetTooltipInfoText()
    {
        //Use for coloured text
        //stringBuilder.Append("<color=green>Description: ").Append(description).Append("</color>").AppendLine();

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("Description: ").Append(description).AppendLine();
        
        stringBuilder.Append("Cost: ").Append(resourceCosts[0].y).Append(" ").Append(baseResources.allGameItems[resourceCosts[0].x].item.ItemName);

        return stringBuilder.ToString();
    }
}
