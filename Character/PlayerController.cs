using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacterController
{
    private int selectMask = 9;
    public Character character;
    public SimpleInventoryRenderer simpleInvRenderer;

    private void Start()
    {
        character.Subscribe(this);
    }

    private void Update()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (character == null) return;

        // left clicked

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectMask))
        {
            if (character.Mounted)
            { 
                character.DisMount();
            }

            if (hit.collider.TryGetComponent(out IWorldUsable usable))
            {
                character.StartUse(usable);
                return;
            }

            if (hit.collider.TryGetComponent(out ISimpleContainer container))
            {
                character.StartOpenContainer(container);
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

    public void OnOpenSimpleInv(ISimpleContainer container)
    {
        simpleInvRenderer.Inventory = container.SimpleInv;
        simpleInvRenderer.UpdateRender();
        simpleInvRenderer.gameObject.SetActive(true);
    }
}