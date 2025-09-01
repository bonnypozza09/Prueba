using UnityEngine;
using UnityEngine.UI;

public class ResourceHealthBar : MonoBehaviour
{
    private Slider slider;
    private float currentHealth, MaxHealth;
    public GameObject globalState;
    public Text healthText;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (GlobalState.Instance != null)
        {
            currentHealth = GlobalState.Instance.resourceHealth;
            MaxHealth = GlobalState.Instance.resourceMaxHealth;
            
            if (MaxHealth > 0)
            {
                float fillValue = currentHealth / MaxHealth;
                slider.value = Mathf.Clamp01(fillValue);
                
                // Calcular y mostrar porcentaje
                float percentage = currentHealth / MaxHealth * 100f;
                healthText.text = Mathf.RoundToInt(percentage) + "%";
            }
            else
            {
                slider.value = 0f;
                healthText.text = "0%";
            }
        }
    }

    // private void Update()
    // {
    //     currentHealth = GlobalState.Instance.resourceHealth;
    //     MaxHealth = GlobalState.Instance.resourceMaxHealth;
    //     float fillValue = currentHealth / MaxHealth;
    //     slider.value = fillValue;
    // }


}
