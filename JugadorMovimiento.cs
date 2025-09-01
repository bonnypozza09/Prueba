using UnityEngine;

public class JugadorMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    private Vector3 lastPosition = new Vector3(0, 0, 0);
    public bool isMoving;

    [Header("Correr")]  
    [SerializeField] private float runSpeed = 20f;  
    private float currentSpeed; 

    [Header("Sensibilidad Mouse")]
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    float YRotation = 0f;

    [Header("Agotamiento Por Correr y Saltar")]
    public PlayerStatsManager statsManager;
    [Header("Agotamiento en Correr")]
    public float consumoHidratacionCorrer = -2f;
    public float consumoCaloriasCorrer = -1f;
    public float tiempoConsumoCorrer = 2f; // cada 2 segundos
    private float contadorConsumoCorrer;
    [Header("Agotamiento en Salto")]
    public float consumoHidratacionSalto = -2f;
    public float consumoCaloriasSalto = -3f;
    
    [Header("Sistema de Peso")]
    public float pesoMaximoParaCorrer = 5f; // Peso máximo para poder correr
    public float pesoMaximoParaSaltar = 10f; // Peso máximo para poder saltar
    public float factorReduccionVelocidad = 0.1f; // Factor de reducción por kg
    private float velocidadOriginal;
    private float velocidadCorrerOriginal;
    

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
        
        // Guardar velocidades originales
        velocidadOriginal = speed;
        velocidadCorrerOriginal = runSpeed;
    }
    void Update()
    {
        if (Time.frameCount % 3 == 0) CheckGrounded();
        
        // Aplicar restricciones de peso
        AplicarRestriccionesPeso();
        
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed;
        MovimientoJugador();
        RotacionCamaraJugador();
        bool estaCorriendoAhora = Input.GetKey(KeyCode.LeftShift) && isMoving;
        ConsumirEstadisticasCorrer(estaCorriendoAhora, consumoHidratacionCorrer, consumoCaloriasCorrer, tiempoConsumoCorrer);
    }
    private void CheckGrounded() {  
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);  
        if (isGrounded && velocity.y < 0) velocity.y = -2f;  
    } 

    void MovimientoJugador()
    {
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");  

        // AGREGAR ESTA LÍNEA:
        isMoving = move.sqrMagnitude > 0.01f;
        
        controller.Move(move * currentSpeed * Time.deltaTime);  

        if (Input.GetButtonDown("Jump") && isGrounded && PuedeJugadorSaltar()) {  
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); 

            ConsumirEstadisticasSaltar(consumoHidratacionSalto, consumoCaloriasSalto);
        }  
        velocity.y += gravity * Time.deltaTime;  
        controller.Move(velocity * Time.deltaTime);  

        if (transform.position != lastPosition && isGrounded == true)
        {
            isMoving = true;
            lastPosition = transform.position;
            SoundManager.Instance.PlaySound(SoundManager.Instance.grassWalkSound);
        }
        else
        {
            isMoving = false;
            SoundManager.Instance.grassWalkSound.Stop();
        }
        lastPosition = transform.position;
    }

    private void ConsumirEstadisticasSaltar(float consumoHidratacion, float consumoCalorias)
    {
        if (statsManager != null)
        {
            statsManager.ConsumirAgua((int)consumoHidratacion);
            statsManager.ConsumirComida((int)consumoCalorias);
        }
    }

    void RotacionCamaraJugador()
    {
        if(!InventorySystem.Instance.estabierto && !CraftingSystem.Instance.estabierto)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;  
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;  
            xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);  
            YRotation += mouseX;  
            transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f); 
        }
    }

    private void ConsumirEstadisticasCorrer(bool estaCorreindo, float consumoHidratacion, float consumoCalorias, float tiempoConsumo)
    {
        if (estaCorreindo)
        {
            contadorConsumoCorrer += Time.deltaTime;
            if (contadorConsumoCorrer >= tiempoConsumo)
            {
                if (statsManager != null)
                {
                    statsManager.ConsumirAgua((int)consumoHidratacion);
                    statsManager.ConsumirComida((int)consumoCalorias);
                }
                contadorConsumoCorrer = 0f;
            }
        }
        else
        {
            contadorConsumoCorrer = 0f; // Resetear si no está corriendo
        }
    }
    
    private void AplicarRestriccionesPeso()
    {
        float pesoActual = InteractableObject.GetPesoTotalCargado();
        
        // Calcular reducción de velocidad basada en el peso
        float reduccionVelocidad = pesoActual * factorReduccionVelocidad;
        
        // Aplicar velocidad reducida
        speed = Mathf.Max(velocidadOriginal - reduccionVelocidad, velocidadOriginal * 0.3f); // Mínimo 30% de velocidad original
        
        // Determinar si puede correr
        if (pesoActual <= pesoMaximoParaCorrer)
        {
            // Puede correr normalmente
            runSpeed = Mathf.Max(velocidadCorrerOriginal - reduccionVelocidad, velocidadCorrerOriginal * 0.5f);
        }
        else
        {
            // No puede correr, solo caminar
            runSpeed = speed;
        }
    }
    
    private bool PuedeJugadorSaltar()
    {
        float pesoActual = InteractableObject.GetPesoTotalCargado();
        return pesoActual <= pesoMaximoParaSaltar;
    }
}