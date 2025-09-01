// using UnityEngine;
// using System.Collections;

// public class TreeStump : MonoBehaviour
// {
//     [Header("Configuración de Regeneración")]
//     [SerializeField] private GameObject originalTreePrefab;
//     public GameObject specificTreePrefab;
//     public float regenerationTime = 30f;

//     private bool canRegenerate = true;
//     private GameObject treeTemplate; // Plantilla del árbol específico
    
//     void Start()
//     {
//         // Crear una copia del árbol específico como plantilla antes de que sea destruido
//         if (specificTreePrefab != null)
//         {
//             treeTemplate = Instantiate(specificTreePrefab);
//             treeTemplate.SetActive(false); // Mantenerlo inactivo como plantilla
            
//             // Limpiar cualquier Rigidbody de la plantilla
//             Rigidbody templateRb = treeTemplate.GetComponent<Rigidbody>();
//             if (templateRb != null)
//             {
//                 DestroyImmediate(templateRb);
//             }
//         }
        
//         // Iniciar el proceso de regeneración automáticamente
//         if (canRegenerate)
//         {
//             StartCoroutine(RegenerateTree());
//         }
//     }
    
//     IEnumerator RegenerateTree()
//     {
//         yield return new WaitForSeconds(regenerationTime);
        
//         // Usar la plantilla del árbol específico si está disponible, sino usar el prefab genérico
//         GameObject prefabToUse = treeTemplate != null ? treeTemplate : originalTreePrefab;
        
//         if (prefabToUse != null)
//         {
//             // Instanciar nuevo árbol en la posición actual del tocón
//             GameObject newTree = Instantiate(prefabToUse, this.transform.position, Quaternion.identity);
            
//             // SOLUCIÓN NOMBRE: Limpiar el nombre para evitar acumulación de "(Clone)"
//             string cleanName = newTree.name.Replace("(Clone)", "").Trim();
//             newTree.name = cleanName;
            
//             // SOLUCIÓN 1: Asegurar que el árbol esté activo
//             newTree.SetActive(true);
            
//             // SOLUCIÓN 2: Limpiar cualquier Rigidbody residual
//             Rigidbody existingRb = newTree.GetComponent<Rigidbody>();
//             if (existingRb != null)
//             {
//                 DestroyImmediate(existingRb);
//             }
            
//             // Resetear la vida del nuevo árbol
//             ChoppableTree newTreeScript = newTree.GetComponentInChildren<ChoppableTree>();
//             if (newTreeScript != null)
//             {
//                 newTreeScript.treeHealth = newTreeScript.treeMaxHealth;
//                 newTreeScript.originalTreePrefab = prefabToUse;
                
//                 // SOLUCIÓN 3: Resetear estados del ChoppableTree
//                 newTreeScript.playerInRange = false;
//                 newTreeScript.canBeChopped = false;
//             }
            
//             // SOLUCIÓN 4: Reactivar el animator si existe
//             Animator treeAnimator = newTree.GetComponent<Animator>();
//             if (treeAnimator != null)
//             {
//                 treeAnimator.enabled = true;
//             }
            
//             // Limpiar la plantilla antes de destruir el tocón
//             if (treeTemplate != null)
//             {
//                 Destroy(treeTemplate);
//             }
            
//             Destroy(gameObject);
//         }
//     }
    
//     void OnDestroy()
//     {
//         // Limpiar la plantilla si el tocón es destruido prematuramente
//         if (treeTemplate != null)
//         {
//             Destroy(treeTemplate);
//         }
//     }
// }