using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if (transform.childCount > 0 )
            {
                return transform.GetChild(0).gameObject;
            }
 
            return null;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        //si no hay un elemento ya entonces establecer nuestro elemento.
        if (!Item)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.sonidoSoltarObjeto);
            
            DragDrop.artículoArrastrado.transform.SetParent(transform);
            DragDrop.artículoArrastrado.transform.localPosition = new Vector2(0, 0);

            if(transform.CompareTag("QuickSlot") == false)
            {
                DragDrop.artículoArrastrado.GetComponent<InventoryItem>().isInsideQuickSlot = false;
                InventorySystem.Instance.ReCalculateList();
            }

            if(transform.CompareTag("QuickSlot"))
            {
                DragDrop.artículoArrastrado.GetComponent<InventoryItem>().isInsideQuickSlot = true;
                InventorySystem.Instance.ReCalculateList();
            }
        }
    }
}