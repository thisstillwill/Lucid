using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item item;
    public Image icon;
    public Text textPanel;
    public int amount = 0;
    private int maxAmount;
    public GameObject amountPanel;
    public Text amountText;
    public bool isSelected = false;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        textPanel.text = item.name;
    }

    public void AddRecipeComponent(Item newComponent, int n)
    {
            item = newComponent;
            icon.sprite = item.icon;
            icon.enabled = true;
            amount = n;
            maxAmount = n;
            amountPanel.SetActive(true);
            amountText.text = amount + "";
    }

    public void SubtractFromAmount()
    {
        if (amount > 0)
        {
            amount--;
            amountText.text = amount + "";
        }
    }

    public void AddToAmount()
    {
        if (amount < maxAmount)
        {
            amount++;
            amountText.text = amount + "";
        }
    }
    public void ResetAmount()
    {
        amount = maxAmount;
        amountText.text = amount + "";
    }
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        amount = 0;
        maxAmount = 0;
        if (textPanel != null)
        {
            textPanel.text = "";
        }
        if (amountText != null)
        {
            amountText.text = "";
            amountPanel.SetActive(false);
        }
    }
}
