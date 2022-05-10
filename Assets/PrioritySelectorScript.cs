using UnityEngine;
using UnityEngine.UI;

public class PrioritySelectorScript : MonoBehaviour
{
    //Code from https://forum.unity.com/threads/problems-linking-a-slider-and-an-input-field.279083/
    public BasePriorityManager basePriorityManager;
    public BuildingManager.taskTypes taskType;
    public InputField inputField;
    public Slider slider;
    public void SliderUpdate(float value) 
    { 
        inputField.text = value.ToString(); 
        basePriorityManager.prioritys[(int)taskType] = (int)value;
    }
    public void InputFieldUpdate(string text) 
    { 
        slider.value = System.Convert.ToSingle(text); 
        basePriorityManager.prioritys[(int)taskType] = (int)slider.value;
    }
}
