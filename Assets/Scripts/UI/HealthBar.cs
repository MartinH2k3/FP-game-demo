using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{

public class HealthBar : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI healthText;

    public void SetHealth(int health, int maxHealth) {
        if (slider != null) slider.value = (float)health / maxHealth;
        if (healthText != null) healthText.text = $"{health} HP";
    }

}

}
