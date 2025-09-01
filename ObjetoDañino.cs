using UnityEngine;

public class ObjetoDañino : MonoBehaviour
{
    public int daño = 10;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            SistemaVidaPrueba sistemaVida = other.GetComponent<SistemaVidaPrueba>();
            if(sistemaVida != null)
            {
                sistemaVida.RecibirDano(daño);
            }
        }
    }
}
