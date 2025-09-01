using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string itemName;
    
    void Start()
    {
        // Obtener el nombre del item del GameObject (removiendo "(Clone)" si existe)
        itemName = gameObject.name.Replace("(Clone)", "");
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(EstaEnQuickSlot())
            return;
        {
            // Buscar el ScriptableObject correspondiente
            VidaPlayerInfoItems itemData = InventorySystem.Instance.availableItems.Find(item => item.itemName == itemName);
            
            if (itemData != null)
            {
                TooltipSystem.Instance.ShowTooltip(itemData.itemName, itemData.description, itemData.functionality, itemData.itemIcon);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.HideTooltip();
    }

    private bool EstaEnQuickSlot()
    {
        Transform parent = transform.parent;
        if(parent != null && parent.CompareTag("QuickSlot"))
        {
            return true;
        }
        return false;
    }
}