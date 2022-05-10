using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipObject : MonoBehaviour
{
    [SerializeField] GameObject canvasObject;
    [SerializeField] RectTransform popupObject;
    [SerializeField] Text infoText;
    [SerializeField] Vector3 offset;
    [SerializeField] float padding;
    [SerializeField] Canvas canvas;

    private void Update() {
        FollowCursor();
    }

    public void FollowCursor()
    {
        Vector3 newPos = Input.mousePosition + offset;
        newPos.z = 0f;
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + popupObject.rect.width * canvas.scaleFactor / 2) - padding;
        if (rightEdgeToScreenEdgeDistance < 0)
        {
            newPos.x += rightEdgeToScreenEdgeDistance;
        }
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - popupObject.rect.width * canvas.scaleFactor * 1.8F) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + popupObject.rect.height * canvas.scaleFactor) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            newPos.y += topEdgeToScreenEdgeDistance;
        }
        popupObject.transform.position = newPos;
    }

    public void DisplayInfo(Item item)
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("<size=35>").Append(item.ItemName).Append("</size>").AppendLine();
        stringBuilder.Append(item.GetTooltipInfoText());

        infoText.text = stringBuilder.ToString();

        canvasObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }

    public void DisplayInfo(Building building)
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("<size=35>").Append(building.BuildingName).Append("</size>").AppendLine();
        stringBuilder.Append(building.GetTooltipInfoText());

        infoText.text = stringBuilder.ToString();

        canvasObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }

    public void DisplayInfo(string text)
    {

        infoText.text = text;

        canvasObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }

    public void HideInfo()
    {
        canvasObject.SetActive(false);
    }
}
