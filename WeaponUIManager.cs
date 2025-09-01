using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager Instance { get; private set; }
    
    [Header("Referencias UI del Arma")]
    public TextMeshProUGUI contadorBalasText;
    public Slider barraBalas;
    public Transform contenedorBalas;
    public Transform weaponHolder;

    [Header("Panel UI del Arma")]
    public GameObject weaponUIPanel;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void ShowWeaponUI()
    {
        if (weaponUIPanel != null)
        {
            weaponUIPanel.SetActive(true);
        }
    }

    public void HideWeaponUI()
    {
        if (weaponUIPanel != null)
        {
            weaponUIPanel.SetActive(false);
        }
    }

    private void Start()
    {
        // Ocultar UI del arma al inicio
        HideWeaponUI();
    }
}