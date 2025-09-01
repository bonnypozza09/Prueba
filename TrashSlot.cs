using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class TrashSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
 
    public GameObject trashAlertUI;
    private TextMeshProUGUI textToModify;
    private Image itemIconImage;
 
    public Sprite trash_closed;
    public Sprite trash_opened;
 
    private Image imageComponent;
 
    Button YesBTN, NoBTN;
 
    GameObject DraggedItem
    {
        get
        {
            return DragDrop.artículoArrastrado;
        }
    }
    GameObject itemToBeDeleted;
  
    public string ItemName
    {
        get
        {
            string name = itemToBeDeleted.name;
            string toRemove = "(Clone)";
            string result = name.Replace(toRemove, "");
            return result;
        }
    }
 
    void Start()
    {
        imageComponent = transform.Find("background").GetComponent<Image>();
 
        textToModify = trashAlertUI.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        itemIconImage = trashAlertUI.transform.Find("ItemIcon").GetComponent<Image>();
 
        YesBTN = trashAlertUI.transform.Find("yes").GetComponent<Button>();
        YesBTN.onClick.AddListener(delegate { DeleteItem(); });
 
        NoBTN = trashAlertUI.transform.Find("no").GetComponent<Button>();
        NoBTN.onClick.AddListener(delegate { CancelDeletion(); });
 
    }
 
    public void OnDrop(PointerEventData eventData)
    {
        //itemToBeDeleted = DragDrop.itemBeingDragged.gameObject;
        if (DraggedItem.GetComponent<InventoryItem>().esEliminable == true)
        {
            itemToBeDeleted = DraggedItem.gameObject;
 
            StartCoroutine(NotifyBeforeDeletion());
        }
        
    }
 
    IEnumerator NotifyBeforeDeletion()
    {
        trashAlertUI.SetActive(true);
        textToModify.text = "Eliminar " + ItemName + "?";
        
        // Obtener y mostrar el icono del item
        VidaPlayerInfoItems itemData = InventorySystem.Instance.availableItems.Find(item => item.itemName == ItemName);
        if (itemData != null && itemData.itemIcon != null && itemIconImage != null)
        {
            itemIconImage.sprite = itemData.itemIcon;
            itemIconImage.gameObject.SetActive(true);
        }
        else if (itemIconImage != null)
        {
            itemIconImage.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(1f);
    }
 
    private void CancelDeletion()
    {
        imageComponent.sprite = trash_closed;
        trashAlertUI.SetActive(false);
    }
 
    private void DeleteItem()
    {
        imageComponent.sprite = trash_closed;
        DestroyImmediate(itemToBeDeleted.gameObject);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
        trashAlertUI.SetActive(false);
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
 
        if (DraggedItem != null && DraggedItem.GetComponent<InventoryItem>().esEliminable == true)
        {
            imageComponent.sprite = trash_opened;
        }
       
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        if (DraggedItem != null && DraggedItem.GetComponent<InventoryItem>().esEliminable == true)
        {
            imageComponent.sprite = trash_closed;
        }
    }
}