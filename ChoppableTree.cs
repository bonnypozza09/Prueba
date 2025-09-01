using UnityEngine;

public class ChoppableTree : HarvestableResource
{
    // Este script ahora solo contiene lógica específica de árboles si es necesaria
    // La mayoría del comportamiento está en la clase base HarvestableResource
    
    protected override void Start()
    {
        base.Start();
        
        // Configuración específica de árboles si es necesaria
        // Por ejemplo, configurar el ResourceData por defecto si no está asignado
        if (resourceData == null)
        {
            Debug.LogWarning("ResourceData no asignado en " + gameObject.name + ". Asigna un ResourceData para árboles.");
        }
    }
    
    // Puedes override métodos de la clase base si necesitas comportamiento específico para árboles
    // Por ejemplo:
    /*
    public override void GetHit()
    {
        // Lógica específica para árboles antes del hit
        base.GetHit(); // Llamar a la lógica base
        // Lógica específica para árboles después del hit
    }
    */
}

// using UnityEngine;
// using System.Collections;

// [RequireComponent(typeof(CapsuleCollider))]
// public class ChoppableTree : MonoBehaviour
// {
//     public bool playerInRange;
//     public bool canBeChopped;

//     [Header("Vida maxima")]
//     public float treeMaxHealth;
//     public float treeHealth;
//     [SerializeField] private Animator animator;
    
//     [Header("Consumo Por Talar Arboles")]
//     private PlayerStatsManager  statsManager;
//     [SerializeField] private int consumoHidratacion;
//     [SerializeField] private int consumoCalorias;

//     [Header("Prefabs de Árbol y Altura del arbol")]
//     [SerializeField] private GameObject stumpPrefab;
//     public GameObject originalTreePrefab;
//     [SerializeField] private float stumpHeight = 0.5f;

//     [Header("Sistema de Regeneración")]
//     [SerializeField] private float regenerationTime = 30f;


//     private void Start()
//     {
//         treeHealth = treeMaxHealth;
//         animator = transform.parent.transform.parent.GetComponent<Animator>();
//         statsManager = FindFirstObjectByType<PlayerStatsManager>();

//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInRange = true;
//         }
//     }
    
//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInRange = false;
//         }
//     }

//     public void GetHit()
//     {
//         animator.SetTrigger("shake");
//         treeHealth = Mathf.Max(0, treeHealth -1);
//         // Consumir hidratación y calorias al talar
//         if(statsManager != null)
//         {
//             statsManager.sistemaHidratacion?.ConsumirHidratacion(consumoHidratacion);
//             statsManager.sistemaCalorias?.ConsumirCalorias(consumoCalorias);
//         }
//         if(treeHealth <= 0)
//         {
//             SelectionManager.Instance.enabled = false;
//             canBeChopped = false;
//             SelectionManager.Instance.selectedTree = null;
//             SelectionManager.Instance.chopHolder.gameObject.SetActive(false);
//             SelectionManager.Instance.middleReticle.enabled = true;
//             StartCoroutine(TreeIsDead());
//         }
//     }

//     IEnumerator TreeIsDead()
//     {
//         GameObject treeParent = transform.parent.transform.parent.gameObject;
//         Vector3 baseTreePosition = treeParent.transform.position;
        
//         // Crear stump con configuración
//         GameObject stump = null;
//         if (stumpPrefab != null)
//         {
//             stump = Instantiate(stumpPrefab, baseTreePosition, Quaternion.identity);
            
//             TreeStump stumpScript = stump.AddComponent<TreeStump>();
//             // Pasar el prefab específico del árbol actual
//             stumpScript.specificTreePrefab = treeParent;
//             stumpScript.regenerationTime = regenerationTime;
            
//             if (stump.GetComponent<Renderer>() != null)
//             {
//                 stumpHeight = stump.GetComponent<Renderer>().bounds.size.y;
//             }
            
//             Vector3 elevatedPosition = new Vector3(
//                 treeParent.transform.position.x,
//                 treeParent.transform.position.y + stumpHeight,
//                 treeParent.transform.position.z
//             );
//             treeParent.transform.position = elevatedPosition;
//         }
        
//         // Configurar caída
//         Rigidbody rb = treeParent.AddComponent<Rigidbody>();
//         rb.mass = 15f;
//         rb.linearDamping = 0.3f;
//         rb.angularDamping = 0.5f;
        
//         Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
//         rb.AddForceAtPosition(randomDirection * 200f, transform.position + Vector3.up * 3f);
        
//         yield return new WaitForSeconds(3f);
        
//         Vector3 finalTreePosition = treeParent.transform.position;
//         Quaternion finalTreeRotation = treeParent.transform.rotation;
//         SelectionManager.Instance.enabled = true;
        
//         // CAMBIO IMPORTANTE: Destruir en lugar de desactivar
//         // para evitar que el prefab mantenga estados no deseados
//         Destroy(treeParent);
        
//         GameObject brokenTree = Instantiate(Resources.Load<GameObject>("Prefabs/ChoppedTree"), finalTreePosition, finalTreeRotation);
//     }

//     private void Update()
//     {
//         if(canBeChopped)
//         {
            
//             GlobalState.Instance.resourceHealth = treeHealth;
//             GlobalState.Instance.resourceMaxHealth = treeMaxHealth;
//         }
//     }
// }
