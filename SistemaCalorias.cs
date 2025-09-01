using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SistemaCalorias : MonoBehaviour
{
    [Header("Estadísticas de Calorías")]
    public int caloriasMaximas = 100;
    public int caloriasActuales;
    
    [Header("UI")]
    public TextMeshProUGUI textoCalorias;
    public Slider barraCalorias;

    [Header("Iconos Dinámicos")]
    public Image iconoEstadistica;  // ¡Esta variable te faltaba!
    public VidaPlayerInfoItems configuracion;

    void Start()
    {
        caloriasActuales = caloriasMaximas;
        ActualizarUI();
    }
    
    public void ConsumirCalorias(int cantidad)
    {
        caloriasActuales -= cantidad;
        caloriasActuales = Mathf.Clamp(caloriasActuales, 0, caloriasMaximas);
        ActualizarUI();
    }
    
    public void RecuperarCalorias(int cantidad)
    {
        caloriasActuales += cantidad;
        caloriasActuales = Mathf.Clamp(caloriasActuales, 0, caloriasMaximas);
        ActualizarUI();
    }
    
    private void ActualizarUI()
    {
        if (textoCalorias != null)
            textoCalorias.text = "" + caloriasActuales;
            
        if (barraCalorias != null)
        {
            barraCalorias.maxValue = caloriasMaximas;
            barraCalorias.value = caloriasActuales;
        }

        IconoManager.ActualizarIconoPorPorcentaje(iconoEstadistica, caloriasActuales, caloriasMaximas, configuracion);

    }
    
    public bool EstaHambriento(int umbral = 20)
    {
        return caloriasActuales <= umbral;
    }
    
    public bool EstaCritico()
    {
        return caloriasActuales <= 0;
    }
}