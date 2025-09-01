using UnityEngine;

public class Bullet : MonoBehaviour 
{
    public float lifetime = 3f;
    public float fuerzaEmpuje = 500f;
    private PlayerShoot playerShoot;
    void Start() 
    {
        // En lugar de destruir, desactivar después del tiempo
        Invoke("DeactivateBullet", lifetime);
    }
    public void SetPool(PlayerShoot shooter)
    {
        playerShoot = shooter;
    }
    void OnCollisionEnter(Collision collision) 
    {
        // Empujar objetos con Rigidbody
        Rigidbody hitRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (hitRigidbody != null) 
        {
            Vector3 direccionEmpuje = collision.transform.position - transform.position;
            direccionEmpuje.y = 0; // Opcional: solo empujar horizontalmente
            direccionEmpuje.Normalize();
            
            hitRigidbody.AddForce(direccionEmpuje * fuerzaEmpuje);
        }
        
        DeactivateBullet();
    }
    
    void DeactivateBullet() 
    {
        CancelInvoke();
        
        // Regresar al pool en lugar de solo desactivar
        if (playerShoot != null)
        {
            playerShoot.ReturnBulletToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}