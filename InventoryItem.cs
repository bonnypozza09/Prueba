using System;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class InventoryItem : MonoBehaviour, IPointerDownHandler
{
    // --- Is this item trashable --- //
    public bool esEliminable;

    [Header("Equipamiento")]
    public bool isEquipable;
    public bool isInsideQuickSlot;
    public bool isSelected;

    [Header("Sistema de construccion")]
    public bool isUseable;
    public GameObject itemPendingToBeUsed;

    void Update()
    {
        if(isSelected)
        {
            gameObject.GetComponent<DragDrop>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<DragDrop>().enabled = true;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Right Mouse Button Click on
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Obtener informaciÃ³n del item
            string itemName = gameObject.name.Replace("(Clone)", "");
            VidaPlayerInfoItems itemData = InventorySystem.Instance.availableItems.Find(item => item.itemName == itemName);
            
            // Si es consumible, consumir el item
            if (itemData != null && itemData.isConsumable)
            {
                ConsumeItem(itemData);
                return;
            }
            
            // Si es equipable y no consumible, equipar
            if (isEquipable && isInsideQuickSlot == false && EquipSystem.Instance.CheckIfFull() == false)
            {
                EquipSystem.Instance.AddToQuickSlots(gameObject);
                isInsideQuickSlot = true;
            }

            // Si es usables y es el item pendiente, usarlo
            if(isUseable && itemPendingToBeUsed == gameObject)
            {
                // DestroyImmediate(gameObject);
                InventorySystem.Instance.ReCalculateList();
                CraftingSystem.Instance.RefreshNeededItems();
            }

            // Si es usables y no consumible, usarlo
            if(isUseable)
            {
                itemPendingToBeUsed = gameObject;
                UseItem();
            }
        }
    }

    private void UseItem()
    {
        InventorySystem.Instance.estabierto = false;
        InventorySystem.Instance.inventarioUI.SetActive(false);

        CraftingSystem.Instance.estabierto = false;
        CraftingSystem.Instance.craftingScreenUI.SetActive(false);
        CraftingSystem.Instance.toolsScreenUI.SetActive(false);
        CraftingSystem.Instance.survivalScreenUI.SetActive(false);
        CraftingSystem.Instance.refineScreenUI.SetActive(false);
        CraftingSystem.Instance.constructionScreenUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SelectionManager.Instance.EnableSelection();
        SelectionManager.Instance.enabled = true;

        switch (gameObject.name)
        {
            case "Foundation(Clone)":
                ConstructionManager.Instance.ActivateConstructionPlacement("FounfationModel");
                break;
            case "Foundation":
                ConstructionManager.Instance.ActivateConstructionPlacement("FoundationModel");
                break;
            default:
                break;
        }

    }

    private void ConsumeItem(VidaPlayerInfoItems itemData)
    {
        // Obtener el PlayerStatsManager
        PlayerStatsManager statsManager = FindFirstObjectByType<PlayerStatsManager>();
        
        if (statsManager != null)
        {
            // Aplicar efectos de recuperaciÃ³n
            if (itemData.caloriasRecuperadas > 0)
                statsManager.ConsumirComida(itemData.caloriasRecuperadas);
            
            if (itemData.hidratacionRecuperada > 0)
                statsManager.ConsumirAgua(itemData.hidratacionRecuperada);
            
            // Reproducir sonido de consumo
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickupItemSound);
            
            // Eliminar el item del inventario usando la misma lÃ³gica que TrashSlot
            Destroy(gameObject);
            TooltipSystem.Instance.HideTooltip();
            InventorySystem.Instance.ReCalculateList();
            CraftingSystem.Instance.RefreshNeededItems();
        }
    }
 
}