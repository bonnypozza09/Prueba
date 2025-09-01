using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SistemaHidratacion : MonoBehaviour
{
    [Header("Estadísticas de Hidratación")]
    public int hidratacionMaxima = 100;
    public int hidratacionActual;
    
    [Header("UI")]
    public TextMeshProUGUI textoHidratacion;
    public Slider barraHidratacion;

    [Header("Iconos Dinámicos")]
    public Image iconoEstadistica;  // ¡Esta variable te faltaba!
    public VidaPlayerInfoItems configuracion;
    void Start()
    {
        hidratacionActual = hidratacionMaxima;
        ActualizarUI();
    }
    
    public void ConsumirHidratacion(int cantidad)
    {
        hidratacionActual -= cantidad;
        hidratacionActual = Mathf.Clamp(hidratacionActual, 0, hidratacionMaxima);
        ActualizarUI();
    }
    
    public void RecuperarHidratacion(int cantidad)
    {
        hidratacionActual += cantidad;
        hidratacionActual = Mathf.Clamp(hidratacionActual, 0, hidratacionMaxima);
        ActualizarUI();
    }
    
    private void ActualizarUI()
    {
        if (textoHidratacion != null)
            textoHidratacion.text = "" + hidratacionActual;

        if (barraHidratacion != null)
        {
            barraHidratacion.maxValue = hidratacionMaxima;
            barraHidratacion.value = hidratacionActual;
        }
        IconoManager.ActualizarIconoPorPorcentaje(iconoEstadistica, hidratacionActual, hidratacionMaxima, configuracion);

    }
    
    public bool EstaSediento(int umbral = 20)
    {
        return hidratacionActual <= umbral;
    }
    
    public bool EstaCritico()
    {
        return hidratacionActual <= 0;
    }
}