using UnityEngine;

[CreateAssetMenu(fileName = "IconoConfig", menuName = "Configuracion/Icono Config")]
public class VidaPlayerInfoItems : ScriptableObject
{
    [Range(0f, 1f)] public float porcentajeCompleto = 0.75f;
    [Range(0f, 1f)] public float porcentajeMedio = 0.25f;
    public Sprite iconoCompleto;
    public Sprite iconoMedio;
    public Sprite iconoVacio;

    [Header("Configuración de Transiciones")]
    public bool usarTransiciones = true;
    public TipoTransicion tipoTransicion = TipoTransicion.Fade;
    [Range(0.1f, 2f)] public float duracionTransicion = 0.3f;

    [Header("Información Básica De Items")]
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    [TextArea(2, 4)]
    public string functionality;
    public Sprite itemIcon;
    [Header("Propiedades")]
    public ItemType itemType;
    public int maxStackSize = 1;
    public bool isConsumable = false;
    
    [Header("Propiedades de Consumo")]
    [Range(0, 100)] public int caloriasRecuperadas = 0;
    [Range(0, 100)] public int hidratacionRecuperada = 0;
}

public enum TipoTransicion
{
    Instantaneo,
    Fade,
    Scale,
    Bounce
}
public enum ItemType
{
    Tool,
    Resource,
    Consumable,
    Weapon,
    Other
}
