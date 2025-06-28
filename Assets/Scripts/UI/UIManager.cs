using UnityEngine;

namespace UI
{


public class UIManager : MonoBehaviour
{
    private HealthBar _healthBar;

    private void Awake() {
        _healthBar = GetComponentInChildren<HealthBar>();
    }

    public void SetHealth(int health, int maxHealth) {
        if (_healthBar != null) {
            _healthBar.SetHealth(health, maxHealth);
        }
    }
}


}