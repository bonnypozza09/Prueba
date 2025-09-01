using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour 
{
    public GameObject balaPrefab;
    public Transform puntoSpawn;
    public float velocidadBala;
    public int poolBalas;
    private int contadorBalas;
    public KeyCode disparar;
    public KeyCode recargar;
    public float tiempoRecarga;
    private bool recargando = false;
    private Coroutine recargaCoroutine;

    private Queue<GameObject> bulletPool = new();
    
    void Start() 
    {
        // Obtener referencias del WeaponUIManager
        WeaponUIManager uiManager = WeaponUIManager.Instance;
        if (uiManager != null)
        {
            // Crear contenedor de balas si no está asignado
            if(uiManager.contenedorBalas == null)
            {
                GameObject contenedor = new("ContenedorBalas");
                uiManager.contenedorBalas = contenedor.transform;
            }
        }
        contadorBalas = 0;
        ConfigurarBarra();
        ActualizarUI();
    }
    public void InitializeBulletPool()
    {
        // Solo crear pool si no existe
        if (bulletPool.Count == 0)
        {
            WeaponUIManager uiManager = WeaponUIManager.Instance;
            if (uiManager != null)
            {
                // Crear contenedor de balas si no está asignado
                if(uiManager.contenedorBalas == null)
                {
                    GameObject contenedor = new("ContenedorBalas");
                    uiManager.contenedorBalas = contenedor.transform;
                }
            }
            
            // Crear pool de balas
            for (int i = 0; i < poolBalas; i++) 
            {
                GameObject bullet = Instantiate(balaPrefab, uiManager?.contenedorBalas);
                bullet.SetActive(false);
                bulletPool.Enqueue(bullet);
            }
            
            contadorBalas = poolBalas;
            ConfigurarBarra();
            ActualizarUI();
        }
    }
    
    void Update() 
    {
        bool anyUIOpen = InventorySystem.Instance.estabierto || CraftingSystem.Instance.estabierto;

        if (Input.GetKeyDown(disparar) && !recargando && !anyUIOpen) 
        {
            Shoot();
        }
        if(Input.GetKeyDown(recargar) && !recargando && !anyUIOpen)
        {
            recargaCoroutine = StartCoroutine(AnimacionRecarga());
        }

    }

    public void StopReloading()
    {
        if (recargaCoroutine != null)
        {
            StopCoroutine(recargaCoroutine);
            recargaCoroutine = null;
        }
        recargando = false;
    }
    
    void Shoot() 
    {
        if (contadorBalas > 0 && bulletPool.Count > 0 && !recargando) 
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.transform.position = puntoSpawn.position;
            bullet.transform.rotation = puntoSpawn.rotation;
            bullet.SetActive(true);

            contadorBalas--;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetPool(this);
            }
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero; // Resetear velocidad
            rb.linearVelocity = puntoSpawn.forward * velocidadBala;
            ActualizarUI();
        }
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        WeaponUIManager uiManager = WeaponUIManager.Instance;
        bullet.transform.SetParent(uiManager?.contenedorBalas);
        bulletPool.Enqueue(bullet);
    }

    IEnumerator AnimacionRecarga()
    {
        recargando = true;
        int balasIniciales = contadorBalas;
        
        // Limpiar balas activas
        GameObject[] activeBullets = GameObject.FindGameObjectsWithTag("Bala");
        foreach (GameObject bullet in activeBullets) 
        {
            if (bullet.activeInHierarchy) 
            {
                bullet.SetActive(false);
                // Mover a contenedor
                // bullet.transform.SetParent(contenedorBalas);
                bulletPool.Enqueue(bullet);
            }
        }
        
        float tiempoTranscurrido = 0f;
        
        while (tiempoTranscurrido < tiempoRecarga)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / tiempoRecarga;
            
            // Interpolar entre balas iniciales y poolSize
            contadorBalas = Mathf.RoundToInt(Mathf.Lerp(balasIniciales, poolBalas, progreso));
            ActualizarUI();
            
            yield return null;
        }
        
        contadorBalas = poolBalas;
        ActualizarUI();
        recargando = false;
        recargaCoroutine = null;
    }

    void ConfigurarBarra()
    {
        WeaponUIManager uiManager = WeaponUIManager.Instance;
        if (uiManager?.barraBalas != null)
        {
            uiManager.barraBalas.maxValue = poolBalas;
            uiManager.barraBalas.value = contadorBalas;
        }
    }

    void ActualizarUI()
    {
        WeaponUIManager uiManager = WeaponUIManager.Instance;
        if(uiManager?.contadorBalasText != null)
        {
            uiManager.contadorBalasText.text = contadorBalas + "/" + poolBalas;
        }
        if (uiManager?.barraBalas != null)
        {
            uiManager.barraBalas.value = contadorBalas;
        }
    }
}
