using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("Informacion de los objetos interactivos")]
    public bool jugadorEnRango = false;
    public string ItemName;
    [SerializeField] private KeyCode recogerRecurso;
    
    [Header("Sistema de Carga")]
    public static GameObject objetoCargado = null;
    private static Vector3 posicionOriginalCargado;
    private static bool rigidBodyOriginalKinematic;
    private static Vector3 escalaOriginalCargado;
    
    [Header("Sistema de Peso")]
    public float pesoObjeto = 1f;
    private static float pesoTotalCargado = 0f;

    public string GetItemName() => ItemName;

    private void Update() {
        // Funcionalidad existente de recoger con tecla
        if (Input.GetKeyDown(recogerRecurso) && jugadorEnRango && SelectionManager.Instance.elObjetivo && 
            SelectionManager.Instance.selectedObject == gameObject && !HayObjetoCargado())
        {
            if(InventorySystem.Instance.CheckSlotsAvailable(1))
            {
                // Si este objeto está cargado, liberarlo antes de recogerlo
                if (objetoCargado == gameObject)
                {
                    SoltarObjetoCargadoFrenteJugador();
                }
                Destroy(gameObject);
                InventorySystem.Instance.AddToInventory(ItemName, true);
            }
            else
            {
                SelectionManager.Instance.AlertaInventarioFull();
            }
        }
        
        // Nueva funcionalidad de cargar con click izquierdo
        if (Input.GetMouseButtonDown(0) && jugadorEnRango && SelectionManager.Instance.elObjetivo && 
            SelectionManager.Instance.selectedObject == gameObject && CompareTag("Carryable") && 
            (objetoCargado == null || objetoCargado == gameObject))
        {
            ManejarCargaObjeto();
        }
        
        // Funcionalidad para desequipar objeto con click izquierdo cuando no se apunta a nada
        if (Input.GetMouseButtonDown(0) && !SelectionManager.Instance.elObjetivo && objetoCargado != null)
        {
            SoltarObjetoCargadoFrenteJugador();
        }
    }
    
    private void ManejarCargaObjeto()
    {
        if (objetoCargado == null)
        {
            // No hay objeto cargado, cargar este objeto
            CargarObjetoEnCarryHolder(gameObject);
        }
        else if (objetoCargado == gameObject)
        {
            // Este objeto ya está cargado, soltarlo
            SoltarObjetoCargadoFrenteJugador();
        }
        else
        {
            // Hay otro objeto cargado, intercambiar posiciones
            IntercambiarObjetos();
        }
    }
    
    public static bool HayObjetoCargado()
    {
        return objetoCargado != null;
    }
    
    private static void CargarObjetoEnCarryHolder(GameObject objeto)
    {
        if (EquipSystem.Instance != null && EquipSystem.Instance.carryHolder != null)
        {
            // Desequipar item actual antes de cargar nuevo
            EquipSystem.Instance.DesequiparItemActual();

            objetoCargado = objeto;
            posicionOriginalCargado = objeto.transform.position;
            escalaOriginalCargado = objeto.transform.localScale;
            
            // Sumar peso del objeto al peso total
            InteractableObject interactable = objeto.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                pesoTotalCargado += interactable.pesoObjeto;
            }
            
            // Manejar Rigidbody si existe
            Rigidbody rb = objeto.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rigidBodyOriginalKinematic = rb.isKinematic;
                rb.isKinematic = true;
            }
            
            // Mover el objeto al carryHolder
            objeto.transform.SetParent(EquipSystem.Instance.carryHolder.transform);
            objeto.transform.localPosition = Vector3.zero;
            objeto.transform.localRotation = Quaternion.identity;
        }
    }
    
    private static void SoltarObjetoCargadoFrenteJugador()
    {
        if (objetoCargado != null)
        {
            // Guardar referencia al objeto antes de liberarlo
            GameObject objetoASoltar = objetoCargado;
            
            // Restar peso del objeto del peso total
            InteractableObject interactable = objetoCargado.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                pesoTotalCargado -= interactable.pesoObjeto;
                if (pesoTotalCargado < 0) pesoTotalCargado = 0f;
            }
            
            // Calcular posición frente al jugador
            Transform jugador = Camera.main.transform;
            Vector3 posicionEnfrente = jugador.position + jugador.forward * 2f;
            
            // Restaurar el objeto frente al jugador
            RestaurarObjeto(objetoCargado, posicionEnfrente, escalaOriginalCargado, true);
            
            VerificarTodosLosObjetosCercanosConDelay();
            
            objetoCargado = null;
        }
    }
    
    private void IntercambiarObjetos()
    {
        if (objetoCargado != null)
        {
            // Guardar referencia al objeto actualmente cargado
            GameObject objetoAnterior = objetoCargado;
            
            // Soltar el objeto anterior frente al jugador
            Transform jugador = Camera.main.transform;
            Vector3 posicionEnfrente = jugador.position + jugador.forward * 2f;
            RestaurarObjeto(objetoAnterior, posicionEnfrente, escalaOriginalCargado, true);
            
            VerificarTodosLosObjetosCercanosConDelay();

            // Cargar este nuevo objeto
            CargarObjetoEnCarryHolder(gameObject);
        }
    }
    
    private static void RestaurarObjeto(GameObject objeto, Vector3 posicion, Vector3 escala, bool activarFisicaCompleta = false)
    {
        // Restaurar transform
        objeto.transform.SetParent(null);
        objeto.transform.position = posicion;
        objeto.transform.localScale = escala;
        
        // Manejar Rigidbody
        Rigidbody rb = objeto.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (activarFisicaCompleta)
            {
                rb.isKinematic = false;
            }
            else
            {
                rb.isKinematic = rigidBodyOriginalKinematic;
            }
        }
    }
    
    private static void VerificarTodosLosObjetosCercanosConDelay()
    {
        InteractableObject instancia = FindFirstObjectByType<InteractableObject>();
        if (instancia != null)
        {
            instancia.StartCoroutine(instancia.VerificarObjetosCercanosDespuesDeDelay());
        }
    }
    
    private IEnumerator VerificarObjetosCercanosDespuesDeDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        VerificarTodosLosObjetosCercanos();
    }
    
    private static void VerificarTodosLosObjetosCercanos()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;
        
        Collider[] objetosCercanos = Physics.OverlapSphere(jugador.transform.position, 5f);
        
        foreach (Collider col in objetosCercanos)
        {
            InteractableObject interactable = col.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                bool objetoEstaCargado = (objetoCargado == interactable.gameObject);
                
                if (!objetoEstaCargado)
                {
                    float distancia = Vector3.Distance(jugador.transform.position, interactable.transform.position);
                    
                    if (distancia <= 10f)
                    {
                        interactable.jugadorEnRango = true;
                    }
                    else
                    {
                        interactable.jugadorEnRango = false;
                    }
                }
                else
                {
                    interactable.jugadorEnRango = false;
                }
            }
        }
    }
    
    public static float GetPesoTotalCargado()
    {
        return pesoTotalCargado;
    }

    private void OnTriggerEnter(Collider other) => jugadorEnRango = other.CompareTag("Player");

    private void OnTriggerExit(Collider other) => 
        jugadorEnRango = other.CompareTag("Player") ? false : jugadorEnRango;
}