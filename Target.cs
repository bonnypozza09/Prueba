using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour 
{
    public float wobbleDuration = 2f;
    public float fuerzaOcilacion = 30f;
    private float wobbleTimer = 0f;
    
    void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Bala")) 
        {
            // Reiniciar o extender el timer del tambaleo
            wobbleTimer = wobbleDuration;
            if (!IsInvoking("UpdateWobble")) 
            {
                InvokeRepeating("UpdateWobble", 0f, 0.02f);
            }
        }
    }
    
    void UpdateWobble() 
    {
        if (wobbleTimer > 0f) 
        {
            float tambaleo = Mathf.Sin(Time.time * 10f) * fuerzaOcilacion * (wobbleTimer/wobbleDuration);
            transform.rotation = Quaternion.Euler(tambaleo, 0, 0);
            wobbleTimer -= Time.deltaTime;
        }
        else 
        {
            transform.rotation = Quaternion.identity;
            CancelInvoke("UpdateWobble");
        }
    }
}