using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }
    public bool elObjetivo;
    public GameObject selectedObject;
    public GameObject interaction_Info_UI;
    Text interaction_text;

    public Image middleReticle;
    public Image handIcon;

    [Header("Alerta Inventario Lleno")]
    private bool alertaActiva = false;
    public GameObject alertaInventarioFull;

    public bool handIsVisible;

    [Header("Variables del ChoppableTree")]
    public GameObject selectedTree;
    public GameObject chopHolder;
    [Header("UI Necesita Hacha")]
    public GameObject needAxeUI;
    public Text needAxeText;
    public Image needAxeIcon;


    private void Start()
    {
        interaction_text = interaction_Info_UI.GetComponent<Text>();
        elObjetivo = false;
        alertaInventarioFull.SetActive(false);
    }
    private void Awake() {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // REEMPLAZAR el método Update() en SelectionManager.cs con este código:

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            // Sistema de HarvestableResource (reemplaza el sistema específico de ChoppableTree)
            HarvestableResource harvestableResource = selectionTransform.GetComponent<HarvestableResource>();
            if (harvestableResource && harvestableResource.playerInRange)
            {
                if (harvestableResource.HasRequiredTool())
                {
                    // Jugador tiene herramienta requerida - mostrar UI normal de recolectar
                    middleReticle.enabled = false;
                    harvestableResource.canBeHarvested = true;
                    selectedTree = harvestableResource.gameObject;
                    chopHolder.gameObject.SetActive(true);
                    needAxeUI.SetActive(false);
                }
                else
                {
                    // Jugador NO tiene herramienta requerida - mostrar UI informativa
                    middleReticle.enabled = true;
                    harvestableResource.canBeHarvested = false;
                    selectedTree = harvestableResource.gameObject;
                    chopHolder.gameObject.SetActive(false);
                    needAxeUI.SetActive(true);
                    
                    // Usar configuración del ResourceData
                    if (harvestableResource.resourceData != null)
                    {
                        needAxeText.text = harvestableResource.resourceData.toolNeededMessage;
                        
                        if (needAxeIcon != null && harvestableResource.resourceData.toolIcon != null)
                        {
                            needAxeIcon.sprite = harvestableResource.resourceData.toolIcon;
                        }
                    }
                }
            }
            else
            {
                // Ocultar UIs cuando no está en rango del recurso
                HideTreeUI();
                needAxeUI.SetActive(false);
            }

            // Resto del código de InteractableObject permanece igual
            if (interactable && interactable.jugadorEnRango)
            {
                bool hayObjetoCargado = InteractableObject.HayObjetoCargado();
                bool esObjetoCargado = InteractableObject.objetoCargado == interactable.gameObject;
                
                if (!hayObjetoCargado || esObjetoCargado)
                {
                    elObjetivo = true;
                    selectedObject = interactable.gameObject;
                    interaction_text.text = interactable.GetItemName();
                    interaction_Info_UI.SetActive(true);

                    if(interactable.CompareTag("Pickable"))
                    {
                        middleReticle.gameObject.SetActive(false);
                        handIcon.gameObject.SetActive(true);
                        handIsVisible = true;
                    }
                    else
                    {
                        middleReticle.gameObject.SetActive(true);
                        handIcon.gameObject.SetActive(false);
                        handIsVisible = false;
                    }
                }
                else
                {
                    elObjetivo = false;
                    interaction_Info_UI.SetActive(false);
                    middleReticle.gameObject.SetActive(true);
                    handIcon.gameObject.SetActive(false);
                    handIsVisible = false;
                }
            }
            else 
            { 
                elObjetivo = false;
                interaction_Info_UI.SetActive(false);
                middleReticle.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                handIsVisible = false;
            }
        }
        else
        {
            elObjetivo = false;
            interaction_Info_UI.SetActive(false);
            middleReticle.gameObject.SetActive(true);
            handIcon.gameObject.SetActive(false);
            handIsVisible = false;
            HideTreeUI();
        }
    }

// ELIMINAR el método HasAxeEquipped() ya que ahora se usa HasRequiredTool() del HarvestableResource
 
    // void Update()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;
    //     if (Physics.Raycast(ray, out hit))
    //     {
    //         var selectionTransform = hit.transform;
    //         InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

    //         //Sistema de ChoppableTree
    //         ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
    //         if (choppableTree && choppableTree.playerInRange)
    //         {
    //             if (HasAxeEquipped())
    //             {
    //                 // Jugador tiene hacha - mostrar UI normal de talar
    //                 middleReticle.enabled = false;
    //                 choppableTree.canBeChopped = true;
    //                 selectedTree = choppableTree.gameObject;
    //                 chopHolder.gameObject.SetActive(true);
    //                 needAxeUI.SetActive(false); // Ocultar UI de necesita hacha
    //             }
    //             else
    //             {
    //                 // Jugador NO tiene hacha - mostrar UI informativa
    //                 middleReticle.enabled = true;
    //                 choppableTree.canBeChopped = false;
    //                 selectedTree = choppableTree.gameObject;
    //                 chopHolder.gameObject.SetActive(false);
    //                 needAxeUI.SetActive(true); // Mostrar UI de necesita hacha
    //                 needAxeText.text = "Necesaria una Hacha";
    //                 // Obtener sprite dinÃ¡micamente del ScriptableObject
    //                 if (needAxeIcon != null)
    //                 {
    //                     VidaPlayerInfoItems axeData = InventorySystem.Instance.availableItems.Find(item => item.itemName == "Axe");
    //                     if (axeData != null && axeData.itemIcon != null)
    //                     {
    //                         needAxeIcon.sprite = axeData.itemIcon;
    //                     }
    //                     else
    //                     {
    //                         Debug.LogWarning("No se encontrÃ³ el ScriptableObject del Axe o no tiene itemIcon asignado");
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             // Ocultar ambas UIs cuando no estÃ¡ en rango del Ã¡rbol
    //             HideTreeUI();
    //             needAxeUI.SetActive(false);
    //         }
 
    //         if (interactable && interactable.jugadorEnRango)
    //         {
    //             // Verificar si hay un objeto cargado y si este objeto NO es el que está cargado
    //             bool hayObjetoCargado = InteractableObject.HayObjetoCargado();
    //             bool esObjetoCargado = InteractableObject.objetoCargado == interactable.gameObject;
                
    //             // Solo mostrar UI si no hay objeto cargado O si es el objeto que está cargado actualmente
    //             if (!hayObjetoCargado || esObjetoCargado)
    //             {
    //                 elObjetivo = true;
    //                 selectedObject = interactable.gameObject;
    //                 interaction_text.text = interactable.GetItemName();
    //                 interaction_Info_UI.SetActive(true);

    //                 if(interactable.CompareTag("Pickable"))
    //                 {
    //                     middleReticle.gameObject.SetActive(false);
    //                     handIcon.gameObject.SetActive(true);
    //                     handIsVisible = true;
    //                 }
    //                 else
    //                 {
    //                     middleReticle.gameObject.SetActive(true);
    //                     handIcon.gameObject.SetActive(false);
    //                     handIsVisible = false;
    //                 }
    //             }
    //             else
    //             {
    //                 // Hay un objeto cargado y este no es el objeto cargado - ocultar UI
    //                 elObjetivo = false;
    //                 interaction_Info_UI.SetActive(false);
    //                 middleReticle.gameObject.SetActive(true);
    //                 handIcon.gameObject.SetActive(false);
    //                 handIsVisible = false;
    //             }
    //         }
    //         else 
    //         { 
    //             elObjetivo = false;
    //             interaction_Info_UI.SetActive(false);
    //             middleReticle.gameObject.SetActive(true);
    //             handIcon.gameObject.SetActive(false);
    //             handIsVisible = false;

    //         }
    //     }
    //     else
    //     {
    //         elObjetivo = false;
    //         interaction_Info_UI.SetActive(false);
    //         middleReticle.gameObject.SetActive(true);
    //         handIcon.gameObject.SetActive(false);
    //         handIsVisible = false;
    //         HideTreeUI();
    //     }
    // }
    // private bool HasAxeEquipped()
    // {
    //     if(EquipSystem.Instance.selectedItem != null)
    //     {
    //         string itemName = EquipSystem.Instance.selectedItem.name.Replace("(Clone)", "");
    //         // Verificar si el item equipado es un hacha
    //         return itemName.Contains("Axe") || itemName.Contains("Hacha");
    //     }
    //     return false;
    // }


    public void AlertaInventarioFull(float duracion = 1.5f)
    {
        if(!alertaActiva)    
            StartCoroutine(MostrarYOcultarAlerta(duracion));
    } 

    private IEnumerator MostrarYOcultarAlerta(float duracion)
    {
        alertaActiva = true;
        alertaInventarioFull?.SetActive(true);
        yield return new WaitForSeconds(duracion);
        alertaInventarioFull?.SetActive(false);
        alertaActiva = false;
    }

    public void DisableSelection()
    {
        handIcon.enabled = false;
        middleReticle.enabled = false;
        interaction_Info_UI.SetActive(false);
        selectedObject = null;
        HideTreeUI();
        needAxeUI.SetActive(false);
    }

    public void HideTreeUI()//Metodo para ocultar la UI del arbol
    {
        if(selectedTree != null)
        {
            selectedTree.GetComponent<HarvestableResource>().canBeHarvested = false;
            selectedTree = null;
            chopHolder.gameObject.SetActive(false);
            needAxeUI.SetActive(false);
            middleReticle.enabled = true;
        }
    }

    public void EnableSelection()
    {
        handIcon.enabled = true;
        middleReticle.enabled = true;
        interaction_Info_UI.SetActive(true);
    }

}