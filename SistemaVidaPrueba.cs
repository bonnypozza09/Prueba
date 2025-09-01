using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SistemaVidaPrueba : MonoBehaviour

{
    public int vidaMaxima = 100;
    public int vidaActual;
    public TextMeshProUGUI textoVida;
    public Slider barraVida;

    [Header("Iconos Dinámicos")]
    public Image iconoEstadistica;  // ¡Esta variable te faltaba!
    public VidaPlayerInfoItems configuracion;


    private void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarUI();
    }
    public void RecibirDano(int cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        ActualizarUI();
    }
    public void RecuperarVida(int cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        textoVida.text = "" + vidaActual;
        barraVida.value = vidaActual;
        IconoManager.ActualizarIconoPorPorcentaje(iconoEstadistica, vidaActual, vidaMaxima, configuracion);
    }
}