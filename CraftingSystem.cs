using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CraftingSystem : MonoBehaviour
{
    public KeyCode keyCode = KeyCode.C;
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI, survivalScreenUI, refineScreenUI, constructionScreenUI;
    public List<string> inventoryItemList = new();

    //Botones de categoría
    Button toolsBTN, survivalBTN, refineBTN, constructionBTN;

    //Botones de receta
    Button craftAxeBTN, craftPlankBTN, craftFoundationBTN, craftWallBTN;

    //Requerimientos Texto
    TextMeshProUGUI AxeReq1, AxeReq2, PlankReq1, FoundationReq1, WallReq1;

    public bool estabierto;
    public static CraftingSystem Instance { get; set; }
    public Blueprint AxeBLP = new("Axe", 1, 2, "Stone", 3, "Stick", 3);
    public Blueprint PlankBLP = new("Plank", 2, 1, "Log", 1, "", 0);
    public Blueprint FoundationBLP = new("Foundation", 1, 1, "Plank", 4, "", 0);
    public Blueprint WallBLP = new("Wall", 1, 1, "Plank", 2, "", 0);

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(() => OpenToolsCategory(true));//Nuevo

        refineBTN = craftingScreenUI.transform.Find("RefineButton").GetComponent<Button>();
        refineBTN.onClick.AddListener(() => OpenRefineCategory(true));//Nuevo

        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(() => OpenSurvivalCategory(true));//Nuevo

        constructionBTN = craftingScreenUI.transform.Find("ConstructionButton").GetComponent<Button>();
        constructionBTN.onClick.AddListener(() => OpenConstructionCategory(true));//Nuevo

        // Configuración inicial existente...
        Button toolsBackButton = toolsScreenUI.transform.Find("BackButton").GetComponent<Button>();
        toolsBackButton.onClick.AddListener(() => OpenToolsCategory(false));//Nuevo

        Button refineBackButton = refineScreenUI.transform.Find("BackButton").GetComponent<Button>();
        refineBackButton.onClick.AddListener(() => OpenRefineCategory(false));//Nuevo

        Button constructionBackButton = constructionScreenUI.transform.Find("BackButton").GetComponent<Button>();
        constructionBackButton.onClick.AddListener(() => OpenConstructionCategory(false));//Nuevo

        Button survivalBackButton = survivalScreenUI.transform.Find("BackButton").GetComponent<Button>();
        survivalBackButton.onClick.AddListener(() => OpenSurvivalCategory(false));//Nuevo
        
        //Axe
        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TextMeshProUGUI>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate {CraftAnyItem(AxeBLP);});

        //Plank
        PlankReq1 = refineScreenUI.transform.Find("Plank").transform.Find("req1").GetComponent<TextMeshProUGUI>();
    
        craftPlankBTN = refineScreenUI.transform.Find("Plank").transform.Find("Button").GetComponent<Button>();
        craftPlankBTN.onClick.AddListener(delegate {CraftAnyItem(PlankBLP);});

        //Foundation
        FoundationReq1 = constructionScreenUI.transform.Find("Foundation").transform.Find("req1").GetComponent<TextMeshProUGUI>();
    
        craftFoundationBTN = constructionScreenUI.transform.Find("Foundation").transform.Find("Button").GetComponent<Button>();
        craftFoundationBTN.onClick.AddListener(delegate {CraftAnyItem(FoundationBLP);});

        //Wall
        WallReq1 = constructionScreenUI.transform.Find("Wall").transform.Find("req1").GetComponent<TextMeshProUGUI>();
    
        craftWallBTN = constructionScreenUI.transform.Find("Wall").transform.Find("Button").GetComponent<Button>();
        craftWallBTN.onClick.AddListener(delegate {CraftAnyItem(WallBLP);});
    }
    void OpenToolsCategory(bool showToolsScreen)//Nuevo
    {
        craftingScreenUI.SetActive(!showToolsScreen);
        refineScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);

        toolsScreenUI.SetActive(showToolsScreen);
    }
    void OpenRefineCategory(bool showRefineScreen)//Nuevo
    {
        craftingScreenUI.SetActive(!showRefineScreen);
        toolsScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);

        refineScreenUI.SetActive(showRefineScreen);
    }
    void OpenSurvivalCategory(bool showSurvivalScreen)//Nuevo
    {
        craftingScreenUI.SetActive(!showSurvivalScreen);
        toolsScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);

        survivalScreenUI.SetActive(showSurvivalScreen);
    }
    void OpenConstructionCategory(bool showConstructionScreen)//Nuevo
    {
        craftingScreenUI.SetActive(!showConstructionScreen);
        toolsScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);

        constructionScreenUI.SetActive(showConstructionScreen);
    }

    void CraftAnyItem(Blueprint blueprinToCraft)
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.craftingSound);
        
        // Desactivar TODOS los botones inmediatamente
        craftAxeBTN.interactable = false;
        craftPlankBTN.interactable = false;
        craftFoundationBTN.interactable = false;
        craftWallBTN.interactable = false;
        
        // Reactivar después de X segundos (ajusta el tiempo que quieras)
        Invoke(nameof(ReactivateButton), 2f); // 2 segundos de delay

        if(InventorySystem.Instance.CheckSlotsAvailable(blueprinToCraft.numberOfItemsToProduce))
        {
            for(int i = 0; i < blueprinToCraft.numberOfItemsToProduce; i++)
            {
                InventorySystem.Instance.AddToInventory(blueprinToCraft.itemName, false);
            }
        }
        else
        {
            // Cancelar el Invoke si no hay espacio y reactivar inmediatamente
            CancelInvoke(nameof(ReactivateButton));
            return;
        }

        if(blueprinToCraft.numOfRequirements == 1)
        {
            InventorySystem.Instance.RemoveItem(blueprinToCraft.Req1, blueprinToCraft.Req1amount);
        }
        else if(blueprinToCraft.numOfRequirements == 2)
        {
            InventorySystem.Instance.RemoveItem(blueprinToCraft.Req1, blueprinToCraft.Req1amount);
            InventorySystem.Instance.RemoveItem(blueprinToCraft.Req2, blueprinToCraft.Req2amount);
        }

        StartCoroutine(Calculate());
    }

    // Agregar este método:
    void ReactivateButton()
    {
        // Reactivar todos los botones de crafteo
        if(craftAxeBTN != null) craftAxeBTN.interactable = true;
        if(craftPlankBTN != null) craftPlankBTN.interactable = true;
        if(craftFoundationBTN != null) craftFoundationBTN.interactable = true;
        if(craftWallBTN != null) craftWallBTN.interactable = true;
    }

    public IEnumerator Calculate()
    {
        yield return 0;
        InventorySystem.Instance.ReCalculateList();
        RefreshNeededItems();
    }

    void Update()
    {
        if(Input.GetKeyDown(keyCode) && !estabierto)
        {
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            Cursor.visible = true;
            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
            estabierto = true;
        }
        else if(Input.GetKeyDown(keyCode) && estabierto)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            refineScreenUI.SetActive(false);
            survivalScreenUI.SetActive(false);
            constructionScreenUI.SetActive(false);

            if(!InventorySystem.Instance.estabierto)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SelectionManager.Instance.EnableSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            }
            estabierto = false;
        }

    }
    
    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;
        int log_count = 0;
        int plank_count = 0;

        inventoryItemList = InventorySystem.Instance.itemList;
        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count+=1;
                    break;
                case "Stick":
                    stick_count+=1;
                    break;
                case "Log":
                    log_count+=1;
                    break;
                case "Plank":
                    plank_count+=1;
                    break;
            }
        }
        //---Hacha---//
        AxeReq1.text = "3 Stone [" + stone_count + "]";
        AxeReq2.text = "3 Stick [" + stick_count + "]";

        if (stone_count >= 3 && stick_count >= 3 && InventorySystem.Instance.CheckSlotsAvailable(1))
            craftAxeBTN.gameObject.SetActive(true);
        else
            craftAxeBTN.gameObject.SetActive(false);

        //---Plank---//
        PlankReq1.text = "1 Log [" + log_count + "]";

        if (log_count >= 1 && InventorySystem.Instance.CheckSlotsAvailable(2))
            craftPlankBTN.gameObject.SetActive(true);
        else
            craftPlankBTN.gameObject.SetActive(false);
            
        //---Foundation---//
        FoundationReq1.text = "4 Plank [" + plank_count + "]";

        if (plank_count >= 4 && InventorySystem.Instance.CheckSlotsAvailable(1))
            craftFoundationBTN.gameObject.SetActive(true);
        else
            craftFoundationBTN.gameObject.SetActive(false);
        
        //---Wall---//
        WallReq1.text = "2 Plank [" + plank_count + "]";

        if (plank_count >= 2 && InventorySystem.Instance.CheckSlotsAvailable(1))
            craftWallBTN.gameObject.SetActive(true);
        else
            craftWallBTN.gameObject.SetActive(false);
    }
}
