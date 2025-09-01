using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{
    public Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && InventorySystem.Instance.estabierto == false && CraftingSystem.Instance.estabierto == false && !SelectionManager.Instance.elObjetivo)
        {
            StartCoroutine(SwingSoundDelay());
            animator.SetTrigger("hit");
        }
    }

    public void GetHit()
    {
        GameObject selectedTree = SelectionManager.Instance.selectedTree;

        if(selectedTree != null)
        {
            // Nuevo sistema: verificar si es HarvestableResource primero
            HarvestableResource harvestableResource = selectedTree.GetComponent<HarvestableResource>();
            if (harvestableResource != null)
            {
                harvestableResource.GetHit();
            }
            else
            {
                // Fallback al sistema anterior para compatibilidad
                ChoppableTree choppableTree = selectedTree.GetComponent<ChoppableTree>();
                if (choppableTree != null)
                {
                    choppableTree.GetHit();
                }
            }
            
            SoundManager.Instance.PlaySound(SoundManager.Instance.chopSound);
        }
    }
    // public void GetHit()
    // {
    //     GameObject selectedTree = SelectionManager.Instance.selectedTree;

    //     if(selectedTree != null)
    //     {
    //         selectedTree.GetComponent<ChoppableTree>().GetHit();
    //         SoundManager.Instance.PlaySound(SoundManager.Instance.chopSound);
    //     }
    // }

    IEnumerator SwingSoundDelay()
    {
        yield return new WaitForSeconds(0.3f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.toolSwingSound);
    }

}
