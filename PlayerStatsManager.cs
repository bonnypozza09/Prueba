using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("Referencias")]
    public SistemaVidaPrueba sistemaVida;
    public SistemaCalorias sistemaCalorias;
    public SistemaHidratacion sistemaHidratacion;
    
    [Header("Configuración de Decaimiento")]
    public float tiempoDecaimiento = 30f; // cada 30 segundos
    public int consumoTiempoCalorias = 1;
    public int consumoTiempoHidratacion = 1;

    [Header("Consumo por Decaimiento Vida")]
    public int consumoDecaimientoVida = 1;
    
    [Header("Regeneración de Vida")]
    public float tiempoRegeneracion = 5f; // cada 5 segundos
    public int cantidadRegeneracion = 2;
    public int umbralMinimo = 30; // necesario para regenerar vida
    private float contadorDecaimiento;
    private float contadorRegeneracion;
    [Header("Consumo por Recuperacion")]
    public int consumoPorRecuperacion = 1;
    
    void Start()
    {
        // Buscar sistemas automáticamente si no están asignados
        if (sistemaVida == null)
            sistemaVida = FindFirstObjectByType<SistemaVidaPrueba>();
            
        if (sistemaCalorias == null)
            sistemaCalorias = FindFirstObjectByType<SistemaCalorias>();
            
        if (sistemaHidratacion == null)
            sistemaHidratacion = FindFirstObjectByType<SistemaHidratacion>();
    }
    
    void Update()
    {
        // Manejar decaimiento de estadísticas
        contadorDecaimiento += Time.deltaTime;
        if (contadorDecaimiento >= tiempoDecaimiento)
        {
            ReducirEstadisticas(consumoTiempoCalorias, consumoTiempoHidratacion);
            contadorDecaimiento = 0f;
        }
        
        // Manejar regeneración de vida
        contadorRegeneracion += Time.deltaTime;
        if (contadorRegeneracion >= tiempoRegeneracion)
        {
            RegenerarVidaSiEsPosible();
            contadorRegeneracion = 0f;
        }
    }
    
    private void ReducirEstadisticas(int consumoCalorias, int consumoHidratacion)
    {
        // Reducir calorías usando el sistema individual
        if (sistemaCalorias != null)
            sistemaCalorias.ConsumirCalorias(consumoCalorias);
        
        // Reducir hidratación usando el sistema individual
        if (sistemaHidratacion != null)
            sistemaHidratacion.ConsumirHidratacion(consumoHidratacion);

        
        // Si están en estado crítico, dañar al jugador
        bool caloriasEnCero = sistemaCalorias != null && sistemaCalorias.EstaCritico();
        bool hidratacionEnCero = sistemaHidratacion != null && sistemaHidratacion.EstaCritico();
        
        if (caloriasEnCero || hidratacionEnCero)
        {
            if (sistemaVida != null)
                sistemaVida.RecibirDano(consumoDecaimientoVida);
        }
    }
    
    private void RegenerarVidaSiEsPosible()
    {
        // Solo regenerar si ambas estadísticas están por encima del umbral mínimo
        bool caloriasOk = sistemaCalorias != null && !sistemaCalorias.EstaHambriento(umbralMinimo);
        bool hidratacionOk = sistemaHidratacion != null && !sistemaHidratacion.EstaSediento(umbralMinimo);
        
        if (caloriasOk && hidratacionOk)
        {
            if (sistemaVida != null && sistemaVida.vidaActual < sistemaVida.vidaMaxima)
            {
                sistemaVida.RecuperarVida(cantidadRegeneracion);
                if(sistemaCalorias != null)
                    sistemaCalorias.ConsumirCalorias(consumoPorRecuperacion);
                if(sistemaHidratacion != null)
                    sistemaHidratacion.ConsumirHidratacion(consumoPorRecuperacion);
            }
        }
    }
    
    public void ConsumirComida(int cantidad)
    {
        if (sistemaCalorias != null)
            sistemaCalorias.RecuperarCalorias(cantidad);
    }
    
    public void ConsumirAgua(int cantidad)
    {
        if (sistemaHidratacion != null)
            sistemaHidratacion.RecuperarHidratacion(cantidad);
    }
    
    public string ObtenerEstadoJugador()
    {
        bool caloriasEnCero = sistemaCalorias != null && sistemaCalorias.EstaCritico();
        bool hidratacionEnCero = sistemaHidratacion != null && sistemaHidratacion.EstaCritico();
        bool hambriento = sistemaCalorias != null && sistemaCalorias.EstaHambriento(umbralMinimo);
        bool sediento = sistemaHidratacion != null && sistemaHidratacion.EstaSediento(umbralMinimo);
        
        if (caloriasEnCero || hidratacionEnCero)
            return "Crítico";
        else if (hambriento)
            return "Hambriento";
        else if (sediento)
            return "Sediento";
        else
            return "Normal";
    }
}

public static class IconoManager
{
    public static void ActualizarIconoPorPorcentaje(Image icono, float valorActual, float valorMaximo, VidaPlayerInfoItems configuracion)
    {
        if (icono == null || configuracion == null) return;
        
        float porcentaje = valorActual / valorMaximo;
        Sprite nuevoSprite = null;
        
        if (porcentaje > configuracion.porcentajeCompleto)
            nuevoSprite = configuracion.iconoCompleto;
        else if (porcentaje > configuracion.porcentajeMedio)
            nuevoSprite = configuracion.iconoMedio;
        else
            nuevoSprite = configuracion.iconoVacio;
            
        // Solo hacer transición si el sprite cambió y las transiciones están habilitadas
        if (icono.sprite != nuevoSprite)
        {
            if (configuracion.usarTransiciones)
            {
                MonoBehaviour mono = icono.GetComponent<MonoBehaviour>();
                if (mono != null)
                {
                    switch (configuracion.tipoTransicion)
                    {
                        case TipoTransicion.Fade:
                            mono.StartCoroutine(FadeTransition(icono, nuevoSprite, configuracion.duracionTransicion));
                            break;
                        case TipoTransicion.Scale:
                            mono.StartCoroutine(ScaleTransition(icono, nuevoSprite, configuracion.duracionTransicion));
                            break;
                        case TipoTransicion.Bounce:
                            mono.StartCoroutine(BounceTransition(icono, nuevoSprite, configuracion.duracionTransicion));
                            break;
                        default:
                            icono.sprite = nuevoSprite;
                            break;
                    }
                }
                else
                {
                    icono.sprite = nuevoSprite;
                }
            }
            else
            {
                icono.sprite = nuevoSprite;
            }
        }
    }
    
    private static System.Collections.IEnumerator FadeTransition(Image icono, Sprite nuevoSprite, float duracion)
    {
        float tiempoTranscurrido = 0f;
        Color colorOriginal = icono.color;
        
        // Fade out
        while (tiempoTranscurrido < duracion / 2)
        {
            float alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / (duracion / 2));
            icono.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        icono.sprite = nuevoSprite;
        
        // Fade in
        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracion / 2)
        {
            float alpha = Mathf.Lerp(0f, 1f, tiempoTranscurrido / (duracion / 2));
            icono.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        icono.color = colorOriginal;
    }
    
    private static System.Collections.IEnumerator ScaleTransition(Image icono, Sprite nuevoSprite, float duracion)
    {
        Vector3 escalaOriginal = icono.transform.localScale;
        float tiempoTranscurrido = 0f;
        
        // Scale down
        while (tiempoTranscurrido < duracion / 2)
        {
            float escala = Mathf.Lerp(1f, 0.8f, tiempoTranscurrido / (duracion / 2));
            icono.transform.localScale = escalaOriginal * escala;
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        icono.sprite = nuevoSprite;
        
        // Scale up
        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracion / 2)
        {
            float escala = Mathf.Lerp(0.8f, 1f, tiempoTranscurrido / (duracion / 2));
            icono.transform.localScale = escalaOriginal * escala;
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        icono.transform.localScale = escalaOriginal;
    }
    
    private static System.Collections.IEnumerator BounceTransition(Image icono, Sprite nuevoSprite, float duracion)
    {
        Vector3 escalaOriginal = icono.transform.localScale;
        float tiempoTranscurrido = 0f;
        
        // Scale down
        while (tiempoTranscurrido < duracion / 3)
        {
            float escala = Mathf.Lerp(1f, 0.7f, tiempoTranscurrido / (duracion / 3));
            icono.transform.localScale = escalaOriginal * escala;
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        icono.sprite = nuevoSprite;
        
        // Bounce up
        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracion / 3)
        {
            float escala = Mathf.Lerp(0.7f, 1.2f, tiempoTranscurrido / (duracion / 3));
            icono.transform.localScale = escalaOriginal * escala;
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        // Return to normal
        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracion / 3)
        {
            float escala = Mathf.Lerp(1.2f, 1f, tiempoTranscurrido / (duracion / 3));
            icono.transform.localScale = escalaOriginal * escala;
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        
        icono.transform.localScale = escalaOriginal;
    }
}