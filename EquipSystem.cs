using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }
 
    // -- UI -- //
    public GameObject quickSlotsPanel;
 
    public List<GameObject> quickSlotsList = new();
    public GameObject numbersHolder;
    public int selectedNumber = -1;
    public GameObject selectedItem;
    public GameObject toolHolder; // Para equipar armas y herramientas desde el inventario
    public GameObject carryHolder; // Para cargar objetos interactivos del mundo (separado del toolHolder)
    public GameObject selectedItemModel;
    private Dictionary<string, GameObject> modelPool = new();


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
 
 
    private void Start()
    {
        PopulateSlotList();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectQuickSlot(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectQuickSlot(2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectQuickSlot(4);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectQuickSlot(5);
        }
    }

    void SelectQuickSlot(int number)
    {
        if(InteractableObject.HayObjetoCargado())return;

        if(CheckIfSlotIsFull(number) == true)
        {
            if(selectedNumber != number)
            {
                selectedNumber = number;

                if(selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                }

                selectedItem = GetSelectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                //Metodo para establecer objeto equipado
                SetEquippedItem(selectedItem);

                foreach(Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.black;
                }
                Text toBeChanged = numbersHolder.transform.Find("Number" + number).transform.Find("Text").GetComponent<Text>();
                toBeChanged.color = Color.white;
            }
            else
            {
                selectedNumber = -1;
                if(selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }

                //Ocultar el modelo de las manos del jugador
                if(selectedItemModel != null)
                {
                    PlayerShoot playerShoot = selectedItemModel.GetComponent<PlayerShoot>();
                    if (playerShoot != null)
                    {
                        playerShoot.StopReloading();
                    }
                    
                    selectedItemModel.SetActive(false);
                    selectedItemModel = null;
                    WeaponUIManager.Instance?.HideWeaponUI();
                }

                foreach(Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.black;
                }
            }

        }
    }

    // Método para desequipar el item actual
    public void DesequiparItemActual()
    {
        if (selectedNumber != -1)
        {
            selectedNumber = -1;
            if(selectedItem != null)
            {
                selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                selectedItem = null;
            }

            // Ocultar el modelo de las manos del jugador
            if(selectedItemModel != null)
            {
                PlayerShoot playerShoot = selectedItemModel.GetComponent<PlayerShoot>();
                if (playerShoot != null)
                {
                    playerShoot.StopReloading();
                }
                
                selectedItemModel.SetActive(false);
                selectedItemModel = null;
                WeaponUIManager.Instance?.HideWeaponUI();
            }

            foreach(Transform child in numbersHolder.transform)
            {
                child.transform.Find("Text").GetComponent<Text>().color = Color.black;
            }
        }
    }

    void SetEquippedItem(GameObject selectedItem)
    {
        // Ocultar modelo anterior si existe
        if(selectedItemModel != null)
        {
            selectedItemModel.SetActive(false);
        }
        WeaponUIManager.Instance?.HideWeaponUI();
        
        string selectedItemName = selectedItem.name.Replace("(Clone)", "");

        // Determinar el holder correcto según el tipo de item
        Transform targetHolder = toolHolder.transform; // Default holder

        // Si es un arma de fuego, usar el weaponHolder del WeaponUIManager
        InventoryItem itemScript = selectedItem.GetComponent<InventoryItem>();
        if (itemScript != null && selectedItemName.Contains("Arma")) // O usa otro criterio
        {
            WeaponUIManager uiManager = WeaponUIManager.Instance;
            if (uiManager?.weaponHolder != null)
            {
                targetHolder = uiManager.weaponHolder;
            }
        }
        
        // Verificar si ya existe en el pool
        if (!modelPool.ContainsKey(selectedItemName))
        {
            // Crear y agregar al pool
            GameObject newModel = Instantiate(Resources.Load<GameObject>("Models/" + selectedItemName + "_Model"), targetHolder);

            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.identity;
            modelPool[selectedItemName] = newModel;
        }
        
        // Activar modelo del pool
        selectedItemModel = modelPool[selectedItemName];
        selectedItemModel.SetActive(true);
        // Si es un arma, inicializar el pool de balas
        PlayerShoot playerShoot = selectedItemModel.GetComponent<PlayerShoot>();
        if (playerShoot != null)
        {
            playerShoot.InitializeBulletPool();
            WeaponUIManager.Instance?.ShowWeaponUI();
        }
    }

    GameObject GetSelectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber-1].transform.GetChild(0).gameObject;
    }


    bool CheckIfSlotIsFull(int slotNumber)
    {
        if(quickSlotsList[slotNumber-1].transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

 
    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }
 
    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);
 
        InventorySystem.Instance.ReCalculateList();
 
    }
 
 
    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }
 
    public bool CheckIfFull()
    {
 
        int counter = 0;
 
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }
 
        if (counter == 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}