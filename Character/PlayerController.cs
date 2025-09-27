using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, ICharacterController
{
    private int selectMask = 9;
    public Character character;

    [SerializeField]
    private SimpleInventoryRenderer simpleInvRenderer;
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
            if (!simpleInvRenderer.gameObject.activeSelf)
            {
                OpenSimpleInv(character.Inventory);
            } 
            else 
            {
                CloseSimpleInv();
            }
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
        simpleInvRenderer.Inventory = inventory;
        simpleInvRenderer.UpdateRender();
        simpleInvRenderer.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
    }

    private void CloseSimpleInv()
    {
        simpleInvRenderer.Inventory = null;
        simpleInvRenderer.UpdateRender();
        simpleInvRenderer.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }
}