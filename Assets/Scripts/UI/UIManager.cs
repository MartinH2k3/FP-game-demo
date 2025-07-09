using Characters.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{


public class UIManager : MonoBehaviour
{
    private HealthBar _healthBar;
    private ActiveAttackTypeIndicator _activeAttackTypeIndicator;
    [SerializeField] private Image jumpAvailable;
    [SerializeField] private Image jumpUnavailable;

    private void Awake() {
        _healthBar = GetComponentInChildren<HealthBar>();
        _activeAttackTypeIndicator = GetComponentInChildren<ActiveAttackTypeIndicator>();
    }

    public void SetHealth(int health, int maxHealth) {
        _healthBar?.SetHealth(health, maxHealth);

    }

    public void SetJumpAvailable(bool available) {
        jumpAvailable?.gameObject.SetActive(available);
        jumpUnavailable?.gameObject.SetActive(!available);
    }

    public void SetActiveAttackType(AttackType attackType) {
        _activeAttackTypeIndicator?.SetActiveAttackType(attackType);
    }

}


}