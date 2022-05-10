using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    //This holds varibles required by all inventory scripts
    public GameObject[] inventorySlots;

    public GameObject[] crafterInventorySlots;

    public GameObject[] playerInventorySlots;

    public Slider craftingProgressBar;
    public Text craftingTimerText;
    public Color slotSelectedColor;
}
