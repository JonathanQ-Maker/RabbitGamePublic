using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class UIItemRenderer : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private ItemStack itemStack;
    public ItemStack ItemStack
    {
        get { return itemStack; }
        set { itemStack = value; }
    }

    public ItemSlotUI parentSlot;

    [SerializeField]
    private Image image;

    [SerializeField]
    private TextMeshProUGUI countText;


    private IEnumerator actionLoop;
    protected virtual IEnumerator ActionLoop
    {
        get { return actionLoop; }
        set
        {
            if (actionLoop != null)
                StopCoroutine(actionLoop);
            actionLoop = value;
            if (actionLoop != null)
                StartCoroutine(actionLoop);
        }
    }




    private Canvas canvas;
    private void Start()
    {
        Canvas[] canvases = GetComponentsInParent<Canvas>();
        canvas = canvases[canvases.Length - 1]; // topmost canvas
        ResetPosition();
    }



    public void UpdateRender()
    {
        if (ItemStack != null && ItemStack.Item.GetCount(ItemStack) > 0)
        {
            name = $"UIItemRenderer ({ItemStack.Item.GetName(ItemStack)})";
            image.sprite = ItemStack.Item.Sprite;
            countText.gameObject.SetActive(true);
            countText.text = $"{ItemStack.Item.GetCount(ItemStack)}";
            return;
        }

        // no item, so should not be rendered
        Destroy(gameObject);
    }

    public void ResetPosition()
    {
        if (parentSlot == null) return;
        transform.SetParent(null);
        transform.SetParent(parentSlot.ItemHolder);
        ((RectTransform)transform).localPosition = Vector3.zero;
    }

    public int TransferStackTo(ItemStack other, int amountRequested)
    {
        if (parentSlot == null || !parentSlot.interactable)
            return 0;

        // can the two items combine?
        if (!ItemStack.Item.CanCombine(ItemStack, other))
            return 0;

        int amountTransferred = ItemStack.Item.TransferToStack(ItemStack, other, amountRequested);
        UpdateRender();
        return amountTransferred;
    }

















    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != InputButton.Left) return;
        if (parentSlot != null && !parentSlot.interactable) return;
        parentSlot.OnDismountItem();

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        ActionLoop = HandleClick();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != InputButton.Left) return;

        // update current hover game object

        if (parentSlot != null && !parentSlot.interactable) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != InputButton.Left) return;

        // did we drop on something?
        if (eventData.pointerEnter == null)
        {
            // nope, go back
            goto Reset;
        }

        // did we drop on a slot?
        if (eventData.pointerEnter.TryGetComponent(out ItemSlotUI slotUI))
        {
            // yup, but can item be mounted?
            if (slotUI.TryMountItem(this))
            {
                // success
                ResetPosition();
                image.raycastTarget = true;
                return;
            }

            // nope, go back
            goto Reset;
        }

        // did we drop on another ui item?
        if (eventData.pointerEnter.TryGetComponent(out UIItemRenderer uiItem))
        {
            // yup, try to combine
            if (uiItem.TransferStackTo(ItemStack, ItemStack.Item.MaxStackCount) > 0)
            {
                // some did combine
                UpdateRender();
            }

            if (ItemStack.Item.GetCount(ItemStack) > 0)
                goto Reset;

            // nothing left in stack, wait to be destroyed
            return;
        }



    Reset:
        if (!parentSlot.TryMountItem(this))
        {
            // can't go back, drop myself
            // TODO: actually drop
            Debug.Log("Item Dropped");
            Destroy(gameObject);
            return;
        }
        ResetPosition();
        image.raycastTarget = true;
    }




    private IEnumerator HandleClick()
    {
        while (true)
        {
            // did player click?
            if (!Input.GetMouseButtonDown(1)) goto Next;

            GameObject hover = GetCurrentUIUnderPointer();
            if (hover == null) goto Next;

            // was it clicked on another ui item?
            if (hover.TryGetComponent(out UIItemRenderer uiItem))
            {
                if (uiItem.TransferStackTo(ItemStack, 1) > 0)
                {
                    UpdateRender();
                }
                goto Next;
            }

            // was it clocked on an item slot?
            if (hover.TryGetComponent(out ItemSlotUI slotUI))
            {
                // is the ui item's slot blocked?
                if (!slotUI.interactable)
                    goto Next;

                if (ItemStack.Item.SplitStack(ItemStack, out ItemStack newStack, 1) > 0)
                {
                    // can split stack for empty slot
                    slotUI.SetItem(newStack);
                    slotUI.UpdateRender();
                }
                else
                {
                    slotUI.SetItem(ItemStack);
                    ItemStack = null;
                    slotUI.UpdateRender();
                }
                UpdateRender();
            }

            Next:
            yield return null;
        }
    }

    public static GameObject GetCurrentUIUnderPointer()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }
}