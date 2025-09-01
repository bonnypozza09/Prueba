using UnityEngine;

public class ObjetoCurativo : MonoBehaviour
{
    public int cantidadCurar = 10;
    public float tiempoRespawn = 5f;
    bool puedeUsarse = true;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && puedeUsarse)
        {
            SistemaVidaPrueba sistemaVida = other.GetComponent<SistemaVidaPrueba>();
            if (sistemaVida != null)
            {
                sistemaVida.RecuperarVida(cantidadCurar);
                puedeUsarse = false;
                GetComponent<Renderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                Invoke("ReactivarObjetos", tiempoRespawn);
            }
        }
    }
    private void ReactivarObjetos()
    {
        puedeUsarse = true;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }
}
