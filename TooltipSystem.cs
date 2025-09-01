using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }
    
    [Header("Referencias Tooltip UI")]
    public GameObject panelInformacion;
    public Image itemImagen;
    public TextMeshProUGUI itemNombreText;
    public TextMeshProUGUI itemDescripcionText;
    public TextMeshProUGUI itemFuncionalidadText;

    [Header("Ajustes de posicionamiento")]
    public Vector2 offset = new Vector2(10, 10);
    public float rellenoPantalla = 20f;
    
    private RectTransform tooltipRect;
    private Canvas canvasPadre;
    private Coroutine ocultarCorrutina;
    private bool tooltipActivo = false;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        tooltipRect = panelInformacion.GetComponent<RectTransform>();
        canvasPadre = GetComponentInParent<Canvas>();
        panelInformacion.SetActive(false);
    }

    void Update()
    {
        // Solo actualizar la posición si el tooltip está activo
        if (tooltipActivo && panelInformacion.activeInHierarchy)
        {
            ActualizarPosicionTooltip();
        }
    }
    
    public void ShowTooltip(string itemName, string description, string functionality, Sprite itemSprite = null)
    {
        // Cancelar cualquier corrutina de ocultado en progreso
        if (ocultarCorrutina != null)
        {
            StopCoroutine(ocultarCorrutina);
            ocultarCorrutina = null;
        }
        
        tooltipActivo = true;
        panelInformacion.SetActive(true);
        
        itemNombreText.text = itemName;
        itemDescripcionText.text = !string.IsNullOrEmpty(description) ? description : "Sin descripción";
        itemFuncionalidadText.text = !string.IsNullOrEmpty(functionality) ? functionality : "Sin funcionalidad";
        
        // Mostrar el icono del item si está disponible
        if (itemImagen != null)
        {
            if (itemSprite != null)
            {
                itemImagen.sprite = itemSprite;
                itemImagen.gameObject.SetActive(true);
                // Asegurar que la imagen tenga el componente Image habilitado
                itemImagen.enabled = true;
            }
            else
            {
                itemImagen.gameObject.SetActive(false);
            }
        }
        
        ActualizarPosicionTooltip();
    }
    
    public void HideTooltip()
    {
        if (ocultarCorrutina != null)
        {
            StopCoroutine(ocultarCorrutina);
            ocultarCorrutina = null;
        }
        tooltipActivo = false;
        panelInformacion.SetActive(false);
    }
    
    private void ActualizarPosicionTooltip()
    {
        Vector2 mousePosition = Input.mousePosition;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasPadre.transform as RectTransform, mousePosition,canvasPadre.worldCamera,out Vector2 localPoint);

        Vector2 targetPosition = localPoint + offset;
        Vector2 canvasSize = (canvasPadre.transform as RectTransform).sizeDelta;
        Vector2 tooltipSize = tooltipRect.sizeDelta;
        
        if (targetPosition.x + tooltipSize.x > canvasSize.x / 2 - rellenoPantalla)
            targetPosition.x = localPoint.x - offset.x - tooltipSize.x;
        
        if (targetPosition.y + tooltipSize.y > canvasSize.y / 2 - rellenoPantalla)
            targetPosition.y = localPoint.y - offset.y - tooltipSize.y;
        
        tooltipRect.localPosition = targetPosition;
    }
}