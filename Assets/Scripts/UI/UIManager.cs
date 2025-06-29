using UnityEngine;
using UnityEngine.UI;

namespace UI
{


public class UIManager : MonoBehaviour
{
    private HealthBar _healthBar;
    [SerializeField] private Image jumpAvailable;
    [SerializeField] private Image jumpUnavailable;

    private void Awake() {
        _healthBar = GetComponentInChildren<HealthBar>();
    }

    public void SetHealth(int health, int maxHealth) {
        if (_healthBar != null) {
            _healthBar.SetHealth(health, maxHealth);
        }
    }

    public void SetJumpAvailable(bool available) {
        jumpAvailable?.gameObject.SetActive(available);
        jumpUnavailable?.gameObject.SetActive(!available);
    }
}


}