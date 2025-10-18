using UnityEngine;

public class VendingMachine : MonoBehaviour, IOpenable
{
    private TradeModel tradeModel;
    public TradeModel TradeMenu { get { return tradeModel; } }

    [Range(1, 10)]
    public int tradeSlots;


    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Item coin;

    private void Start()
    {
        tradeModel = new TradeModel();

        for (int i = 0; i < tradeSlots; i++)
            tradeModel.AddTrade(new ItemStack(coin), null);
    }

    public object Open(object source)
    {
        animator.SetTrigger("HatchFlip");
        return tradeModel;
    }

    public void Close()
    {
    }
}