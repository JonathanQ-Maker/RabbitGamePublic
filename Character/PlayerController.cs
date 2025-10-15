using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, ICharacterController
{
    private int selectMask = 9;
    public Character character;

    [SerializeField]
    private SimpleInventoryMenu otherInv;
    [SerializeField]
    private SimpleInventoryMenu characterInv;
    [SerializeField]
    private Image background;
    [SerializeField]
    private LineDrawerer lineDrawerer;

    private void Start()
    {
        character.Subscribe(this);
    }

    private void Update()
    {
        HandleClick();
        HandleHover();
        HandleKey();
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (character == null) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // left clicked

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectMask))
        {
            if (!ReferenceEquals(character.OpenedObject, null)) 
            { 
                character.CloseContainer();
            }

            if (character.Mounted)
            { 
                character.DisMount();
            }

            if (hit.collider.TryGetComponent(out WorldItemRenderer worldItem))
            {
                character.StartGetItem(worldItem);
                return;
            }

            if (hit.collider.TryGetComponent(out IUsable usable))
            {
                character.StartUse(usable);
                return;
            }

            if (hit.collider.TryGetComponent(out IOpenable container))
            {
                character.StartOpen(container);
                return;
            }

            if (hit.collider.TryGetComponent(out IMountable mountable))
            {
                character.StartMount(mountable);
                return;
            }

            character.StartMoveTo(hit.point);
        }
    }
    private void HandleHover() 
    {
        if (character == null) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            lineDrawerer.UpdateBounds(null);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectMask))
        {
            lineDrawerer.UpdateBounds(hit.collider);
            return;
        }
        lineDrawerer.UpdateBounds(null);
    }

    private void HandleKey() 
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (otherInv.gameObject.activeSelf || characterInv.gameObject.activeSelf)
            {
                CloseSimpleInv();
                return;
            }

            OpenSimpleInv(null);
        }
    }

    public void OnOpen(object result)
    {
        if (result is SimpleInventory inventory)
        {
            OpenSimpleInv(inventory);
        }
    }

    public void OnClose()
    {
        CloseSimpleInv();
    }

    private void OpenSimpleInv(SimpleInventory inventory) 
    {
        if (inventory != null) { 
            otherInv.Inventory = inventory;
            otherInv.UpdateRender();
            otherInv.gameObject.SetActive(true);
        }
        characterInv.Inventory = character.Inventory;
        characterInv.UpdateRender();
        characterInv.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
    }

    private void CloseSimpleInv()
    {
        otherInv.Inventory = null;
        otherInv.UpdateRender();
        otherInv.gameObject.SetActive(false);

        characterInv.Inventory = null;
        characterInv.UpdateRender();
        characterInv.gameObject.SetActive(false);

        background.gameObject.SetActive(false);
    }
}