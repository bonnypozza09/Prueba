using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class InventorySystem : MonoBehaviour
{
   public static InventorySystem Instance { get; set; }
    public GameObject inventarioUI;
    public List<GameObject> slotList = new();
    public List<string> itemList = new();
    private GameObject itemToAdd;//Item a agregar
    private GameObject whatSlotToEquip;//Que ranura equipar
    public KeyCode keyCode = KeyCode.Tab;
    public bool estabierto;

    //Variables del metodo TriggerPickupPopUP
    [Header("Alerta Recoger Recursos")]
    private bool pickupAlertaActiva = false;
    public GameObject pickupAlert;
    public TextMeshProUGUI pickupName;
    public Image pickupImage;

    public List<VidaPlayerInfoItems> availableItems = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        estabierto = false;
        inventarioUI.SetActive(false);
        PopulateSlotList();
        Cursor.visible = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(keyCode) && !estabierto)
        {
            inventarioUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
            estabierto = true;
        }
        else if(Input.GetKeyDown(keyCode) && estabierto)
        {
            inventarioUI.SetActive(false);
            TooltipSystem.Instance.HideTooltip();
            if(!CraftingSystem.Instance.estabierto)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SelectionManager.Instance.EnableSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            }
            estabierto = false;
        }

    }
    private void PopulateSlotList()
    {
        foreach (Transform child in inventarioUI.transform)
        {
            if(child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    public void AddToInventory(string itemName, bool playSound)
    {
        if(playSound)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.pickupItemSound);
        }

        VidaPlayerInfoItems itemData = GetItemDataByName(itemName);
        if (itemData == null) return;
        
        whatSlotToEquip = FindNextEmptySlot();
        itemToAdd = Instantiate(Resources.Load<GameObject>("Items/" + itemName), whatSlotToEquip.transform.position,whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        // AÃ±adir TooltipTrigger automÃ¡ticamente
        if (itemToAdd.GetComponent<TooltipTrigger>() == null)
        {
            itemToAdd.AddComponent<TooltipTrigger>();
        }
        
        itemList.Add(itemName);
        TriggerPickupPopUP(itemName, itemData.itemIcon);
        ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }

    private VidaPlayerInfoItems GetItemDataByName(string itemName)
    {
        return availableItems.Find(item => item.itemName == itemName);
    }

    //Metodo para mostrar alerta de recoger recursos
    public void TriggerPickupPopUP(string itemName, Sprite itemSprite)
    {
        if(!pickupAlertaActiva)
            StartCoroutine(MostrarYOcultarPickupAlert(itemName, itemSprite));
    }
    private IEnumerator MostrarYOcultarPickupAlert(string itemName, Sprite itemSprite, float duracion = 0.5f)
    {
        pickupAlertaActiva = true;
        pickupAlert.SetActive(true);
        pickupName.text = itemName;
        pickupImage.sprite = itemSprite;
        yield return new WaitForSeconds(duracion);
        pickupAlert.SetActive(false);
        pickupAlertaActiva = false;
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if(slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckSlotsAvailable(int emptyMeeded)
    {
        int emptySlots = 0;
        foreach (GameObject slot in slotList)
        {
            if(slot.transform.childCount <= 0)
            {
                emptySlots++;
            }
        }
        return emptySlots >= emptyMeeded;
    }

    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        int counter = amountToRemove;
        for (var i = slotList.Count - 1; i >= 0; i--)
        {
            if(slotList[i].transform.childCount > 0)
            {
                if(slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter-=1;
                }
            }
        }

        ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();

    }
    public void ReCalculateList()
    {
        itemList.Clear();
        foreach (GameObject slot in slotList)
        {
            if(slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string result = name.Replace(str2, "");
                itemList.Add(result);
            }
        }
    }


}