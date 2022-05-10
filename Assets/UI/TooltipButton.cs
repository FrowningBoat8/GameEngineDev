using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TooltipObject tooltipObject; 
    [SerializeField] BaseResources baseResources;
    [HideInInspector] public Item tooltipItem;
    [SerializeField] PlayerControls.buildings building;
    [SerializeField] PlayerMenus playerMenus;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (building.Equals(PlayerControls.buildings.NOTHING))
        {
            tooltipObject.DisplayInfo(tooltipItem);
        }
        else
        {
            tooltipObject.DisplayInfo(playerMenus.buildingsInfo[(int)building-1]);
        }

        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipObject.HideInfo();
    }
}
