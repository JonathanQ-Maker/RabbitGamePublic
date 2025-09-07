using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ToolTipUI : MonoBehaviour
{
    public Vector2 offset;
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
        rectTransform.position = Input.mousePosition + new Vector3(offset.x, offset.y);
    }

    private void Update()
    {
        UpdatePosition();
    }
}