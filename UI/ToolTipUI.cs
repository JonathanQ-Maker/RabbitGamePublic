using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI toolTip;
    private Canvas canvas;

    public RectTransform rectTransform { get { return transform as RectTransform; } }

    public string Text
    {
        get { return toolTip.text; }
        set
        {
            toolTip.text = value;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    public void Initalize(Canvas parentCanvas)
    { 
        canvas = parentCanvas;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        rectTransform.position = Input.mousePosition;
        UpdatePivot();
    }

    private void UpdatePivot()
    {
        Vector2 topRight = rectTransform.anchoredPosition + rectTransform.sizeDelta;
        Vector2 currentPivot = rectTransform.pivot;
        if (topRight.x > Screen.width / canvas.scaleFactor)
        {
            currentPivot.x = 1;
        }
        else
        {
            currentPivot.x = 0;
        }

        if (topRight.y > Screen.height / canvas.scaleFactor)
        {
            currentPivot.y = 1;
        }
        else
        {
            currentPivot.y = 0;
        }
        rectTransform.pivot = currentPivot;
    }

    private void Update()
    {
        UpdatePosition();
    }
}